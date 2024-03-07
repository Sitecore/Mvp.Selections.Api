using System.Linq;
using System.Reflection;
using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.ContractResolvers
{
    public class MvpProfileContractResolver : CamelCasePropertyNamesContractResolver
    {
        public static readonly MvpProfileContractResolver Instance = new ();

        private readonly string[] _userExcludedMembers = [nameof(User.Consents), nameof(User.Applications), nameof(User.Mentors), nameof(User.Reviews), nameof(User.Email), nameof(User.Roles), nameof(User.Rights)];

        private readonly string[] _selectionExcludedMembers = [nameof(Selection.ApplicationsActive), nameof(Selection.ApplicationsEnd), nameof(Selection.ApplicationsStart), nameof(Selection.ReviewsActive), nameof(Selection.ReviewsEnd), nameof(Selection.ReviewsStart), nameof(Selection.MvpTypes)];

        private readonly string[] _applicationExcludedMembers = [nameof(Application.Contributions), nameof(Application.Eligibility), nameof(Application.Mentor), nameof(Application.Objectives), nameof(Application.Reviews), nameof(Application.Status), nameof(Application.Applicant)];

        private readonly string[] _countryExcludedMembers = [nameof(Country.Region), nameof(Country.Users)];

        private readonly string[] _titleExcludedMembers = [nameof(Title.Warning)];

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty? result;
            if (member.DeclaringType == typeof(Region) && member.Name == nameof(Region.Countries))
            {
                result = null;
            }
            else if (member.DeclaringType == typeof(MvpType) && member.Name == nameof(MvpType.Selections))
            {
                result = null;
            }
            else if (member.DeclaringType == typeof(Contribution) && member.Name == nameof(Contribution.Application))
            {
                result = null;
            }
            else if (member.DeclaringType == typeof(Product) && member.Name == nameof(Product.Contributions))
            {
                result = null;
            }
            else if (member.DeclaringType == typeof(ProfileLink) && member.Name == nameof(ProfileLink.User))
            {
                result = null;
            }
            else if (member.DeclaringType == typeof(User) && _userExcludedMembers.Contains(member.Name))
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
            else if (member.DeclaringType == typeof(Title) && _titleExcludedMembers.Contains(member.Name))
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
