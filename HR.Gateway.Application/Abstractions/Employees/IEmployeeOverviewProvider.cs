using HR.Gateway.Application.Employees.Models;

namespace HR.Gateway.Application.Abstractions.Employees;

public interface IEmployeeOverviewProvider
{
    Task<EmployeeOverview> GetOverviewAsync(string email, CancellationToken ct);
}