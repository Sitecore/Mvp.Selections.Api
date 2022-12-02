using System.Reflection;
using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.ContractResolvers
{
    public class ApplicantContractResolver : CamelCasePropertyNamesContractResolver
    {
        public static readonly ApplicantContractResolver Instance = new ();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty result;
            if (member.DeclaringType == typeof(Region) && member.Name == nameof(Region.Countries))
            {
                result = null;
            }
            else if (member.DeclaringType == typeof(Country) && member.Name == nameof(Country.Users))
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
