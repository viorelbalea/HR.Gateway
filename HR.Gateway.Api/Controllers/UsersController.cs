using HR.Gateway.Api.Contracts;
using HR.Gateway.Application.Abstractions.Employees;
using HR.Gateway.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HR.Gateway.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "admin")]
public class UsersController(UserManager<AppUser> users, RoleManager<AppRole> roles) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        var list = users.Users.Select(u => new { u.Id, u.Email, u.UserName }).ToList();
        return Ok(list);
    }

    [HttpPost("{id:guid}/roles/{role}")]
    public async Task<IActionResult> AddRole(Guid id, string role)
    {
        var user = await users.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();

        if (!await roles.RoleExistsAsync(role))
            await roles.CreateAsync(new AppRole { Id = Guid.NewGuid(), Name = role });

        var res = await users.AddToRoleAsync(user, role);
        if (!res.Succeeded) return Problem(string.Join("; ", res.Errors.Select(e => e.Description)));
        
        return Ok(new { user.Id, role });
    }
    
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await users.GetUserAsync(User);
        if (user is null) return Unauthorized();
        var roles = await users.GetRolesAsync(user);
        return Ok(new { user.Id, user.Email, user.UserName, roles });
    }
    
    [Authorize]
    [HttpGet("api/users/me/overview")]
    public async Task<ActionResult<UserOverviewDto>> Overview(
        [FromServices] IEmployeeOverviewProvider provider, CancellationToken ct)
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

        var app = await provider.GetOverviewAsync(email, ct);

        var dto = new UserOverviewDto
        {
            Profile = new UserProfileDto
            {
                Email = app.Profile.Email,
                FullName = app.Profile.FullName,
                FirstName = app.Profile.FirstName,
                LastName = app.Profile.LastName,
                Department = app.Profile.Department,
                Position = app.Profile.Position
            },
            Leave = new LeaveOverviewDto
            {
                TotalAvailable = app.TotalAvailable,
                PerYear = app.PerYear
                    .Select(p => new LeavePerYearDto { Year = p.Year, Available = p.Available })
                    .ToList(),
                PerYearDetailed = app.PerYearDetailed?
                    .Select(p => new LeavePerYearDetailedDto
                    {
                        An = p.Year,
                        AnId = p.YearId,
                        NumarZileAlocate = p.Allocated,
                        NumarZileConsumate = p.Used,
                        NumarZileRamase = p.Available
                    })
                    .ToList()
            }
        };

        return Ok(dto);
    }

}

