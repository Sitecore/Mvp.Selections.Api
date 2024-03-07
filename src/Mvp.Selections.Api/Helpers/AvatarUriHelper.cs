using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mvp.Selections.Api.Clients;
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Domain;
using C = Mvp.Selections.Api.Model.Community;
using X = Mvp.Selections.Api.Model.X;

namespace Mvp.Selections.Api.Helpers
{
    public class AvatarUriHelper(XClient xClient, CommunityClient cClient)
    {
        public async Task<Uri?> GetImageUri(User user)
        {
            Uri? result;
            switch (user.ImageType)
            {
                case ImageType.Community:
                    ProfileLink? communityLink = user.Links.FirstOrDefault(l => l.Type == ProfileLinkType.Community);
                    result = await GetCommunityUri(communityLink);
                    break;
                case ImageType.Gravatar:
                    result = GetGravatarUri(user.Email);
                    break;
                case ImageType.Twitter:
                    ProfileLink? twitterLink = user.Links.FirstOrDefault(l => l.Type == ProfileLinkType.Twitter);
                    result = await GetTwitterUri(twitterLink);
                    break;
                case ImageType.Anonymous:
                default:
                    result = null;
                    break;
            }

            return result;
        }

        public async Task<Uri?> GetImageUri(ProfileLink link)
        {
            Uri? result;
            switch (link.Type)
            {
                case ProfileLinkType.Community:
                    result = await GetCommunityUri(link);
                    break;
                case ProfileLinkType.Twitter:
                    result = await GetTwitterUri(link);
                    break;
                default:
                    result = null;
                    break;
            }

            return result;
        }

        private static Uri? GetGravatarUri(string? email)
        {
            Uri? result = null;
            if (!string.IsNullOrWhiteSpace(email))
            {
                string hash = email.Trim().ToLowerInvariant().ToMD5Hash();
                result = new Uri($"https://www.gravatar.com/avatar/{hash}");
            }

            return result;
        }

        private async Task<Uri?> GetCommunityUri(ProfileLink? link)
        {
            Uri? result = null;
            if (link is { Type: ProfileLinkType.Community })
            {
                string? userId = CommunityClient.GetUserId(link.Uri);
                if (userId != null)
                {
                    C.Response<C.Profile> profileResponse = await cClient.GetProfile(userId);
                    if (profileResponse.StatusCode == HttpStatusCode.OK && !string.IsNullOrWhiteSpace(profileResponse.Result?.Photo?.Value))
                    {
                        result = cClient.GetAbsolutePath(profileResponse.Result.Photo.Value);
                    }
                }
            }

            return result;
        }

        private async Task<Uri?> GetTwitterUri(ProfileLink? link)
        {
            Uri? result = null;
            if (link is { Type: ProfileLinkType.Twitter })
            {
                // We only want to look up each user once a day to stay in Free tier
                if (link.ModifiedOn != null ? link.ModifiedOn < DateTime.UtcNow.AddDays(-1) : link.CreatedOn < DateTime.UtcNow.AddDays(-1))
                {
                    string username = link.Uri.Segments.Last();
                    X.Response<X.Profile> profileResponse = await xClient.GetProfile(username);
                    if (profileResponse.StatusCode == HttpStatusCode.OK && !string.IsNullOrWhiteSpace(profileResponse.Result?.Data?.ProfileImage))
                    {
                        result = new Uri(profileResponse.Result.Data.ProfileImage);
                    }
                }
                else
                {
                    result = link.ImageUri;
                }
            }

            return result;
        }
    }
}
