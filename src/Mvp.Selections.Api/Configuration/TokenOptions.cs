namespace Mvp.Selections.Api.Configuration
{
    public class TokenOptions
    {
        public const string Token = "Token";

        public string FirstNameClaimType { get; set; } = "first_name";

        public string LastNameClaimType { get; set; } = "last_name";

        public string EmailClaimType { get; set; } = "email";

        public string AccountNameClaimType { get; set; } = "account_name";
    }
}
