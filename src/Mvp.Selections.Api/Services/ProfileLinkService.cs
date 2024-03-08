using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Helpers;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class ProfileLinkService(
        ILogger<ProfileLinkService> logger,
        IProfileLinkRepository profileLinkRepository,
        IUserService userService,
        AvatarUriHelper avatarUriHelper)
        : IProfileLinkService
    {
        public async Task<OperationResult<ProfileLink>> AddAsync(User user, Guid userId, ProfileLink profileLink)
        {
            OperationResult<ProfileLink> result = new ();
            if (user.Id == userId || user.HasRight(Right.Admin))
            {
                User? updateUser = await userService.GetAsync(userId);
                if (updateUser != null)
                {
                    ProfileLink newProfileLink = new (Guid.Empty)
                    {
                        Name = profileLink.Name,
                        Uri = profileLink.Uri,
                        Type = profileLink.Type,
                        User = updateUser
                    };
                    newProfileLink.ImageUri = await avatarUriHelper.GetImageUri(newProfileLink);
                    result.Result = profileLinkRepository.Add(newProfileLink);
                    await profileLinkRepository.SaveChangesAsync();
                    result.StatusCode = HttpStatusCode.Created;
                }
                else
                {
                    string message = $"User '{userId}' was not found.";
                    logger.LogInformation(message);
                    result.Messages.Add(message);
                }
            }
            else
            {
                string message = $"User '{user.Id}' attempted to add ProfileLink '{profileLink.Id}' to User '{userId}' but isn't authorized.";
                logger.LogWarning(message);
                result.Messages.Add(message);
                result.StatusCode = HttpStatusCode.Forbidden;
            }

            return result;
        }

        public async Task<OperationResult<ProfileLink>> RemoveAsync(User user, Guid userId, Guid id)
        {
            OperationResult<ProfileLink> result = new ();
            if (user.Id == userId || user.HasRight(Right.Admin))
            {
                if (await profileLinkRepository.RemoveAsync(id))
                {
                    await profileLinkRepository.SaveChangesAsync();
                }

                result.StatusCode = HttpStatusCode.NoContent;
            }
            else
            {
                string message = $"User '{user.Id}' attempted to remove ProfileLink '{id}' of User '{userId}' but isn't authorized.";
                logger.LogWarning(message);
                result.Messages.Add(message);
                result.StatusCode = HttpStatusCode.Forbidden;
            }

            return result;
        }
    }
}
