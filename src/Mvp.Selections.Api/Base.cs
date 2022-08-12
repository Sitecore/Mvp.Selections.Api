using Mvp.Selections.Api.Helpers.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;

namespace Mvp.Selections.Api
{
    public abstract class Base
    {
        protected const string JsonContentType = "application/json";

        protected const string PlainTextContentType = "text/plain";

        protected const string JwtBearerFormat = "JWT";

        protected Base(ISerializerHelper serializer, IAuthService authService)
        {
            Serializer = serializer;
            AuthService = authService;
        }

        protected ISerializerHelper Serializer { get; }

        protected IAuthService AuthService { get; }
    }
}
