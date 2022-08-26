using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mvp.Selections.Client.Configuration;

namespace Mvp.Selections.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IHttpClientBuilder AddMvpSelectionsApiClient(this IServiceCollection services)
        {
            services.AddOptions<MvpSelectionsApiClientOptions>()
                .Configure<IConfiguration>((options, configuration) =>
                    configuration.GetSection(MvpSelectionsApiClientOptions.MvpSelectionsApiClient).Bind(options));
            return services.AddHttpClient<MvpSelectionsApiClient>();
        }
    }
}
