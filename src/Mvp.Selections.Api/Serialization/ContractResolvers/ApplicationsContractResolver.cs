using System.Linq;
using System.Reflection;
using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.ContractResolvers
{
    public class ApplicationsContractResolver : CamelCasePropertyNamesContractResolver
    {
        public static readonly ApplicationsContractResolver Instance = new ();

        private readonly string[] _userExcludedMembers = { nameof(User.Consents), nameof(User.Applications), nameof(User.Mentors), nameof(User.Reviews), nameof(User.Email) };

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty result;
            if (member.DeclaringType == typeof(Application) && member.Name == nameof(Application.Reviews))
            {
                result = null;
            }
            else if (member.DeclaringType == typeof(Region) && member.Name == nameof(Region.Countries))
            {
                result = null;
            }
            else if (member.DeclaringType == typeof(Contribution) && member.Name == nameof(Contribution.Application))
            {
                result = null;
            }
            else if (member.DeclaringType == typeof(User) && _userExcludedMembers.Contains(member.Name))
            {
                result = null;
            }
            else if (member.DeclaringType == typeof(Product) && member.Name == nameof(Product.Contributions))
            {
                result = null;
            }
            else if (member.DeclaringType == typeof(Country) && member.Name == nameof(Country.Users))
            {
                result = null;
            }
            else if (member.DeclaringType == typeof(ProfileLink) && member.Name == nameof(ProfileLink.User))
            {
                result = null;
            }
            else
            {
                result = base.CreateProperty(member, memberSerialization);
            }

            return result;
        }
    }
}
