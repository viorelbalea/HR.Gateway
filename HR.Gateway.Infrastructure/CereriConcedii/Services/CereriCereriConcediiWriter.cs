// HR.Gateway.Infrastructure.CereriConcedii.Services/CereriConcediiWriter.cs
using HR.Gateway.Application.Abstractions.Concedii;
using HR.Gateway.Application.Models.CereriConcediu;
using HR.Gateway.Infrastructure.CereriConcedii.Client;
using HR.Gateway.Infrastructure.CereriConcedii.Client.Dtos;

namespace HR.Gateway.Infrastructure.CereriConcedii.Services;

internal sealed class CereriConcediiWriter : ICereriConcediiWriter
{
    private readonly IVemCereriConcediiService _vem;
    public CereriConcediiWriter(IVemCereriConcediiService vem) => _vem = vem;

    public async Task<int> CreeazaCerereAsync(CreareCerereConcediuOdihnaReq req, CancellationToken ct)
    {
        var resp = await _vem.CreateAsync(new CreateReq
        {
            Email              = req.Email,
            DataInceput        = req.DataInceput,
            DataSfarsit        = req.DataSfarsit,
            EmailInlocuitor    = req.EmailInlocuitor,
            NumarZileCalculate = req.NumarZileCalculate,
            AlocareManuala     = req.AlocareManuala
        }, ct);

        var id = resp.CerereConcediuOdihnaId.GetValueOrDefault();
        if (!resp.Success || id <= 0)
            throw new InvalidOperationException(resp.Message ?? "Crearea cererii a eșuat.");

        return id;
    }

    public async Task AlocaZileAsync(AlocareZileLaCerereConcediuOdihnaReq req, CancellationToken ct)
    {
        var allocReq = new AllocReq
        {
            CerereConcediuId = req.CerereConcediuId,
            Items = req.AlocariZileConcediu.Select(x => new AllocReq.AllocItem
            {
                ConcediuPerAngajatId = x.ConcediuPerAngajatId,
                NumarZile            = x.NumarZile
            }).ToList()
        };

        var resp = await _vem.AllocateAsync(allocReq, ct);
        if (!resp.Success)
            throw new InvalidOperationException(resp.Message ?? "Alocarea zilelor a eșuat.");
    }
}