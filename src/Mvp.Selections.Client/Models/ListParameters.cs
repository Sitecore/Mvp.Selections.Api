namespace Mvp.Selections.Client.Models;

/// <summary>
/// Model for list parameters.
/// </summary>
public class ListParameters
{
    /// <summary>
    /// Query string key for page.
    /// </summary>
    public const string PageQueryStringKey = "p";

    /// <summary>
    /// Query string key for page size.
    /// </summary>
    public const string PageSizeQueryStringKey = "ps";

    /// <summary>
    /// Gets or sets the current page.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the current page size.
    /// </summary>
    public short PageSize { get; set; } = 100;

    /// <summary>
    /// Create query string representation of the parameters.
    /// </summary>
    /// <param name="isFirst">If true the first separator will be '?' instead of '&amp;'.</param>
    /// <returns>A querystring version of the <see cref="ListParameters"/>.</returns>
    public string ToQueryString(bool isFirst = false)
    {
        string start = isFirst ? "?" : "&";
        return $"{start}{PageQueryStringKey}={Page}&{PageSizeQueryStringKey}={PageSize}";
    }
}