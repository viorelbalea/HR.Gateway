using HR.Gateway.Infrastructure.Entities;
using HR.Gateway.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HR.Gateway.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // 1) DbContext (Npgsql)
        var cs = config.GetConnectionString("Postgres")
                 ?? throw new InvalidOperationException("Missing ConnectionStrings:Postgres");

        services.AddDbContext<HRDbContext>(opt =>
            opt.UseNpgsql(cs, npgsql =>
            {
                // dacă vrei să pui migrațiile într-un assembly separat:
                // npgsql.MigrationsAssembly(typeof(HRDbContext).Assembly.FullName);
            }));

        // 2) Identity Core (UserManager / RoleManager / Store-uri EF)
        services
            .AddIdentityCore<AppUser>(o =>
            {
                o.User.RequireUniqueEmail = true;
                // reguli de parolă mai relaxate pentru dev
                o.Password.RequiredLength = 6;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireDigit = false;
            })
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<HRDbContext>(); // util pt reset, 2FA etc.

        // (opțional) dacă folosești SignInManager pe viitor:
        // services.AddSignInManager<SignInManager<AppUser>>();

        return services;
    }
}