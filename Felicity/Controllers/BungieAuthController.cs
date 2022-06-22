using System.Security.Claims;
using Felicity.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Felicity.Controllers;

[Route("api/auth")]
[ApiController]
public class BungieAuthController : ControllerBase
{
    public BungieAuthController()
    {
    }

    [HttpGet("bungie_net/{discordId}")]
    public async Task RedirectToBungieNet(ulong discordId)
    {
        await HttpContext.ChallengeAsync(
            "BungieNet",
            new AuthenticationProperties()
            {
                RedirectUri = $"api/auth/bungie_net/{discordId}/post_callback/"
            });
    }

    [HttpGet("bungie_net/{discordId}/post_callback")]
    public async Task<IActionResult> HandleAuthPostCallback(ulong discordId)
    {
        var claim = HttpContext.User.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        if (claim is null)
            return NotFound();
        
        var id = long.Parse(claim.Value);
        if (!BungieAuthCacheService.GetByIdAndRemove(id, out var context))
            return NotFound();
        
        var token = context.Token;
        // Here we save this token
        return Ok();

    }
}