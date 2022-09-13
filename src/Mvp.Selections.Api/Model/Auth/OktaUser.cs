using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Mvp.Selections.Api.Configuration;

namespace Mvp.Selections.Api.Model.Auth
{
    public class OktaUser
    {
        public OktaUser()
        {
        }

        public OktaUser(JwtSecurityToken token, TokenOptions options)
        {
            Identifier = token.Subject;
            Name = token.Claims.FirstOrDefault(c => c.Type == options.NameClaimType)?.Value;
            Email = token.Claims.FirstOrDefault(c => c.Type == options.EmailClaimType)?.Value;
            AccountName = token.Claims.FirstOrDefault(c => c.Type == options.AccountNameClaimType)?.Value;
        }

        public string Identifier { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string AccountName { get; set; }
    }
}
