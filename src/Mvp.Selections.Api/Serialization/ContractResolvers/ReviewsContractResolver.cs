﻿using System.Linq;
using System.Reflection;
using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.ContractResolvers
{
    public class ReviewsContractResolver : CamelCasePropertyNamesContractResolver
    {
        public static readonly ReviewsContractResolver Instance = new ();

        private readonly string[] _userExcludedMembers = { nameof(User.Titles), nameof(User.Consents), nameof(User.Applications), nameof(User.Mentors), nameof(User.Reviews), nameof(User.Roles) };

        private readonly string[] _reviewCategoryScoreExcludedMembers = { nameof(ReviewCategoryScore.ReviewId), nameof(ReviewCategoryScore.Review), nameof(ReviewCategoryScore.ScoreCategory), nameof(ReviewCategoryScore.Score) };

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty result;
            if (member.DeclaringType == typeof(Review) && member.Name == nameof(Review.Application))
            {
                result = null;
            }
            else if (member.DeclaringType == typeof(User) && _userExcludedMembers.Contains(member.Name))
            {
                result = null;
            }
            else if (member.DeclaringType == typeof(ReviewCategoryScore) && _reviewCategoryScoreExcludedMembers.Contains(member.Name))
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
