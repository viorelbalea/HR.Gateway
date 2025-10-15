using HR.Gateway.Infrastructure.Entities;
using HR.Gateway.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

// Application ports
using HR.Gateway.Application.Abstractions.Employees;
using HR.Gateway.Application.Abstractions.MFiles;
using HR.Gateway.Application.Abstractions.Security;

// Infrastructure implementations
using HR.Gateway.Infrastructure.Auth;
using HR.Gateway.Infrastructure.MFiles.Tokens;
using HR.Gateway.Infrastructure.Employee.Client;
using HR.Gateway.Infrastructure.Employee.Services;

namespace HR.Gateway.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // 1) DbContext (Postgres)
        var cs = config.GetConnectionString("Postgres")
                 ?? throw new InvalidOperationException("Missing ConnectionStrings:Postgres");
        services.AddDbContext<HRDbContext>(opt => opt.UseNpgsql(cs));

        // 2) Identity (UserManager<AppUser> etc.)
        services.AddIdentityCore<AppUser>(o =>
        {
            o.User.RequireUniqueEmail = true;
            o.Password.RequiredLength = 6;
            o.Password.RequireNonAlphanumeric = false;
            o.Password.RequireUppercase = false;
            o.Password.RequireDigit = false;
        })
        .AddRoles<AppRole>()
        .AddEntityFrameworkStores<HRDbContext>();

        // 3) M-Files options (suportă atât schema FLAT, cât și Environments)
        services.AddSingleton(sp =>
        {
            // 3a) Încearcă schema pe Environments (ce aveai în clase)
            var opts = new MFilesOptions();
            config.GetSection("MFiles").Bind(opts);

            MFilesEnvironment? env = null;

            if (opts.Environments.Count > 0)
            {
                var key = string.IsNullOrWhiteSpace(opts.ActiveEnvironment) ? "Dev" : opts.ActiveEnvironment;
                opts.Environments.TryGetValue(key, out env);
            }

            // 3b) Dacă nu e definit nimic pe Environments, folosește schema FLAT
            if (env is null)
            {
                var baseUrl  = config["MFiles:BaseUrl"];
                var username = config["MFiles:Username"];
                var password = config["MFiles:Password"];
                var vault    = config["MFiles:VaultGuid"];

                if (string.IsNullOrWhiteSpace(baseUrl))
                    throw new InvalidOperationException("MFiles:BaseUrl missing");

                env = new MFilesEnvironment
                {
                    BaseUrl = baseUrl.TrimEnd('/'),
                    Credentials = new MFilesCredentials
                    {
                        Username  = username ?? "",
                        Password  = password ?? "",
                        VaultGuid = vault ?? ""
                    },
                    Metadata = new MFilesMetadataConfig()
                };
            }

            return env!;
        });

        // 4) Token provider M-Files + handler care pune X-Authentication
        services.AddScoped<IMFilesTokenProvider, MFilesTokenProvider>(); // folosește implementarea ta; dacă nu ai, vezi exemplul minim de mai jos
        services.AddTransient<MFilesAuthHandler>();

        // 5) HttpClient către VEM (apelează extension method-urile din M-Files) + X-Authentication
        services.AddHttpClient<IVemEmployeeClient, VemEmployeeClient>((sp, http) =>
        {
            var env = sp.GetRequiredService<MFilesEnvironment>();

            // Dacă în config-ul tău ai "http://localhost/REST", normalizăm să avem mereu trailing slash
            var baseUrl = env.BaseUrl;
            http.BaseAddress = baseUrl.EndsWith("/REST", StringComparison.OrdinalIgnoreCase)
                ? new Uri(baseUrl + "/")
                : new Uri(baseUrl.TrimEnd('/') + "/REST/");

            http.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddHttpMessageHandler<MFilesAuthHandler>();

        // 6) Providerul din Infrastructure care implementează portul din Application
        services.AddScoped<IEmployeeOverviewProvider, EmployeeOverviewProvider>();

        // 7) LDAP / AD (dacă îl folosești ca port în Application)
        services.AddScoped<IAdAuthService, LdapAdAuthService>();

        return services;
    }
}
