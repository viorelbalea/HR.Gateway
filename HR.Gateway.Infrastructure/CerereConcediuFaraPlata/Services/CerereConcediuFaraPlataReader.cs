using System.Net;
using System.Text.Json;
using HR.Gateway.Application.Abstractions.CerereConcediuFaraPlata;
using HR.Gateway.Application.Models.CerereConcediu;
using AppModel = HR.Gateway.Application.Models.CerereConcediuFaraPlata;
using HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Client;
using ClientDto = HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Client.Dtos;

namespace HR.Gateway.Infrastructure.CerereConcediuFaraPlata.Services;

internal sealed class CerereConcediuFaraPlataReader : ICerereConcediuFaraPlataReader
{
    private readonly HttpClient _http;
    private readonly IVemCerereConcediuFaraPlataService _vem;

    private static readonly JsonSerializerOptions JsonOptions =
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    public CerereConcediuFaraPlataReader(HttpClient http, IVemCerereConcediuFaraPlataService vem)
    {
        _http = http;
        _vem = vem;
    }

    public async Task<AppModel.CerereConcediuFaraPlataGetByIdResponse?> GetByIdAsync(int cerereId, CancellationToken ct = default)
    {
        var req = new ClientDto.CerereConcediuFaraPlataGetByIdRequest { CerereConcediuFaraPlataId = cerereId };
        var vemResp = await _vem.GetByIdAsync(req, ct);

        if (vemResp == null) return null;
        if (!vemResp.Succes) return null; // sau arunci ex, dupa preferin?a ta

        return new AppModel.CerereConcediuFaraPlataGetByIdResponse
        {
            Id               = vemResp.Id,
            DataCreare       = vemResp.DataCreare,
            DataInceput      = vemResp.DataInceput,
            DataSfarsit      = vemResp.DataSfarsit,
            NumarZile        = vemResp.NumarZile,
            Stare            = vemResp.Stare,
            Inlocuitor       = vemResp.Inlocuitor,
            NumarInregistrare = vemResp.NumarInregistrare,
            DataInregistrare = vemResp.DataInregistrare,
            Motiv            = vemResp.Motiv
        };
    }

    public async Task<CerereConcediuGetDocumentResponse?> GetDocumentAsync(int mfilesObjectId, CancellationToken ct = default)
    {
        // identic cu odihna
        var listUrl = $"objects/0/{mfilesObjectId}/latest/files";

        using var listResp = await _http.GetAsync(listUrl, ct);
        if (listResp.StatusCode == HttpStatusCode.NotFound) return null;
        listResp.EnsureSuccessStatusCode();

        var jsonStr = await listResp.Content.ReadAsStringAsync(ct);
        var files = JsonSerializer.Deserialize<List<ClientDto.CerereConcediuFaraPlataGetDocumentFileInfo>>(jsonStr, JsonOptions) ?? new();

        if (files.Count == 0) return null;

        var file = files[0];
        var contentUrl = $"objects/0/{mfilesObjectId}/latest/files/{file.Id}/content";

        using var resp = await _http.GetAsync(contentUrl, HttpCompletionOption.ResponseHeadersRead, ct);
        if (resp.StatusCode == HttpStatusCode.NotFound) return null;
        resp.EnsureSuccessStatusCode();

        var ms = new MemoryStream();
        await (await resp.Content.ReadAsStreamAsync(ct)).CopyToAsync(ms, ct);
        ms.Position = 0;

        var fileName = (resp.Content.Headers.ContentDisposition?.FileNameStar
                     ?? resp.Content.Headers.ContentDisposition?.FileName
                     ?? "document.bin").Trim('"');

        return new CerereConcediuGetDocumentResponse
        {
            Content = ms,
            ContentType = resp.Content.Headers.ContentType?.MediaType ?? "application/octet-stream",
            FileName = fileName
        };
    }
}
