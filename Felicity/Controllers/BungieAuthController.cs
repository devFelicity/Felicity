using System.Security.Claims;
using Felicity.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Felicity.Controllers;

[Route("auth")]
[ApiController]
public class BungieAuthController : ControllerBase
{
    // ReSharper disable once EmptyConstructor
    public BungieAuthController()
    {
    }

    [HttpGet("bungie_net/{discordId}")]
    public async Task RedirectToBungieNet(ulong discordId)
    {
        await HttpContext.ChallengeAsync(
            "BungieNet",
            new AuthenticationProperties
            {
                RedirectUri = $"auth/bungie_net/{discordId}/post_callback/"
            });
    }

    [HttpGet("bungie_net/{discordId}/post_callback")]
    public Task<IActionResult> HandleAuthPostCallback(ulong discordId)
    {
        var claim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        if (claim is null)
            return Task.FromResult<IActionResult>(RedirectPermanent("https://tryfelicity.one/auth_failed"));
        
        var id = long.Parse(claim.Value);
        if (!BungieAuthCacheService.GetByIdAndRemove(id, out var context))
            return Task.FromResult<IActionResult>(RedirectPermanent("https://tryfelicity.one/auth_failed"));
        
        var token = context.Token;
        // Here we save this token
        Console.WriteLine(token);
        Console.WriteLine(discordId);
        return Task.FromResult<IActionResult>(RedirectPermanent("https://tryfelicity.one/auth_success"));
    }
}