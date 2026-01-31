using HR.Gateway.Application.Abstractions.CerereConcediuFaraPlata;
using AppModel = HR.Gateway.Application.Models.CerereConcediuFaraPlata;
using HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Client;
using ClientDto = HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Client.Dtos;
using Microsoft.Extensions.Logging;

namespace HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Services;

internal sealed class CerereConcediuFaraPlataWriter : ICerereConcediuFaraPlataWriter
{
    private readonly IVemCerereConcediuFaraPlataService _vem;
    private readonly ILogger<CerereConcediuFaraPlataWriter> _log;

    public CerereConcediuFaraPlataWriter(
        IVemCerereConcediuFaraPlataService vem,
        ILogger<CerereConcediuFaraPlataWriter> log)
    {
        _vem = vem;
        _log = log;
    }

    public async Task<int> CreeazaCerereAsync(AppModel.CerereConcediuFaraPlataCreateRequest req, CancellationToken ct)
    {
        _log.LogInformation("VEM Create Unpaid: {Email} {Start:d}..{End:d} (inlocuitor={Inloc})",
            req.Email, req.DataInceput, req.DataSfarsit, req.EmailInlocuitor);

        var resp = await _vem.CreateAsync(new ClientDto.CerereConcediuFaraPlataCreateRequest
        {
            Email = req.Email,
            DataInceput = req.DataInceput,
            DataSfarsit = req.DataSfarsit,
            EmailInlocuitor = req.EmailInlocuitor,
            NumarZileCalculate = req.NumarZileCalculate,
            Motiv = req.Motiv
        }, ct);

        _log.LogInformation("VEM Create Unpaid => Success={Success} Id={Id} Message={Msg}",
            resp.Succes, resp.CerereConcediuFaraPlataId, resp.Mesaj);

        if (!resp.Succes || resp.CerereConcediuFaraPlataId <= 0)
            throw new InvalidOperationException($"VEM.CreateUnpaid a e?uat: {resp.Mesaj ?? "fara mesaj"}");

        return resp.CerereConcediuFaraPlataId;
    }

    public async Task ActualizeazaCerereAsync(AppModel.CerereConcediuFaraPlataUpdateRequest req, CancellationToken ct = default)
    {
        _log.LogInformation("Update unpaid cerere {Id}: {Start:d}..{End:d} (inlocuitor={Inloc})",
            req.CerereId, req.DataInceput, req.DataSfarsit, req.EmailInlocuitor);

        var vemReq = new ClientDto.CerereConcediuFaraPlataUpdateRequest
        {
            CerereConcediuFaraPlataId = req.CerereId,
            Email = req.Email,
            DataInceput = req.DataInceput,
            DataSfarsit = req.DataSfarsit,
            EmailInlocuitor = req.EmailInlocuitor,
            Motiv = req.Motiv,
            NumarZileCalculate = req.NumarZileCalculate
        };

        var resp = await _vem.UpdateAsync(vemReq, ct);

        if (!resp.Succes)
            throw new InvalidOperationException(
                $"VEM.UpdateUnpaid a e?uat pentru cererea {req.CerereId}: {resp.Mesaj}");
    }

    public async Task InregistreazaCerereAsync(int cerereId, CancellationToken ct = default)
    {
        _log.LogInformation("�nregistrez cererea fara plata {Id}.", cerereId);

        var resp = await _vem.RegisterAsync(new ClientDto.CerereConcediuFaraPlataRegisterRequest
        {
            CerereConcediuFaraPlataId = cerereId
        }, ct);

        if (!resp.Succes)
            throw new InvalidOperationException(
                $"VEM RegisterUnpaidLeaveRequest a e?uat pentru cererea {cerereId}: {resp.Mesaj}");
    }

    public async Task TrimiteInSemnareElectronicaAsync(int cerereId, CancellationToken ct = default)
    {
        _log.LogInformation("Trimit cererea fara plata {Id} in semnare electronica.", cerereId);

        var resp = await _vem.SendToEsignAsync(new ClientDto.CerereConcediuFaraPlataSendToEsignRequest
        {
            CerereConcediuFaraPlataId = cerereId
        }, ct);

        if (!resp.Succes)
            throw new InvalidOperationException(
                $"VEM SendToEsign a esuat pentru cererea {cerereId}: {resp.Mesaj}");
    }

    public async Task IncarcaDocumentSemnatAsync(int cerereId, string fileName, Stream content, CancellationToken ct = default)
    {
        _log.LogInformation("Incarc documentul semnat pentru cererea fara plata {Id}.", cerereId);

        using var ms = new MemoryStream();
        await content.CopyToAsync(ms, ct);
        var b64 = Convert.ToBase64String(ms.ToArray());

        var resp = await _vem.UploadSignedAsync(new ClientDto.CerereConcediuFaraPlataUploadSignedRequest
        {
            CerereConcediuFaraPlataId = cerereId,
            NumeFisier = fileName,
            ContinutFisierBase64 = b64
        }, ct);

        if (!resp.Succes)
            throw new InvalidOperationException(
                resp.Mesaj ?? "Documentul nu a fost acceptat ca semnat electronic.");
    }

    public async Task TrimiteInAprobareAvizareAsync(int cerereId, CancellationToken ct = default)
    {
        _log.LogInformation("Trimit cererea fara plata {Id} in aprobare/avizare.", cerereId);

        var resp = await _vem.SendForApprovalAsync(new ClientDto.CerereConcediuFaraPlataSendForApprovalRequest
        {
            CerereConcediuFaraPlataId = cerereId
        }, ct);

        if (!resp.Succes)
            throw new InvalidOperationException(
                $"VEM SendForApproval a esuat pentru cererea {cerereId}: {resp.Mesaj}");
    }
}











