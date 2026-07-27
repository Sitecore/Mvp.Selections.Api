namespace Mvp.Selections.Api.Configuration;

public class GraphQlOptions
{
    public const string GraphQl = "GraphQl";

    public int MaxQueryDepth { get; set; } = 20;

    public int MaxPageSize { get; set; } = 100;

    public int DefaultPageSize { get; set; } = 100;

    public CorsOptions Cors { get; set; } = new();

    public class CorsOptions
    {
        public string[] AllowedOrigins { get; set; } = ["*"];
    }
}
