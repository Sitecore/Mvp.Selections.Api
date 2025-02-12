using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Api.Helpers;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Properties;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services;

public class ProfileLinkService(
    ILogger<ProfileLinkService> logger,
    IProfileLinkRepository profileLinkRepository,
    IUserService userService,
    AvatarUriHelper avatarUriHelper)
    : IProfileLinkService
{
    public async Task<OperationResult<ProfileLink>> AddAsync(User user, Guid userId, ProfileLink profileLink)
    {
        OperationResult<ProfileLink> result = new();
        Tuple<bool, User?> validationResult = await ValidateAdd(result, user, userId, profileLink);
        if (validationResult.Item1)
        {
            ProfileLink newProfileLink = new(Guid.Empty)
            {
                Name = profileLink.Name,
                Uri = profileLink.Uri,
                Type = profileLink.Type,
                User = validationResult.Item2!
            };
            newProfileLink.ImageUri = await avatarUriHelper.GetImageUri(newProfileLink);
            result.Result = profileLinkRepository.Add(newProfileLink);
            await profileLinkRepository.SaveChangesAsync();
            result.StatusCode = HttpStatusCode.Created;
        }

        return result;
    }

    public async Task<OperationResult<ProfileLink>> RemoveAsync(User user, Guid userId, Guid id)
    {
        OperationResult<ProfileLink> result = new();
        ProfileLink? existingProfileLink = await profileLinkRepository.GetAsync(id, pl => pl.User);
        if (ValidateRemove(result, user, userId, id, existingProfileLink))
        {
            if (profileLinkRepository.RemoveAsync(existingProfileLink))
            {
                await profileLinkRepository.SaveChangesAsync();
            }

            result.StatusCode = HttpStatusCode.NoContent;
        }

        return result;
    }

    private async Task<Tuple<bool, User?>> ValidateAdd(OperationResult<ProfileLink> operation, User user, Guid userId, ProfileLink profileLink)
    {
        bool result = true;
        User? updateUser = null;
        if (user.Id != userId && !user.HasRight(Right.Admin))
        {
            logger.LogWarning("User '{Id}' attempted to add ProfileLink '{ProfileLink}' to User '{UserId}' but isn't authorized.", user.Id, profileLink, userId);
            operation.Messages.Add(Resources.ProfileLink_Add_ForbiddenFormat.Format(profileLink, userId));
            operation.StatusCode = HttpStatusCode.Forbidden;
            result = false;
        }
        else if (profileLink.Uri.Scheme != "https")
        {
            logger.LogInformation("User '{Id}' submitted an invalid ProfileLink.Uri '{Uri}'.", user.Id, profileLink.Uri);
            operation.Messages.Add(Resources.ProfileLink_Add_InvalidUriScheme);
            operation.StatusCode = HttpStatusCode.BadRequest;
            result = false;
        }
        else if ((updateUser = await userService.GetAsync(userId)) == null)
        {
            logger.LogInformation("User '{userId}' was not found.", userId);
            operation.Messages.Add(Resources.ProfileLink_Add_UserNotFoundFormat.Format(userId));
            operation.StatusCode = HttpStatusCode.BadRequest;
            result = false;
        }

        return new Tuple<bool, User?>(result, updateUser);
    }

    private bool ValidateRemove(OperationResult<ProfileLink> operation, User user, Guid userId, Guid id, ProfileLink? profileLink)
    {
        bool result = true;
        if (profileLink == null)
        {
            logger.LogInformation("ProfileLink '{id}' was not found.", id);
            operation.StatusCode = HttpStatusCode.NoContent;
            result = false;
        }
        else if ((user.Id != userId || profileLink.User.Id != userId) && !user.HasRight(Right.Admin))
        {
            logger.LogWarning("User '{Id}' attempted to remove ProfileLink '{ProfileLinkId}' of User '{UserId}' but isn't authorized.", user.Id, id, userId);
            operation.Messages.Add(Resources.ProfileLink_Remove_ForbiddenFormat.Format(id, userId));
            operation.StatusCode = HttpStatusCode.Forbidden;
            result = false;
        }

        return result;
    }
}