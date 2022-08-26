# Sitecore MVP Selections API Client

## :cd: Installation

Package as a Nuget by running

`dotnet pack -c Release -p:NuspecFile=Mvp.Selections.Client.nuspec`

Add a Local Nuget repository to your Nuget.config

`<add key="Local" value="C:\Code\Local Nuget" />`

Add the Mvp.Selections.Client to your project.

## :arrow_forward: Usage
Provide the configuration through an environment variable such as 
`MvpSelectionsApiClient__BaseAddress: ${MVP_SELECTIONS_API}` or by 
adding it into the local.settings.json.

Register the client on Startup:
```c#
using Microsoft.Extensions.DependencyInjection;
using Mvp.Selections.Client.Extensions;

public void ConfigureServices(IServiceCollection services)
{
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
            Response<IList<User>> usersResponse = await _client.GetUsersAsync(await HttpContext.GetTokenAsync("id_token"), model.Page, model.PageSize);
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

## :wrench: Operations available
See the Postman collection included in the repository `MVP Selections API.postman_collection.json`