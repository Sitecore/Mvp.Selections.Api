namespace Mvp.Selections.Api.Helpers.Interfaces
{
    public interface ICurrentUserNameProvider : Data.Interfaces.ICurrentUserNameProvider
    {
        public string UserName { get; set; }
    }
}
