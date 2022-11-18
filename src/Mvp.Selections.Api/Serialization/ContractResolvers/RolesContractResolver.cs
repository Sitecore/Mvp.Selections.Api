using System.Linq;
using System.Reflection;
using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.ContractResolvers
{
    public class RolesContractResolver : CamelCasePropertyNamesContractResolver
    {
        public static readonly RolesContractResolver Instance = new ();

        private readonly string[] _userExcludedMembers =
        {
            nameof(User.Applications),
            nameof(User.Consents),
            nameof(User.Mentors),
            nameof(User.Reviews),
            nameof(User.Titles),
            nameof(User.Links),
            nameof(User.Roles)
        };

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty result;
            if (member.DeclaringType == typeof(User) && _userExcludedMembers.Contains(member.Name))
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
