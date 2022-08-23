using Mvp.Selections.Api.Helpers.Interfaces;

namespace Mvp.Selections.Api.Helpers
{
    public class CurrentUserNameProvider : ICurrentUserNameProvider
    {
        public string UserName { get; set; }

        public string GetCurrentUserName()
        {
            return UserName;
        }
    }
}
