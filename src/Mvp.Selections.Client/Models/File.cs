namespace Mvp.Selections.Client.Models;

/// <summary>
/// Model of a File response.
/// </summary>
public class File
{
    /// <summary>
    /// Gets or sets the file name.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the media type of the File.
    /// </summary>
    public string ContentType { get; set; } = "application/octet-stream";

    /// <summary>
    /// Gets or sets the binary content data.
    /// </summary>
    public Stream Content { get; set; } = Stream.Null;
}