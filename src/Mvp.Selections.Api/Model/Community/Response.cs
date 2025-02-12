using System.Net;

namespace Mvp.Selections.Api.Model.Community;

public class Response<T>
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Unused;

    public string Message { get; set; } = string.Empty;

    public T? Result { get; set; } = default;
}