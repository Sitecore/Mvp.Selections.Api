using System.Reflection;
using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.ContractResolvers
{
    internal class LicenseContractResolver : CamelCasePropertyNamesContractResolver
    {
        public static readonly LicenseContractResolver Instance = new();


        private readonly string[] _licenseExcludedMembers = [nameof(Mvp.Selections.Domain.License.LicenseContent)];

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            if (member.DeclaringType == typeof(Mvp.Selections.Domain.License) && _licenseExcludedMembers.Contains(member.Name))
            {
                return null!;
            }

            return base.CreateProperty(member, memberSerialization);
        }
    }
}
