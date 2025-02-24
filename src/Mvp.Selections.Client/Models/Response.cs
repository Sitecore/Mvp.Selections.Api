using System.Net;

namespace Mvp.Selections.Client.Models;

/// <summary>
/// Model of an API response.
/// </summary>
/// <typeparam name="T">Type of the result.</typeparam>
public class Response<T>
{
    /// <summary>
    /// Gets or sets the status code.
    /// </summary>
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Unused;

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the result.
    /// </summary>
    public T? Result { get; set; }
}