using System.Reflection;
using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.ContractResolvers
{
    public class ScoresContractResolver : CamelCasePropertyNamesContractResolver
    {
        public static readonly ScoresContractResolver Instance = new();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty? result;
            if (member.DeclaringType == typeof(Score) && member.Name == nameof(Score.ScoreCategories))
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
