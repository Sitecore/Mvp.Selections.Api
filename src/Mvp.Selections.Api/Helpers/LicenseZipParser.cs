using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Helpers.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Helpers;

public class LicenseZipParser(ILogger<LicenseZipParser> logger)
    : ILicenseZipParser
{
    public async Task<IList<License>> ParseAsync(IFormFile zipFile)
    {
        List<License> licenses = [];
        await using Stream zipStream = zipFile.OpenReadStream();
        using ZipArchive archive = new(zipStream, ZipArchiveMode.Read);

        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            string? xmlContent = null;
            if (entry.FullName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                await using Stream nestedStream = entry.Open();
                using ZipArchive nestedArchive = new(nestedStream, ZipArchiveMode.Read);
                ZipArchiveEntry? nestedXmlEntry = nestedArchive.Entries.FirstOrDefault(e => e.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase));

                if (nestedXmlEntry != null)
                {
                    xmlContent = await ReadContentFromEntryAsync(nestedXmlEntry);
                }
                else
                {
                    logger.LogWarning("No XML file found in nested zip: {EntryName}", entry.FullName);
                }
            }
            else if (entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                xmlContent = await ReadContentFromEntryAsync(entry);
            }
            else
            {
                logger.LogInformation("Skipping unsupported file type: {EntryName}", entry.FullName);
            }

            if (!string.IsNullOrEmpty(xmlContent))
            {
                XmlReaderSettings settings = new() { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null };
                using XmlReader reader = XmlReader.Create(new StringReader(xmlContent), settings);
                XmlDocument xmlDoc = new() { XmlResolver = null };
                xmlDoc.Load(reader);

                XmlNodeList expirationNode = xmlDoc.GetElementsByTagName("expiration");

                if (expirationNode.Count > 0)
                {
                    string expiration = expirationNode[0]!.InnerText;
                    if (!string.IsNullOrEmpty(expiration))
                    {
                        // ReSharper disable once StringLiteralTypo - This is the correct format
                        DateTime expiry = DateTime.ParseExact(expiration, "yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture);

                        string base64Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlContent));

                        License license = new(Guid.NewGuid())
                        {
                            LicenseContent = base64Content,
                            ExpirationDate = expiry,
                            AssignedUser = null,
                        };

                        licenses.Add(license);
                    }
                    else
                    {
                        logger.LogWarning("Expiration date is empty in XML for entry: {EntryName}", entry.FullName);
                    }
                }
                else
                {
                    logger.LogWarning("Expiration node not found in XML for entry: {EntryName}", entry.FullName);
                }
            }
            else
            {
                logger.LogInformation("No XML content found in entry: {EntryName}", entry.FullName);
            }
        }

        return licenses;
    }

    private static async Task<string> ReadContentFromEntryAsync(ZipArchiveEntry entry)
    {
        await using Stream entryStream = entry.Open();
        using StreamReader reader = new(entryStream);
        return await reader.ReadToEndAsync();
    }
}
