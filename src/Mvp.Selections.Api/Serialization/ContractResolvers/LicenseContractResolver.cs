using System.Reflection;
using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.ContractResolvers
{
    public class LicenseContractResolver : CamelCasePropertyNamesContractResolver
    {
        public static readonly LicenseContractResolver Instance = new();
        private readonly string[] _licenseExcludedMembers = [nameof(License.LicenseContent)];
        private readonly string[] _userIncludedMembers = [nameof(User.Id), nameof(User.Name), nameof(User.Email)];

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            if (member.DeclaringType == typeof(License) && _licenseExcludedMembers.Contains(member.Name))
            {
                return null!;
            }

            if (member.ReflectedType == typeof(User) && !_userIncludedMembers.Contains(member.Name))
            {
                return null!;
            }

            return base.CreateProperty(member, memberSerialization);
        }
    }
}
