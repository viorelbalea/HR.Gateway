using HR.Gateway.Api.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace HR.Gateway.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhooksController : ControllerBase
{
    private readonly IHubContext<CereriConcediuHub> _hubContext;
    private readonly ILogger<WebhooksController> _log;

    public WebhooksController(
        IHubContext<CereriConcediuHub> hubContext,
        ILogger<WebhooksController> log)
    {
        _hubContext = hubContext;
        _log = log;
    }

    /// <summary>
    /// Webhook apelat de M-Files când se înregistrează o cerere
    /// </summary>
    [HttpPost("cerere-registered")]
    [AllowAnonymous] // TODO: Securizează cu API key sau IP whitelist
    public async Task<IActionResult> CerereRegisteredAsync([FromBody] CerereStatusChangedNotification notif, CancellationToken ct)
    {
        _log.LogInformation("Webhook: Cerere {Id} (tip: {Tip}) registered - status: {Status}",
            notif.CerereId, notif.TipCerere, notif.Status);

        // Trimite notificare la toți clienții conectați via SignalR
        await _hubContext.Clients.All.SendAsync("CerereStatusChanged", notif, ct);

        return Ok();
    }

    /// <summary>
    /// Webhook generic pentru orice schimbare de status
    /// </summary>
    [HttpPost("cerere-status-changed")]
    [AllowAnonymous] // TODO: Securizează cu API key sau IP whitelist
    public async Task<IActionResult> CerereStatusChangedAsync([FromBody] CerereStatusChangedNotification notif, CancellationToken ct)
    {
        _log.LogInformation("Webhook: Cerere {Id} (tip: {Tip}) status changed: {Status} - message: {Msg}",
            notif.CerereId, notif.TipCerere, notif.Status, notif.Message);

        // Trimite notificare la toți clienții conectați via SignalR
        await _hubContext.Clients.All.SendAsync("CerereStatusChanged", notif, ct);

        return Ok();
    }
}

/// <summary>
/// Model pentru notificările de status de la M-Files
/// </summary>
public record CerereStatusChangedNotification
{
    public int CerereId { get; init; }
    public string TipCerere { get; init; } = string.Empty; // "Odihna", "FaraPlata", "LaEveniment"
    public string Status { get; init; } = string.Empty; // "Registered", "InSemnare", "InAprobare", etc.
    public string? Message { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
