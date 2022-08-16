using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Helpers.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;

namespace Mvp.Selections.Api
{
    public abstract class Base<T>
        where T : Base<T>
    {
        protected const string JsonContentType = "application/json";

        protected const string PlainTextContentType = "text/plain";

        protected const string JwtBearerFormat = "JWT";

        protected Base(ILogger<T> logger, ISerializerHelper serializer, IAuthService authService)
        {
            Logger = logger;
            Serializer = serializer;
            AuthService = authService;
        }

        protected ILogger<T> Logger { get; }

        protected ISerializerHelper Serializer { get; }

        protected IAuthService AuthService { get; }
    }
}
