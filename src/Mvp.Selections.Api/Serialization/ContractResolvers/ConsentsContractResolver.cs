using System.Reflection;
using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.ContractResolvers
{
    public class ConsentsContractResolver : CamelCasePropertyNamesContractResolver
    {
        public static readonly ConsentsContractResolver Instance = new ();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty result;
            if (member.DeclaringType == typeof(Consent) && member.Name == nameof(Consent.User))
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
