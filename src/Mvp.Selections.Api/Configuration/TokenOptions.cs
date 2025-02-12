namespace Mvp.Selections.Api.Configuration;

public class TokenOptions
{
    public const string Token = "Token";

    public string NameClaimType { get; set; } = "name";

    public string EmailClaimType { get; set; } = "email";

    public string AccountNameClaimType { get; set; } = "account_name";
}