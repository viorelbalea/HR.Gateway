    // HR.Gateway.Infrastructure.CerereConcediuOdihna.Services/CerereConcediuOdihnaWriter.cs
    using HR.Gateway.Application.Abstractions.CerereConcediuOdihna;
    using AppModel = HR.Gateway.Application.Models.CerereConcediuOdihna;
    using HR.Gateway.Infrastructure.CerereConcediuOdihna.Client;
    using ClientDto = HR.Gateway.Infrastructure.CerereConcediuOdihna.Client.Dtos;
    using Microsoft.Extensions.Logging;

    namespace HR.Gateway.Infrastructure.CerereConcediuOdihna.Services;

    internal sealed class CerereConcediuOdihnaWriter : ICerereConcediuOdihnaWriter
    {
        private readonly IVemCerereConcediuOdihnaService _vem;
        private readonly ILogger<CerereConcediuOdihnaWriter> _log;
        
        public CerereConcediuOdihnaWriter(IVemCerereConcediuOdihnaService vem, ILogger<CerereConcediuOdihnaWriter> log)
        {
            _vem = vem;
            _log = log;
        }

        public async Task<int> CreeazaCerereAsync(AppModel.CerereConcediuOdihnaCreateRequest req, CancellationToken ct)
        {
            _log.LogInformation("VEM Create: {Email} {Start:d}..{End:d} (inlocuitor={Inloc})",
                req.Email, req.DataInceput, req.DataSfarsit, req.EmailInlocuitor);

                var resp = await _vem.CreateAsync(new ClientDto.CerereConcediuOdihnaCreateRequest
                {
                    Email              = req.Email,
                    DataInceput        = req.DataInceput,
                    DataSfarsit        = req.DataSfarsit,
                    EmailInlocuitor    = req.EmailInlocuitor,
                    NumarZileCalculate = req.NumarZileCalculate,
                    AlocareManuala     = req.AlocareManuala,
                    AlocariZileConcediu = req.AlocariZileConcediu
                        .Select(x => new ClientDto.CerereConcediuOdihnaAllocateDaysItem
                        {
                            ConcediuPerAngajatId = x.ConcediuPerAngajatId,
                            NumarZile            = x.NumarZile
                        })
                        .ToList()
                }, ct);

                _log.LogInformation("VEM Create => Success={Success} Id={Id} Message={Msg}",
                    resp.Succes, resp.CerereConcediuOdihnaId, resp.Mesaj);
                
                var id = resp.CerereConcediuOdihnaId.GetValueOrDefault();
                if (!resp.Succes || id <= 0)
                    throw new InvalidOperationException($"VEM.Create a e?uat: {resp.Mesaj ?? "fara mesaj"}");

                return id;
            
        }

        public async Task ActualizeazaCerereAsync(
            AppModel.CerereConcediuOdihnaUpdateRequest req,
            CancellationToken ct = default)
        {
            _log.LogInformation(
                "Update cerere {Id}: {Start:d}..{End:d} (inlocuitor={Inloc})",
                req.CerereId, req.DataInceput, req.DataSfarsit, req.EmailInlocuitor);

            var vemReq = new ClientDto.CerereConcediuOdihnaUpdateRequest
            {
                CerereConcediuOdihnaId = req.CerereId,
                DataInceput            = req.DataInceput,
                DataSfarsit            = req.DataSfarsit,
                EmailInlocuitor        = req.EmailInlocuitor,
                Motiv                  = req.Motiv,
                NumarZileCalculate     = req.NumarZileCalculate,
                AlocareManuala         = req.AlocareManuala,
                AlocariZileConcediu    = req.AlocariZileConcediu
                    .Select(x => new ClientDto.CerereConcediuOdihnaAllocateDaysItem
                    {
                        ConcediuPerAngajatId = x.ConcediuPerAngajatId,
                        NumarZile            = x.NumarZile
                    })
                    .ToList()
            };

            var resp = await _vem.UpdateAsync(vemReq, ct);

            if (!resp.Succes)
                throw new InvalidOperationException(
                    $"VEM.Update a e?uat pentru cererea {req.CerereId}: {resp.Mesaj}");
        }

        public async Task InregistreazaCerereAsync(
            int cerereId,
            CancellationToken ct = default)
        {
            _log.LogInformation("�nregistrez cererea {Id}.", cerereId);

            var req = new ClientDto.CerereConcediuOdihnaRegisterRequest()
            {
                CerereConcediuOdihnaId = cerereId
            };

            var resp = await _vem.RegisterAsync(req, ct);

            if (!resp.Succes)
                throw new InvalidOperationException(
                    $"VEM RegisterHolidayRequest a e?uat pentru cererea {cerereId}: {resp.Mesaj}");
        }
        
        public async Task TrimiteInSemnareElectronicaAsync(int cerereId, CancellationToken ct = default)
        {
            _log.LogInformation("Trimit cererea {Id} �n 'Semnare electronica'.", cerereId);

            var req = new ClientDto.CerereConcediuOdihnaSendToEsignRequest
            {
                CerereConcediuOdihnaId = cerereId
            };

            var resp = await _vem.SendToEsignAsync(req, ct);

            if (!resp.Succes)
                throw new InvalidOperationException(
                    $"VEM SendToEsign a e?uat pentru cererea {cerereId}: {resp.Mesaj}");
        }
        
        public async Task IncarcaDocumentSemnatAsync(
            int cerereId,
            string fileName,
            Stream content,
            CancellationToken ct = default)
        {
            _log.LogInformation("�ncarc documentul semnat pentru cererea {Id}.", cerereId);

            // citim �n memorie ca sa putem converti la base64
            using var ms = new MemoryStream();
            await content.CopyToAsync(ms, ct);
            var bytes = ms.ToArray();
            var b64   = Convert.ToBase64String(bytes);

            var req = new ClientDto.CerereConcediuOdihnaUploadSignedRequest()
            {
                CerereConcediuOdihnaId = cerereId,
                NumeFisier = fileName,
                ContinutFisierBase64 = b64
            };

            var resp = await _vem.UploadSignedAsync(req, ct);

            if (!resp.Succes)
            {
                _log.LogWarning("Upload semnat e?uat pentru cererea {Id}: {Msg}", cerereId, resp.Mesaj);
                throw new InvalidOperationException(
                    resp.Mesaj ?? "Documentul nu a fost acceptat ca semnat electronic.");
            }

            _log.LogInformation("Documentul semnat pentru cererea {Id} a fost acceptat.", cerereId);
        }
        
        public async Task TrimiteInAprobareAvizareAsync(
            int cerereId, 
            CancellationToken ct = default)
        {
            _log.LogInformation("Trimit cererea {Id} �n aprobare/avizare.", cerereId);

            var vemReq = new ClientDto.CerereConcediuOdihnaSendForApprovalRequest()
            {
                CerereConcediuOdihnaId = cerereId
            };

            var resp = await _vem.SendForApprovalAsync(vemReq, ct);

            if (!resp.Succes)
                throw new InvalidOperationException(
                    $"VEM SendForApproval a e?uat pentru cererea {cerereId}: {resp.Mesaj}");
        }
    }








