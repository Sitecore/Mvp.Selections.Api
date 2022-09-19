namespace Mvp.Selections.Client.Interfaces
{
    public interface ITokenProvider
    {
        Task<string> GetTokenAsync();
    }
}
