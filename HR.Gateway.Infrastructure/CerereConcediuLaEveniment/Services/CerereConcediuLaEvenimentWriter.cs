using HR.Gateway.Application.Abstractions.CerereConcediuLaEveniment;
using AppModel = HR.Gateway.Application.Models.CerereConcediuLaEveniment;
using HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client;
using ClientDto = HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client.Dtos;
using Microsoft.Extensions.Logging;

namespace HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Services;

internal sealed class CerereConcediuLaEvenimentWriter : ICerereConcediuLaEvenimentWriter
{
    private readonly IVemCerereConcediuLaEvenimentService _vem;
    private readonly ILogger<CerereConcediuLaEvenimentWriter> _log;

    public CerereConcediuLaEvenimentWriter(
        IVemCerereConcediuLaEvenimentService vem,
        ILogger<CerereConcediuLaEvenimentWriter> log)
    {
        _vem = vem;
        _log = log;
    }

    public async Task<int> CreeazaCerereAsync(AppModel.CerereConcediuLaEvenimentCreateRequest req, CancellationToken ct)
    {
        _log.LogInformation("VEM Create Special Event: {Email} {Start:d}..{End:d}",
            req.Email, req.DataInceput, req.DataSfarsit);

        var resp = await _vem.CreateAsync(new ClientDto.CerereConcediuLaEvenimentCreateRequest
        {
            Email = req.Email,
            DataInceput = req.DataInceput,
            DataSfarsit = req.DataSfarsit,
            EmailInlocuitor = req.EmailInlocuitor,
            NumarZileCalculate = req.NumarZileCalculate,
            Motiv = req.Motiv ?? string.Empty
        }, ct);

        if (!resp.Succes || resp.CerereConcediuEvenimentId <= 0)
            throw new InvalidOperationException($"VEM.CreateSpecialEvent a e?uat: {resp.Mesaj ?? "fara mesaj"}");

        return resp.CerereConcediuEvenimentId;
    }

    public async Task ActualizeazaCerereAsync(AppModel.CerereConcediuLaEvenimentUpdateRequest req, CancellationToken ct)
    {
        var vemReq = new ClientDto.CerereConcediuLaEvenimentUpdateRequest
        {
            CerereConcediuEvenimentId = req.CerereId,
            Email = req.Email,
            DataInceput = req.DataInceput,
            DataSfarsit = req.DataSfarsit,
            EmailInlocuitor = req.EmailInlocuitor,
            Motiv = req.Motiv ?? string.Empty,
            NumarZileCalculate = req.NumarZileCalculate
        };

        var resp = await _vem.UpdateAsync(vemReq, ct);

        if (!resp.Succes)
            throw new InvalidOperationException(
                $"VEM.UpdateSpecialEvent a e?uat pentru cererea {req.CerereId}: {resp.Mesaj}");
    }

    public async Task InregistreazaCerereAsync(int cerereId, CancellationToken ct)
    {
        var resp = await _vem.RegisterAsync(new ClientDto.CerereConcediuLaEvenimentRegisterRequest
        {
            CerereConcediuEvenimentId = cerereId
        }, ct);

        if (!resp.Succes)
            throw new InvalidOperationException(
                $"VEM RegisterSpecialEvent a e?uat pentru cererea {cerereId}: {resp.Mesaj}");
    }

    public async Task TrimiteInSemnareElectronicaAsync(int cerereId, CancellationToken ct = default)
    {
        _log.LogInformation("Trimit cererea de eveniment {Id} in semnare electronica.", cerereId);

        var resp = await _vem.SendToEsignAsync(new ClientDto.CerereConcediuLaEvenimentSendToEsignRequest
        {
            CerereConcediuEvenimentId = cerereId
        }, ct);

        if (!resp.Succes)
            throw new InvalidOperationException(
                $"VEM SendToEsign a esuat pentru cererea {cerereId}: {resp.Mesaj}");
    }

    public async Task IncarcaDocumentSemnatAsync(int cerereId, string fileName, Stream content, CancellationToken ct = default)
    {
        _log.LogInformation("Incarc documentul semnat pentru cererea de eveniment {Id}.", cerereId);

        using var ms = new MemoryStream();
        await content.CopyToAsync(ms, ct);
        var b64 = Convert.ToBase64String(ms.ToArray());

        var resp = await _vem.UploadSignedAsync(new ClientDto.CerereConcediuLaEvenimentUploadSignedRequest
        {
            CerereConcediuEvenimentId = cerereId,
            NumeFisier = fileName,
            ContinutFisierBase64 = b64
        }, ct);

        if (!resp.Succes)
            throw new InvalidOperationException(
                resp.Mesaj ?? "Documentul nu a fost acceptat ca semnat electronic.");
    }

    public async Task TrimiteInAprobareAvizareAsync(int cerereId, CancellationToken ct = default)
    {
        _log.LogInformation("Trimit cererea de eveniment {Id} in aprobare/avizare.", cerereId);

        var resp = await _vem.SendForApprovalAsync(new ClientDto.CerereConcediuLaEvenimentSendForApprovalRequest
        {
            CerereConcediuEvenimentId = cerereId
        }, ct);

        if (!resp.Succes)
            throw new InvalidOperationException(
                $"VEM SendForApproval a esuat pentru cererea {cerereId}: {resp.Mesaj}");
    }
}











