using Microsoft.Extensions.DependencyInjection;

namespace Mvp.Selections.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IHttpClientBuilder AddMvpSelectionsApiClient(this IServiceCollection services)
        {
            return services.AddHttpClient<MvpSelectionsApiClient>();
        }
    }
}
