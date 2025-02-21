using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Cache;
using Mvp.Selections.Api.Clients;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Helpers;
using Mvp.Selections.Api.Helpers.Interfaces;
using Mvp.Selections.Api.Serialization;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data;
using Mvp.Selections.Data.Repositories;
using Mvp.Selections.Data.Repositories.Interfaces;

namespace Mvp.Selections.Api;

// ReSharper disable once ClassNeverInstantiated.Global
public class Program
{
    // ReSharper disable once UnusedParameter.Global
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Clean main implementation.")]
    public static void Main(string[] args)
    {
        IHost host = new HostBuilder()
            .ConfigureFunctionsWebApplication()
            .ConfigureServices((host, services) =>
            {
                // Azure Functions Isolated
                services.AddApplicationInsightsTelemetryWorkerService();
                services.ConfigureFunctionsApplicationInsights();

                // Options
                services.AddOptions<OktaClientOptions>().Configure<IConfiguration>((options, configuration) =>
                    configuration.GetSection(OktaClientOptions.OktaClient).Bind(options));
                services.AddOptions<TokenOptions>().Configure<IConfiguration>((options, configuration) =>
                    configuration.GetSection(TokenOptions.Token).Bind(options));
                services.AddOptions<MvpSelectionsOptions>().Configure<IConfiguration>((options, configuration) =>
                    configuration.GetSection(MvpSelectionsOptions.MvpSelections).Bind(options));
                services.AddOptions<JsonOptions>().Configure<IConfiguration>((options, configuration) =>
                    configuration.GetSection(JsonOptions.Json).Bind(options));
                services.AddOptions<SearchIngestionClientOptions>().Configure<IConfiguration>((options, configuration) =>
                    configuration.GetSection(SearchIngestionClientOptions.SearchIngestionClient).Bind(options));
                services.AddOptions<XClientOptions>().Configure<IConfiguration>((options, configuration) =>
                    configuration.GetSection(XClientOptions.XClient).Bind(options));
                services.AddOptions<CommunityClientOptions>().Configure<IConfiguration>((options, configuration) =>
                    configuration.GetSection(CommunityClientOptions.CommunityClient).Bind(options));
                services.AddOptions<CacheOptions>().Configure<IConfiguration>((options, configuration) =>
                    configuration.GetSection(CacheOptions.Cache).Bind(options));
                services.AddOptions<SendClientOptions>().Configure<IConfiguration>((options, configuration) =>
                    configuration.GetSection(SendClientOptions.SendClient).Bind(options));

                // Helpers
                services.AddSingleton<ICacheManager, CacheManager>();
                services.AddSingleton<AvatarUriHelper, AvatarUriHelper>();
                services.AddScoped<ISerializer, JsonSerializer>();
                services.AddScoped<ICurrentUserNameProvider, CurrentUserNameProvider>();
                services.AddScoped<Data.Interfaces.ICurrentUserNameProvider>(s => s.GetRequiredService<ICurrentUserNameProvider>());

                // Services
                services.AddScoped<IAuthService, AuthService>();
                services.AddScoped<IUserService, UserService>();
                services.AddScoped<IRoleService, RoleService>();
                services.AddScoped<IRegionService, RegionService>();
                services.AddScoped<ICountryService, CountryService>();
                services.AddScoped<ISelectionService, SelectionService>();
                services.AddScoped<IApplicationService, ApplicationService>();
                services.AddScoped<IProductService, ProductService>();
                services.AddScoped<IMvpTypeService, MvpTypeService>();
                services.AddScoped<IConsentService, ConsentService>();
                services.AddScoped<IContributionService, ContributionService>();
                services.AddScoped<IProfileLinkService, ProfileLinkService>();
                services.AddScoped<IReviewService, ReviewService>();
                services.AddScoped<IScoreCategoryService, ScoreCategoryService>();
                services.AddScoped<IScoreService, ScoreService>();
                services.AddScoped<IApplicantService, ApplicationService>();
                services.AddScoped<IScoreCardService, ScoreCardService>();
                services.AddScoped<ICommentService, CommentService>();
                services.AddScoped<ITitleService, TitleService>();
                services.AddScoped<IMvpProfileService, UserService>();
                services.AddScoped<IMentorService, UserService>();

                // Repositories
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IRoleRepository, RoleRepository>();
                services.AddScoped<IRegionRepository, RegionRepository>();
                services.AddScoped<ICountryRepository, CountryRepository>();
                services.AddScoped<ISelectionRepository, SelectionRepository>();
                services.AddScoped<IApplicationRepository, ApplicationRepository>();
                services.AddScoped<IProductRepository, ProductRepository>();
                services.AddScoped<IMvpTypeRepository, MvpTypeRepository>();
                services.AddScoped<IConsentRepository, ConsentRepository>();
                services.AddScoped<IContributionRepository, ContributionRepository>();
                services.AddScoped<IProfileLinkRepository, ProfileLinkRepository>();
                services.AddScoped<IReviewRepository, ReviewRepository>();
                services.AddScoped<IScoreCategoryRepository, ScoreCategoryRepository>();
                services.AddScoped<IScoreRepository, ScoreRepository>();
                services.AddScoped<ICommentRepository, CommentRepository>();
                services.AddScoped<ITitleRepository, TitleRepository>();
                services.AddScoped<IDispatchRepository, DispatchRepository>();

                // Database
                services.AddDbContextPool<Context>(options =>
                    options.UseSqlServer(
                        host.Configuration.GetConnectionString("MvpSelectionsData")!,
                        o => o.EnableRetryOnFailure().UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)));

                // HttpClients
                services.AddHttpClient<OktaClient>();
                services.AddHttpClient<SearchIngestionClient>((provider, client) =>
                {
                    IOptions<SearchIngestionClientOptions> options =
                        provider.GetRequiredService<IOptions<SearchIngestionClientOptions>>();
                    client.BaseAddress = options.Value.BaseAddress;
                });
                services.AddHttpClient<XClient>((provider, client) =>
                {
                    IOptions<XClientOptions> options =
                        provider.GetRequiredService<IOptions<XClientOptions>>();
                    client.BaseAddress = options.Value.BaseAddress;
                });
                services.AddHttpClient<CommunityClient>((provider, client) =>
                {
                    IOptions<CommunityClientOptions> options =
                        provider.GetRequiredService<IOptions<CommunityClientOptions>>();
                    client.BaseAddress = options.Value.BaseAddress;
                });
                services.AddHttpClient<SendClient>((provider, client) =>
                {
                    IOptions<SendClientOptions> options =
                        provider.GetRequiredService<IOptions<SendClientOptions>>();
                    client.BaseAddress = options.Value.BaseAddress;
                });
            })
            .Build();

        host.Run();
    }
}