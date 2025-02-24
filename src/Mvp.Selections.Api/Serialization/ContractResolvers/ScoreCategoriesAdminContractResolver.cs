using System.Reflection;
using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.ContractResolvers;

public class ScoreCategoriesAdminContractResolver : CamelCasePropertyNamesContractResolver
{
    public static readonly ScoreCategoriesAdminContractResolver Instance = new();

    private readonly string[] _scoreCategoryExcludedMembers = [nameof(ScoreCategory.Selection), nameof(ScoreCategory.MvpType)];

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty? result;
        if (member.DeclaringType == typeof(ScoreCategory) && _scoreCategoryExcludedMembers.Contains(member.Name))
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

        return result!;
    }
}