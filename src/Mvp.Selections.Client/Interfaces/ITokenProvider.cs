namespace Mvp.Selections.Client.Interfaces;

/// <summary>
/// Interface for the token provider used by the API Client.
/// </summary>
public interface ITokenProvider
{
    /// <summary>
    /// Retrieves the token to be used for the authentication of the API call.
    /// </summary>
    /// <returns>Authentication token.</returns>
    Task<string> GetTokenAsync();
}