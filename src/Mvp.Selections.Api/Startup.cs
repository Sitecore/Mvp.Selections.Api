using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mvp.Selections.Api.Clients;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Helpers;
using Mvp.Selections.Api.Helpers.Interfaces;
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

            // Helpers
            builder.Services.AddSingleton<ISerializerHelper, JsonSerializerHelper>();

            // Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IRegionService, RegionService>();
            builder.Services.AddScoped<ICountryService, CountryService>();
            builder.Services.AddScoped<ISelectionService, SelectionService>();

            // Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IRegionRepository, RegionRepository>();
            builder.Services.AddScoped<ICountryRepository, CountryRepository>();
            builder.Services.AddScoped<ISelectionRepository, SelectionRepository>();

            // Database
            FunctionsHostBuilderContext context = builder.GetContext();
            builder.Services.AddDbContext<Context>(options =>
                options.UseSqlServer(context.Configuration.GetConnectionString("MvpSelectionsData")));

            // HttpClients
            builder.Services.AddHttpClient<OktaClient>();
        }
    }
}
