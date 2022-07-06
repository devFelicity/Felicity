using System.Security.Claims;
using DotNetBungieAPI.Clients;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.User;
using Felicity.Models;
using Felicity.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable RouteTemplates.ActionRoutePrefixCanBeExtractedToControllerRoute

namespace Felicity.Controllers;

[Route("auth")]
[ApiController]
public class BungieAuthController : ControllerBase
{
    private readonly IBungieClient _bungieClient;
    private readonly UserDb _dbContext;

    public BungieAuthController(UserDb userDbContext, IBungieClient bungieApiClient)
    {
        _dbContext = userDbContext;
        _bungieClient = bungieApiClient;
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
    public async Task<IActionResult> HandleAuthPostCallback(ulong discordId)
    {
        var claim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        if (claim is null)
            return RedirectPermanent("https://tryfelicity.one/auth_failure");

        var id = long.Parse(claim.Value);
        if (!BungieAuthCacheService.GetByIdAndRemove(id, out var context))
            return RedirectPermanent("https://tryfelicity.one/auth_failure");

        var token = context.Token;

        var nowTime = DateTime.UtcNow;
        var baseTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day,
            nowTime.Hour, nowTime.Minute, nowTime.Second);

        var latestProfile = new DestinyProfileUserInfoCard();

        var user = _dbContext.Users.FirstOrDefault(x => x.DiscordId == discordId);
        var addUser = false;

        if (user == null)
        {
            addUser = true;

            user = new User
            {
                DiscordId = discordId
            };
        }

        user.BungieMembershipId = token.MembershipId;
        user.OAuthToken = token.AccessToken;
        user.OAuthRefreshToken = token.RefreshToken;
        user.OAuthTokenExpires = baseTime.AddSeconds(token.ExpiresIn);
        user.OAuthRefreshExpires = baseTime.AddSeconds(token.RefreshExpiresIn);

        var linkedProfiles =
            await _bungieClient.ApiAccess.Destiny2.GetLinkedProfiles(BungieMembershipType.BungieNext,
                user.BungieMembershipId, true);

        foreach (var potentialProfile in linkedProfiles.Response.Profiles)
            if (potentialProfile.DateLastPlayed > latestProfile.DateLastPlayed)
                latestProfile = potentialProfile;

        user.BungieName = latestProfile.BungieGlobalDisplayName + "#" + latestProfile.BungieGlobalDisplayNameCode;
        user.DestinyMembershipId = latestProfile.MembershipId;
        user.DestinyMembershipType = latestProfile.MembershipType;

        if (addUser)
            _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync();

        return RedirectPermanent("https://tryfelicity.one/auth_success");
    }
}