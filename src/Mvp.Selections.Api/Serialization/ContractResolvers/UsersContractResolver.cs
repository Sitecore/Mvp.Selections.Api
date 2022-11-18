using System.Linq;
using System.Reflection;
using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.ContractResolvers
{
    public class UsersContractResolver : CamelCasePropertyNamesContractResolver
    {
        public static readonly UsersContractResolver Instance = new ();

        private readonly string[] _userExcludedMembers = { nameof(User.Titles), nameof(User.Consents), nameof(User.Applications), nameof(User.Mentors), nameof(User.Reviews) };

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty result;
            if (member.DeclaringType == typeof(User) && _userExcludedMembers.Contains(member.Name))
            {
                result = null;
            }
            else if (member.DeclaringType == typeof(Region) && member.Name == nameof(Region.Countries))
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
