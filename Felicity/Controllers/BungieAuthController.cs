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
    public async Task HandleAuthPostCallback(ulong discordId)
    {
        var authenticationService = HttpContext.RequestServices.GetRequiredService<IAuthenticationService>();
        var authResult = await authenticationService.AuthenticateAsync(HttpContext, "BungieNet");
    }
}