﻿using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

namespace Mvp.Selections.Api
{
    public class Program
    {
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

                    // Helpers
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

                    // Database
                    services.AddDbContextPool<Context>(options =>
                        options.UseSqlServer(
                            host.Configuration.GetConnectionString("MvpSelectionsData") !,
                            o => o.EnableRetryOnFailure().UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)));

                    // HttpClients
                    services.AddHttpClient<OktaClient>();
                    services.AddHttpClient<SearchIngestionClient>();
                })
                .Build();

            host.Run();
        }
    }
}