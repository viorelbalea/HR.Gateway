using Microsoft.AspNetCore.Identity;

namespace HR.Gateway.Infrastructure.Entities;

public class AppUser : IdentityUser<Guid> { }
public class AppRole : IdentityRole<Guid> { }