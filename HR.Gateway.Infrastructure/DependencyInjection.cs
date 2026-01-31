using HR.Gateway.Infrastructure.Entities;
using HR.Gateway.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Application ports
using HR.Gateway.Application.Abstractions.Angajati;
using HR.Gateway.Application.Abstractions.MFiles;
using HR.Gateway.Application.Abstractions.Security;
using HR.Gateway.Application.Abstractions.CerereConcediu;
using HR.Gateway.Application.Abstractions.CerereConcediuFaraPlata;
using HR.Gateway.Application.Abstractions.CerereConcediuLaEveniment;
using HR.Gateway.Application.Abstractions.CerereConcediuOdihna;
using HR.Gateway.Application.Abstractions.Concedii;

// Infrastructure implementations
using HR.Gateway.Infrastructure.Auth;
using HR.Gateway.Infrastructure.MFiles.Tokens;
using HR.Gateway.Infrastructure.Angajat.Client;
using HR.Gateway.Infrastructure.Angajat.Services;
using HR.Gateway.Infrastructure.CerereConcediuOdihna.Client;
using HR.Gateway.Infrastructure.CerereConcediuOdihna.Services;
using HR.Gateway.Infrastructure.Concediu.Services;
using HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Client;
using HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Services;
using HR.Gateway.Infrastructure.MFiles.Common;
using HR.Gateway.Infrastructure.Concediu;
using HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client;
using HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Services;
using HR.Gateway.Infrastructure.Concediu.Client;

namespace HR.Gateway.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // 1) DbContext (Postgres)
        var cs = config.GetConnectionString("Postgres")
                 ?? throw new InvalidOperationException("Missing ConnectionStrings:Postgres");
        services.AddDbContext<HRDbContext>(opt => opt.UseNpgsql(cs));

        // 2) Identity
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

        // 3) M-Files: alegem un singur "environment" activ (din config)
        services.AddSingleton(sp =>
        {
            var opts = new MFilesOptions();
            config.GetSection("MFiles").Bind(opts);

            MFilesEnvironment? env = null;

            if (opts.Environments.Count > 0)
            {
                var key = string.IsNullOrWhiteSpace(opts.ActiveEnvironment) ? "Dev" : opts.ActiveEnvironment;
                if (!opts.Environments.TryGetValue(key, out env))
                    throw new InvalidOperationException($"MFiles: active environment '{key}' not found in configuration.");
            }

            if (env is null)
            {
                var baseUrl  = config["MFiles:BaseUrl"];
                var username = config["MFiles:Username"];
                var password = config["MFiles:Password"];
                var vault    = config["MFiles:VaultGuid"];

                if (string.IsNullOrWhiteSpace(baseUrl))
                    throw new InvalidOperationException("MFiles:BaseUrl missing (and no 'Environments' configured).");

                env = new MFilesEnvironment
                {
                    BaseUrl = baseUrl.TrimEnd('/'),
                    Credentials = new MFilesCredentials
                    {
                        Username  = username ?? "",
                        Password  = password ?? "",
                        VaultGuid = vault    ?? ""
                    },
                    Metadata = new MFilesMetadataConfig()
                };
            }

            return env!;
        });

        // 4) Token provider M-Files + handler care pune X-Authentication
        services.AddScoped<IMFilesTokenProvider, MFilesTokenProvider>();
        services.AddTransient<MFilesAuthHandler>();

        // 5) HttpClient către VEM (apelează extension method-urile din M-Files) + X-Authentication
        services.AddHttpClient<IVemAngajatClient, VemAngajatClient>((sp, http) =>
        {
            var env = sp.GetRequiredService<MFilesEnvironment>();
            var baseUrl = env.BaseUrl.TrimEnd('/');

            // dacă cineva a configurat deja "/REST" în BaseUrl, nu-l mai adăugăm
            if (baseUrl.EndsWith("/REST", StringComparison.OrdinalIgnoreCase))
                http.BaseAddress = new Uri(baseUrl + "/");
            else
                http.BaseAddress = new Uri(baseUrl + "/REST/");

            http.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddHttpMessageHandler<MFilesAuthHandler>();
        
        // 5a) HttpClient către VEM pentru concedii (reusează același auth handler)
        services.AddHttpClient<IVemConcediuService, VemConcediuService>((sp, http) =>
            {
                var env = sp.GetRequiredService<MFilesEnvironment>();
                var baseUrl = env.BaseUrl.TrimEnd('/');

                // asigurăm /REST/ la final (identic cu IVemAngajatClient)
                if (baseUrl.EndsWith("/REST", StringComparison.OrdinalIgnoreCase))
                    http.BaseAddress = new Uri(baseUrl + "/");
                else
                    http.BaseAddress = new Uri(baseUrl + "/REST/");

                http.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<MFilesAuthHandler>();
        
        // 5b) HttpClient către VEM pentru cereri concedii (reusează același auth handler)
        services.AddHttpClient<IVemCerereConcediuOdihnaService, VemCerereConcediuOdihnaService>((sp, http) =>
            {
                var env = sp.GetRequiredService<MFilesEnvironment>();
                var baseUrl = env.BaseUrl.TrimEnd('/');

                http.BaseAddress = baseUrl.EndsWith("/REST", StringComparison.OrdinalIgnoreCase)
                    ? new Uri(baseUrl + "/")
                    : new Uri(baseUrl + "/REST/");

                http.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<MFilesAuthHandler>();
        
        
        // 5c) HttpClient către VEM pentru cereri concedii fără plată (reusează același auth handler)
        services.AddHttpClient<IVemCerereConcediuFaraPlataService, VemCerereConcediuFaraPlataService>((sp, http) =>
            {
                var env = sp.GetRequiredService<MFilesEnvironment>();
                var baseUrl = env.BaseUrl.TrimEnd('/');

                http.BaseAddress = baseUrl.EndsWith("/REST", StringComparison.OrdinalIgnoreCase)
                    ? new Uri(baseUrl + "/")
                    : new Uri(baseUrl + "/REST/");

                http.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<MFilesAuthHandler>();

        // 5d) HttpClient către VEM pentru cereri concedii de eveniment (reusează același auth handler)
        services.AddHttpClient<IVemCerereConcediuLaEvenimentService, HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client.VemCerereConcediuLaEvenimentService>((sp, http) =>
            {
                var env = sp.GetRequiredService<MFilesEnvironment>();
                var baseUrl = env.BaseUrl.TrimEnd('/');

                http.BaseAddress = baseUrl.EndsWith("/REST", StringComparison.OrdinalIgnoreCase)
                    ? new Uri(baseUrl + "/")
                    : new Uri(baseUrl + "/REST/");

                http.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<MFilesAuthHandler>();
        
        // 6) Providerul din Infrastructure care implementează portul din Application
        services.AddScoped<IAngajatReader, AngajatReader>();
        
        // 6b) Portul din Application -> implementarea din Infrastructure
        services.AddScoped<IConcediuCalculator, ConcediuCalculator>();
        
        // 6c) Portul din Application -> implementarea din Infrastructure
        services.AddScoped<ICerereConcediuOdihnaWriter, CerereConcediuOdihnaWriter>();        
        
        // 6d) Portul din Application -> implementarea din Infrastructure
        services.AddHttpClient<ICerereConcediuOdihnaReader, CerereConcediuOdihnaReader>((sp, http) =>
            {
                var env = sp.GetRequiredService<MFilesEnvironment>();
                var baseUrl = env.BaseUrl.TrimEnd('/');

                http.BaseAddress = baseUrl.EndsWith("/REST", StringComparison.OrdinalIgnoreCase)
                    ? new Uri(baseUrl + "/")
                    : new Uri(baseUrl + "/REST/");

                http.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<MFilesAuthHandler>();
        
        // 6e) Inlocuitori (folosește tot M-Files REST + auth handler)
        services.AddHttpClient<IInlocuitoriProvider, InlocuitoriProvider>((sp, http) =>
            {
                var env = sp.GetRequiredService<MFilesEnvironment>();
                var baseUrl = env.BaseUrl.TrimEnd('/');

                http.BaseAddress = baseUrl.EndsWith("/REST", StringComparison.OrdinalIgnoreCase)
                    ? new Uri(baseUrl + "/")
                    : new Uri(baseUrl + "/REST/");

                http.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<MFilesAuthHandler>();

// 6f) Portul din Application -> implementarea din Infrastructure (Concediu fără plată)
        services.AddScoped<ICerereConcediuFaraPlataWriter, HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Services.CerereConcediuFaraPlataWriter>();

// 6g) Reader (folosește tot M-Files REST + auth handler) – necesar pentru download document
        services.AddHttpClient<ICerereConcediuFaraPlataReader, HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Services.CerereConcediuFaraPlataReader>((sp, http) =>
            {
                var env = sp.GetRequiredService<MFilesEnvironment>();
                var baseUrl = env.BaseUrl.TrimEnd('/');

                http.BaseAddress = baseUrl.EndsWith("/REST", StringComparison.OrdinalIgnoreCase)
                    ? new Uri(baseUrl + "/")
                    : new Uri(baseUrl + "/REST/");

                http.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<MFilesAuthHandler>();

        
// 6h) Concediu Eveniment
        services.AddScoped<ICerereConcediuLaEvenimentWriter, CerereConcediuLaEvenimentWriter>();

        services.AddHttpClient<ICerereConcediuLaEvenimentReader, CerereConcediuLaEvenimentReader>((sp, http) =>
            {
                var env = sp.GetRequiredService<MFilesEnvironment>();
                var baseUrl = env.BaseUrl.TrimEnd('/');

                http.BaseAddress = baseUrl.EndsWith("/REST", StringComparison.OrdinalIgnoreCase)
                    ? new Uri(baseUrl + "/")
                    : new Uri(baseUrl + "/REST/");

                http.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<MFilesAuthHandler>();

        // 7) LDAP / AD
        services.AddScoped<IAdAuthService, LdapAdAuthService>();

        return services;
    }
}








