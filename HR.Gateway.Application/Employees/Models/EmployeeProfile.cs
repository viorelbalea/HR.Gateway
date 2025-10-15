namespace HR.Gateway.Application.Employees.Models;

public sealed class EmployeeProfile
{
    public required string Email { get; init; }
    public required string FullName { get; init; }
    public string? FirstName { get; set; }  // Prenume
    public string? LastName  { get; set; }  // Nume
    public string? Department { get; init; }
    public string? Position { get; init; }
}