using System.Text;

namespace Mvp.Selections.Api.Extensions;

public static class StreamExtensions
{
    public static async Task<string?> ReadAsStringAsync(this Stream stream, Encoding? encoding = null)
    {
        using StreamReader reader = new(stream, encoding ?? Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }
}