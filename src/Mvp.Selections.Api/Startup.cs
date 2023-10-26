using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

[assembly: FunctionsStartup(typeof(Mvp.Selections.Api.Startup))]

namespace Mvp.Selections.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Options
            builder.Services.AddOptions<OktaClientOptions>().Configure<IConfiguration>((options, configuration) =>
                configuration.GetSection(OktaClientOptions.OktaClient).Bind(options));
            builder.Services.AddOptions<TokenOptions>().Configure<IConfiguration>((options, configuration) =>
                configuration.GetSection(TokenOptions.Token).Bind(options));
            builder.Services.AddOptions<MvpSelectionsOptions>().Configure<IConfiguration>((options, configuration) =>
                configuration.GetSection(MvpSelectionsOptions.MvpSelections).Bind(options));
            builder.Services.AddOptions<JsonOptions>().Configure<IConfiguration>((options, configuration) =>
                configuration.GetSection(JsonOptions.Json).Bind(options));

            // Helpers
            builder.Services.AddScoped<ISerializer, JsonSerializer>();
            builder.Services.AddScoped<ICurrentUserNameProvider, CurrentUserNameProvider>();
            builder.Services.AddScoped<Data.Interfaces.ICurrentUserNameProvider>(s => s.GetRequiredService<ICurrentUserNameProvider>());

            // Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IRegionService, RegionService>();
            builder.Services.AddScoped<ICountryService, CountryService>();
            builder.Services.AddScoped<ISelectionService, SelectionService>();
            builder.Services.AddScoped<IApplicationService, ApplicationService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IMvpTypeService, MvpTypeService>();
            builder.Services.AddScoped<IConsentService, ConsentService>();
            builder.Services.AddScoped<IContributionService, ContributionService>();
            builder.Services.AddScoped<IProfileLinkService, ProfileLinkService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IScoreCategoryService, ScoreCategoryService>();
            builder.Services.AddScoped<IScoreService, ScoreService>();
            builder.Services.AddScoped<IApplicantService, ApplicationService>();
            builder.Services.AddScoped<IScoreCardService, ScoreCardService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<ITitleService, TitleService>();
            builder.Services.AddScoped<IMvpProfileService, UserService>();

            // Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IRegionRepository, RegionRepository>();
            builder.Services.AddScoped<ICountryRepository, CountryRepository>();
            builder.Services.AddScoped<ISelectionRepository, SelectionRepository>();
            builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IMvpTypeRepository, MvpTypeRepository>();
            builder.Services.AddScoped<IConsentRepository, ConsentRepository>();
            builder.Services.AddScoped<IContributionRepository, ContributionRepository>();
            builder.Services.AddScoped<IProfileLinkRepository, ProfileLinkRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IScoreCategoryRepository, ScoreCategoryRepository>();
            builder.Services.AddScoped<IScoreRepository, ScoreRepository>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<ITitleRepository, TitleRepository>();

            // Database
            FunctionsHostBuilderContext context = builder.GetContext();
            builder.Services.AddDbContextPool<Context>(options =>
                options.UseSqlServer(
                    context.Configuration.GetConnectionString("MvpSelectionsData"),
                    o => o.EnableRetryOnFailure().UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)));

            // HttpClients
            builder.Services.AddHttpClient<OktaClient>();
        }
    }
}
