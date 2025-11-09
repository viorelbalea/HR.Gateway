using System.Text.Json.Serialization;
using HR.Gateway.Api.Contracts.Angajati;
using HR.Gateway.Api.Contracts.Concedii.Common;

namespace HR.Gateway.Api.Contracts.Utilizatori;

public sealed class PrezentareUtilizatorDto
{
    [JsonPropertyName("profile")]
    public required ProfilAngajatDto Profil { get; init; }
    
    [JsonPropertyName("leave")]
    public required SituatieConcediiAngajatDto SituatieConcedii { get; init; }
}