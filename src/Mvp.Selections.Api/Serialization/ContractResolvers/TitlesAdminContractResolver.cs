using System.Linq;
using System.Reflection;
using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.ContractResolvers
{
    public class TitlesAdminContractResolver : CamelCasePropertyNamesContractResolver
    {
        public static readonly TitlesAdminContractResolver Instance = new();

        private readonly string[] _userExcludedMembers = [nameof(User.Consents), nameof(User.Applications), nameof(User.Mentors), nameof(User.Reviews), nameof(User.Email), nameof(User.Roles), nameof(User.Rights)];

        private readonly string[] _selectionExcludedMembers = [nameof(Selection.ApplicationsActive), nameof(Selection.ApplicationsEnd), nameof(Selection.ApplicationsStart), nameof(Selection.ReviewsActive), nameof(Selection.ReviewsEnd), nameof(Selection.ReviewsStart)];

        private readonly string[] _applicationExcludedMembers = [nameof(Application.Contributions), nameof(Application.Eligibility), nameof(Application.Mentor), nameof(Application.Objectives), nameof(Application.Reviews), nameof(Application.Status)];

        private readonly string[] _countryExcludedMembers = [nameof(Country.Region), nameof(Country.Users)];

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty? result;
            if (member.DeclaringType == typeof(User) && _userExcludedMembers.Contains(member.Name))
            {
                result = null;
            }
            else if (member.DeclaringType == typeof(Selection) && _selectionExcludedMembers.Contains(member.Name))
            {
                result = null;
            }
            else if (member.DeclaringType == typeof(Application) && _applicationExcludedMembers.Contains(member.Name))
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
}
