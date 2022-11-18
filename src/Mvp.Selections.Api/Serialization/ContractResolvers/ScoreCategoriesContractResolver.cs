using System.Reflection;
using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.ContractResolvers
{
    public class ScoreCategoriesContractResolver : CamelCasePropertyNamesContractResolver
    {
        public static readonly ScoreCategoriesContractResolver Instance = new ();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty result;
            if (member.DeclaringType == typeof(Selection) && member.Name == nameof(Selection.Titles))
            {
                result = null;
            }
            else if (member.DeclaringType == typeof(Score) && member.Name == nameof(Score.ScoreCategories))
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
