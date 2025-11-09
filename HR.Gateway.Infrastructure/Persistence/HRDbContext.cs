using HR.Gateway.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HR.Gateway.Infrastructure.Persistence;

public class HRDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public HRDbContext(DbContextOptions<HRDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

    }
}