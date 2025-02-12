using System.Linq;
using System.Reflection;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Comments;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.ContractResolvers;

internal class CommentsContractResolver : CamelCasePropertyNamesContractResolver
{
    public static readonly CommentsContractResolver Instance = new();

    private readonly string[] _userExcludedMembers =
    [
        nameof(User.Consents),
        nameof(User.Applications),
        nameof(User.Mentors),
        nameof(User.Reviews),
        nameof(User.Email),
        nameof(User.Roles),
        nameof(User.Rights),
        nameof(User.Links)
    ];

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty? result;
        if (member.DeclaringType == typeof(ApplicationComment) && member.Name == nameof(ApplicationComment.Application))
        {
            result = null;
        }
        else if (member.DeclaringType == typeof(User) && _userExcludedMembers.Contains(member.Name))
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