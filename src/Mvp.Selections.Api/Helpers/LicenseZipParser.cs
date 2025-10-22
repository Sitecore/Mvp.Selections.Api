using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Mvp.Selections.Api.Helpers.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Helpers;

internal class LicenseZipParser : ILicenseZipParser
{
    public async Task<IList<License>> ParseAsync(IFormFile zipFile)
    {
        List<License> licenses = [];
        using ZipArchive archive = new(zipFile.OpenReadStream(), ZipArchiveMode.Read);

        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            XmlDocument xmldoc = new();
            string xmlContent = string.Empty;

            if (entry.FullName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                MemoryStream nestedZipStream = new();
                await entry.Open().CopyToAsync(nestedZipStream);
                nestedZipStream.Position = 0;

                using ZipArchive nestedArchive = new(nestedZipStream, ZipArchiveMode.Read);

                ZipArchiveEntry? nestedXMLEntry = nestedArchive.Entries.FirstOrDefault(e => e.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase));

                if (nestedXMLEntry != null)
                {
                    (xmldoc, xmlContent) = await ReadXmlFromEntryAsync(nestedXMLEntry);
                }
            }
            else if (entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                (xmldoc, xmlContent) = await ReadXmlFromEntryAsync(entry);
            }

            XmlNodeList expirationNode = xmldoc.GetElementsByTagName("expiration");

            if (expirationNode.Count > 0)
            {
                string expiration = expirationNode[0]!.InnerText;
                if (!string.IsNullOrEmpty(expiration))
                {
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
            }
        }

        return licenses;
    }

    private static async Task<(XmlDocument XmlDoc, string XmlContent)> ReadXmlFromEntryAsync(ZipArchiveEntry entry)
    {
        await using Stream entryStream = entry.Open();
        using StreamReader reader = new(entryStream);
        string xmlContent = await reader.ReadToEndAsync();

        XmlDocument xmldoc = new();
        xmldoc.LoadXml(xmlContent);

        return (xmldoc, xmlContent);
    }
}
