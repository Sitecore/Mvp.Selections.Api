using System.Reflection;
using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.ContractResolvers;

public class MentorsContractResolver : CamelCasePropertyNamesContractResolver
{
    public static readonly MentorsContractResolver Instance = new();

    private readonly string[] _countryExcludedMembers = [nameof(Country.Region), nameof(Country.Users)];

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty? result;
        if (member.DeclaringType == typeof(Region) && member.Name == nameof(Region.Countries))
        {
            result = null;
        }
        else if (member.DeclaringType == typeof(Country) && _countryExcludedMembers.Contains(member.Name))
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