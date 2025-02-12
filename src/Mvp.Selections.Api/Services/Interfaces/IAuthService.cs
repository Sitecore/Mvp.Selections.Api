using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Mvp.Selections.Api.Model.Auth;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services.Interfaces;

public interface IAuthService
{
    public const string BearerScheme = "Bearer";

    Task<AuthResult> ValidateAsync(HttpRequest request, params Right[] rights);
}