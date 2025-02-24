using System.Reflection;
using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.ContractResolvers;

public class ContributionsContractResolver : CamelCasePropertyNamesContractResolver
{
    public static readonly ContributionsContractResolver Instance = new();

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty? result;
        if (member.DeclaringType == typeof(Contribution) && member.Name == nameof(Contribution.Application))
        {
            result = null;
        }
        else if (member.DeclaringType == typeof(Product) && member.Name == nameof(Product.Contributions))
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