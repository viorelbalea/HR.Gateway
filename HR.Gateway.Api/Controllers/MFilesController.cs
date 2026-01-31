using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

using HR.Gateway.Application.Abstractions.MFiles;

namespace HR.Gateway.Api.Controllers;

[ApiController]
[Route("api/mfiles")]
// [Authorize]
public class MFilesController(IMFilesClient mf, IMFilesTokenProvider _tokens) : ControllerBase
{
    [HttpGet("ping")]
    public async Task<IActionResult> Ping(CancellationToken ct)
        => Ok(new { ok = true, server = JsonDocument.Parse(await mf.PingAsync(ct)).RootElement });


    [Authorize(Roles = "admin")]
    [HttpGet("token-dev")]
    public async Task<IActionResult> TokenDev(CancellationToken ct)
        => Ok(new { value = await _tokens.GetTokenAsync(ct) });

}