using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mvp.Selections.Client.Configuration;

namespace Mvp.Selections.Client.Extensions
{
    /// <summary>
    /// Extensions for <see cref="ServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the necessary registrations for the MVP Selections API Client.
        /// </summary>
        /// <param name="services"><see cref="ServiceCollection"/> to append to.</param>
        /// <returns><see cref="IHttpClientBuilder"/> to chain configuration.</returns>
        public static IHttpClientBuilder AddMvpSelectionsApiClient(this IServiceCollection services)
        {
            services.AddOptions<MvpSelectionsApiClientOptions>()
                .Configure<IConfiguration>((options, configuration) =>
                    configuration.GetSection(MvpSelectionsApiClientOptions.MvpSelectionsApiClient).Bind(options));
            return services.AddHttpClient<MvpSelectionsApiClient>();
        }
    }
}
