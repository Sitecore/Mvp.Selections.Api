using Mvp.Selections.Api.Helpers.Interfaces;

namespace Mvp.Selections.Api.Helpers
{
    public class CurrentUserNameProvider : ICurrentUserNameProvider
    {
        public string UserName { get; set; } = string.Empty;

        public string GetCurrentUserName()
        {
            return UserName;
        }
    }
}
