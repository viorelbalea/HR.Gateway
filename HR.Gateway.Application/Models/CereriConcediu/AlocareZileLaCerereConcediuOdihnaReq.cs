namespace HR.Gateway.Application.Models.CereriConcediu;

public class AlocareZileLaCerereConcediuOdihnaReq
{
    public required int CerereConcediuId { get; init; }
    public required IReadOnlyList<Item> AlocariZileConcediu { get; init; } = Array.Empty<Item>();

    public sealed class Item
    {
        public required int ConcediuPerAngajatId { get; init; }
        public required int NumarZile { get; init; }
    }
}