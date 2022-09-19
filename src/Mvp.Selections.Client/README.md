# Sitecore MVP Selections API Client

## :arrow_forward: Usage
Provide the configuration through an environment variable such as 
`MvpSelectionsApiClient__BaseAddress: ${MVP_SELECTIONS_API}` or by 
adding it into the local.settings.json.

Create a TokenProvider:
```c#
namespace Example.Providers
{
    public class HttpContextTokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextTokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetTokenAsync()
        {
            string result = string.Empty;
            HttpContext context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                result = await context.GetTokenAsync("id_token");
            }

            return result;
        }
    }
}
```

Register the client and your tokenprovider on Startup:
```c#
using Example.Providers;
using Microsoft.Extensions.DependencyInjection;
using Mvp.Selections.Client.Extensions;
using Mvp.Selections.Client.Interfaces;

public void ConfigureServices(IServiceCollection services)
{
    services.AddScoped<ITokenProvider, HttpContextTokenProvider>();
    services.AddMvpSelectionsApiClient();
}
```

Using the client:
```c#
namespace Example.ViewComponents
{
    public class ExampleViewComponent : ViewComponent
    {
        private readonly MvpSelectionsApiClient _client;

        protected BaseViewComponent(MvpSelectionsApiClient client)
        {
            _client = client;
        }

        public override async Task<IViewComponentResult> InvokeAsync()
        {
            // This example tries to fetch the list of all users
            IViewComponentResult result;
            Response<IList<User>> usersResponse = await _client.GetUsersAsync(model.Page, model.PageSize);
            if (usersResponse.StatusCode == HttpStatusCode.OK && usersResponse.Result != null)
            {
                result = View(usersResponse.Result);
            }
            else
            {
                result = View();
            }

            return result;
        }
    }
}
```

## :hammer: Build

Package as a Nuget by running

`dotnet pack -c Release -p:NuspecFile=Mvp.Selections.Client.nuspec`