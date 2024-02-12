using System.Reflection;
using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.ContractResolvers
{
    public class MvpTypesContractResolver : CamelCasePropertyNamesContractResolver
    {
        public static readonly MvpTypesContractResolver Instance = new ();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty? result;
            if (member.DeclaringType == typeof(MvpType) && member.Name == nameof(MvpType.Selections))
            {
                result = null;
            }
            else
            {
                result = base.CreateProperty(member, memberSerialization);
            }

            return result!;
        }
    }
}
