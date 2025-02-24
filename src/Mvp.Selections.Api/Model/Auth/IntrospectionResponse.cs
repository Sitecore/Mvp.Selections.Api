using Newtonsoft.Json;

namespace Mvp.Selections.Api.Model.Auth;

public class IntrospectionResponse
{
    [JsonProperty("active")]
    public bool Active { get; set; }

    [JsonProperty("aud")]
    public string? Audience { get; set; }

    [JsonProperty("client_id")]
    public string? ClientId { get; set; }

    [JsonProperty("device_id")]
    public string? DeviceId { get; set; }

    [JsonProperty("exp")]
    public int? ExpirationTime { get; set; }

    [JsonProperty("iat")]
    public int? IssueTime { get; set; }

    [JsonProperty("iss")]
    public string? Issuer { get; set; }

    [JsonProperty("jti")]
    public string? TokenIdentifier { get; set; }

    [JsonProperty("nbf")]
    public int? NoProcessingBeforeTime { get; set; }

    [JsonProperty("scope")]
    public string? Scope { get; set; }

    [JsonProperty("sub")]
    public string? Subject { get; set; }

    [JsonProperty("token_type")]
    public string? TokenType { get; set; }

    [JsonProperty("uid")]
    public string? UserId { get; set; }

    [JsonProperty("username")]
    public string? UserName { get; set; }
}