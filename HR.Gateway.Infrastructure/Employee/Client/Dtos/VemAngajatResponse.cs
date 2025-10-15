namespace HR.Gateway.Infrastructure.Employee.Client.Dtos;

public sealed class VemAngajatResponse
{
    public bool FoundSuccess { get; set; }
    public VemAngajatModel? AngajatModel { get; set; }
    public List<VemHoliday>? Holidays { get; set; }
}