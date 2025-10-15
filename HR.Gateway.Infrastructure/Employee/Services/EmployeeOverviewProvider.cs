using HR.Gateway.Application.Abstractions.Employees;
using HR.Gateway.Application.Employees.Models;
using HR.Gateway.Infrastructure.Employee.Client;

namespace HR.Gateway.Infrastructure.Employee.Services;

internal sealed class EmployeeOverviewProvider : IEmployeeOverviewProvider
{
    private readonly IVemEmployeeClient _client;
    public EmployeeOverviewProvider(IVemEmployeeClient client) => _client = client;

    public async Task<EmployeeOverview> GetOverviewAsync(string email, CancellationToken ct)
    {
        var r = await _client.GetByEmailAsync(email, ct)
                ?? throw new InvalidOperationException("VEM response null");
        var a = r.AngajatModel ?? throw new InvalidOperationException("VEM missing AngajatModel");

        var holidays = r.Holidays ?? new();

        var perYear = holidays
            .OrderByDescending(h => h.An)
            .Select(h => new LeavePerYear { Year = h.An, Available = h.NumarZileRamase })
            .ToList();

        var perYearDetailed = holidays
            .OrderByDescending(h => h.An)
            .Select(h => new LeavePerYearDetailed
            {
                Year = h.An,
                YearId = h.AnId,
                Allocated = h.NumarZileAlocate,
                Used = h.NumarZileConsumate,
                Available = h.NumarZileRamase
            })
            .ToList();

        var profile = new EmployeeProfile
        {
            Email = a.Email ?? email,
            FullName = a.NumeComplet ?? $"{a.Prenume} {a.Nume}".Trim(),
            FirstName  = a.Prenume,  
            LastName   = a.Nume,  
            Department = a.Departament,
            Position = a.Functie
        };

        var total = a.ZileConcediuRamase ?? perYear.Sum(x => x.Available);

        return new EmployeeOverview
        {
            Profile = profile,
            TotalAvailable = total,
            PerYear = perYear,
            PerYearDetailed = perYearDetailed
        };
    }

}