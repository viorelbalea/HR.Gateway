using System.Net;
using System.Text.Json;
using HR.Gateway.Application.Abstractions.CerereConcediuLaEveniment;
using HR.Gateway.Application.Models.CerereConcediu;
using AppModel = HR.Gateway.Application.Models.CerereConcediuLaEveniment;
using HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client;
using ClientDto = HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Client.Dtos;

namespace HR.Gateway.Infrastructure.CerereConcediuLaEveniment.Services;

internal sealed class CerereConcediuLaEvenimentReader : ICerereConcediuLaEvenimentReader
{
    private readonly HttpClient _http;
    private readonly IVemCerereConcediuLaEvenimentService _vem;

    private static readonly JsonSerializerOptions JsonOptions =
        new() { PropertyNameCaseInsensitive = true };

    public CerereConcediuLaEvenimentReader(HttpClient http, IVemCerereConcediuLaEvenimentService vem)
    {
        _http = http;
        _vem = vem;
    }

    public async Task<AppModel.CerereConcediuLaEvenimentGetByIdResponse?> GetByIdAsync(int cerereId, CancellationToken ct)
    {
        var req = new ClientDto.CerereConcediuLaEvenimentGetByIdRequest { CerereConcediuEvenimentId = cerereId };
        var vemResp = await _vem.GetByIdAsync(req, ct);

        if (vemResp == null || !vemResp.Succes) return null;

        return new AppModel.CerereConcediuLaEvenimentGetByIdResponse
        {
            Id = vemResp.Id,
            DataCreare = vemResp.DataCreare,
            DataInceput = vemResp.DataInceput,
            DataSfarsit = vemResp.DataSfarsit,
            NumarZile = vemResp.NumarZile,
            Stare = vemResp.Stare,
            Inlocuitor = vemResp.Inlocuitor,
            NumarInregistrare = vemResp.NumarInregistrare,
            DataInregistrare = vemResp.DataInregistrare,
            Motiv = vemResp.Motiv
        };
    }

    public async Task<CerereConcediuGetDocumentResponse?> GetDocumentAsync(int mfilesObjectId, CancellationToken ct)
    {
        var listUrl = $"objects/0/{mfilesObjectId}/latest/files";

        using var listResp = await _http.GetAsync(listUrl, ct);
        if (listResp.StatusCode == HttpStatusCode.NotFound) return null;
        listResp.EnsureSuccessStatusCode();

        var jsonStr = await listResp.Content.ReadAsStringAsync(ct);
        var files = JsonSerializer.Deserialize<List<ClientDto.CerereConcediuLaEvenimentGetDocumentFileInfo>>(jsonStr, JsonOptions) ?? new();

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

