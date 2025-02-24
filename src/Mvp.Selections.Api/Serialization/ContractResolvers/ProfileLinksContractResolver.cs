using System.Reflection;
using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.ContractResolvers;

public class ProfileLinksContractResolver : CamelCasePropertyNamesContractResolver
{
    public static readonly ProfileLinksContractResolver Instance = new();

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty? result;
        if (member.DeclaringType == typeof(ProfileLink) && member.Name == nameof(ProfileLink.User))
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