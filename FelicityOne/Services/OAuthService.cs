using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using BungieSharper.Entities;
using BungieSharper.Entities.Destiny;
using Ceen;
using Ceen.Httpd;
using Discord;
using Discord.WebSocket;
using FelicityOne.Configs;
using FelicityOne.Helpers;
using Log = Serilog.Log;
using ServerConfig = Ceen.Httpd.ServerConfig;

namespace FelicityOne.Services;

internal static class OAuthService
{
    private static DiscordSocketClient DiscordClient = null!;

    public static async Task Start(DiscordSocketClient discordClient)
    {
        DiscordClient = discordClient;

        var tcs = new CancellationTokenSource();
        var config = new ServerConfig()
            .AddRoute("/authorize", new AuthorizationHandler());

        config.SSLCertificate = new X509Certificate2("Data/certificate.pfx", ConfigService.GetBotSettings().PfxSecret);

        var task = HttpServer.ListenAsync(
            new IPEndPoint(IPAddress.Any, 8000),
            true,
            config,
            tcs.Token
        );

        await Task.Delay(-1, tcs.Token);

        tcs.Cancel();
        await task.WaitAsync(tcs.Token);
    }

    public static OAuthConfig CreateUser(ulong discordId)
    {
        var newConfig = new OAuthConfig
        {
            DiscordId = discordId,
            State = ChecksumHelper.Base64Encode($"{discordId}:{DateTime.Now:T}")
        };

        File.WriteAllText($"Users/{discordId}.json", ConfigHelper.ToJson(newConfig));

        return newConfig;
    }

    public static async Task<OAuthConfig> GetUser(ulong discordId)
    {
        var path = $"Users/{discordId}.json";
        var discordUser = DiscordClient.GetUser(discordId);

        var oauthValues = ConfigService.GetUserSettings(discordId);

        if (oauthValues == null)
            return null!;

        if (oauthValues.RefreshExpiresAt < DateTime.Now)
        {
            try
            {
                await discordUser.CreateDMChannelAsync().Result.SendMessageAsync(
                    "Your login has expired and needs to be renewed. If you wish to continue using our features, please use /register again.");
                File.Delete($"Users/{discordId}.json");
            }
            catch (Exception ex)
            {
                ex.Data.Add("action", "OAuth expired");
                ex.Data.Add("target", $"{discordUser} ({discordId})");
                Log.Error(ex, "Failed to DM");
            }

            return null!;
        }

        if (oauthValues.ExpiresAt >= DateTime.Now)
            return ConfigHelper.FromJson<OAuthConfig>(await File.ReadAllTextAsync(path))!;

        var refreshedUser = await BungieAPI.GetApiClient().OAuth.RefreshOAuthToken(oauthValues.RefreshToken);

        UpdateUser(Convert.ToUInt64(discordId), refreshedUser);

        Log.Information($"Refreshed OAuth token for {Format.Code(discordUser.ToString())}");

        return ConfigHelper.FromJson<OAuthConfig>(await File.ReadAllTextAsync(path))!;
    }

    public static void UpdateUser(ulong discordId, TokenResponse oauthResponse, long destinyMembershipId = 0,
        BungieMembershipType destinyMembershipType = BungieMembershipType.None, List<long> destinyCharacterIDs = null!)
    {
        var path = $"Users/{discordId}.json";

        var userConfig = ConfigHelper.FromJson<OAuthConfig>(File.ReadAllText(path));

        if (userConfig == null) return;

        userConfig.TokenType = oauthResponse.TokenType!;
        userConfig.AccessToken = oauthResponse.AccessToken!;
        if (oauthResponse.ExpiresIn != null)
            userConfig.ExpiresAt = DateTime.Now.AddSeconds((double) oauthResponse.ExpiresIn);
        if (oauthResponse.RefreshExpiresIn != null)
            userConfig.RefreshExpiresAt = DateTime.Now.AddSeconds((double) oauthResponse.RefreshExpiresIn);
        userConfig.RefreshToken = oauthResponse.RefreshToken!;
        userConfig.MembershipId = Convert.ToInt64(oauthResponse.MembershipId);

        if (destinyMembershipId != 0)
        {
            var userInfo = BungieAPI.GetApiClient().Api
                .Destiny2_GetProfile(destinyMembershipId, destinyMembershipType, new[]
                {
                    DestinyComponentType.Profiles
                }).Result.Profile.Data.UserInfo;

            userConfig.DestinyMembership = new DestinyMembership
            {
                BungieName = userInfo.BungieGlobalDisplayName + "#" + userInfo.BungieGlobalDisplayNameCode,
                CharacterIds = destinyCharacterIDs.ToArray(),
                MembershipId = destinyMembershipId,
                MembershipType = destinyMembershipType
            };
        }

        File.WriteAllText(path, ConfigHelper.ToJson(userConfig));
    }

    public static UserLinkStatus UserIsLinked(ulong discordId)
    {
        var path = $"Users/{discordId}.json";
        if (!File.Exists(path))
            return UserLinkStatus.NotRegistered;

        var user = ConfigHelper.FromJson<OAuthConfig>(File.ReadAllText(path));

        return string.IsNullOrEmpty(user?.AccessToken) ? UserLinkStatus.Incomplete : UserLinkStatus.Registered;
    }

    public static async Task PopulateDestinyMembership(string discordId, OAuthConfig currentUser)
    {
        if (currentUser.DestinyMembership != null!)
            if (currentUser.DestinyMembership.MembershipId == 0)
                return;
            
        discordId = Regex.Match(discordId, @"\d+").Value;

        try
        {
            var linkedProfile = Extensions.GetLatestProfile(currentUser.MembershipId, BungieMembershipType.BungieNext,
                currentUser.AccessToken);
                
            var destinyMembershipId = linkedProfile.MembershipId;
            var destinyMembershipType = linkedProfile.MembershipType;
            var destinyCharacterIDs = new List<long>();

            var profile = BungieAPI.GetApiClient().Api.Destiny2_GetProfile(destinyMembershipId, destinyMembershipType,
                new[]
                {
                    DestinyComponentType.Characters
                }, currentUser.AccessToken).Result;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var (key, _) in profile.Characters.Data)
                destinyCharacterIDs.Add(key);

            var bungieTag =
                $"{linkedProfile.BungieGlobalDisplayName}#{linkedProfile.BungieGlobalDisplayNameCode}";
            LogService.SendLogDiscord(
                $"Registered `{DiscordClient.GetUserAsync(Convert.ToUInt64(discordId)).Result}` to {bungieTag}.");

            var oAuthResponse = new TokenResponse
            {
                AccessToken = currentUser.AccessToken, ExpiresIn = 3500,
                MembershipId = currentUser.MembershipId, RefreshExpiresIn = 7775990,
                RefreshToken = currentUser.RefreshToken, TokenType = currentUser.TokenType
            };

            UpdateUser(Convert.ToUInt64(discordId), oAuthResponse, destinyMembershipId, destinyMembershipType,
                destinyCharacterIDs);

            var dmChannel = await DiscordClient.GetUser(Convert.ToUInt64(discordId))
                .CreateDMChannelAsync();

            await dmChannel.SendMessageAsync(
                $"You successfully linked your profile to Felicity with the Bungie Name: **{bungieTag}**\n" +
                "If this information is incorrect, please contact a staff member.");
        }
        catch (Exception ex)
        {
            ex.Data.Add("action", "OAuth populate");
            ex.Data.Add("target", discordId);
            Log.Error(ex, "Failed to register");
        }
    }

    internal enum UserLinkStatus
    {
        NotRegistered,
        Incomplete,
        Registered
    }
}

public class AuthorizationHandler : IHttpModule
{
    public async Task<bool> HandleAsync(IHttpContext context)
    {
        var data = context.Request.QueryString.ToList();

        var code = data.FirstOrDefault(pair => pair.Key == "code").Value;
        var state = data.FirstOrDefault(pair => pair.Key == "state").Value;
        var discordId = ChecksumHelper.Base64Decode(state).Split(":").First();

        Console.WriteLine($"Received OAuth request for user id: {discordId}");

        context.Response.SetNonCacheable();

        if (!File.Exists($"Users/{discordId}.json"))
        {
            await context.Response.WriteAllAsync("Invalid user.");
            return false;
        }

        var response = BungieAPI.GetApiClient().OAuth.GetOAuthToken(code).Result;

        if (string.IsNullOrEmpty(response.ErrorDescription))
        {
            await context.Response.WriteAllAsync("Registration successful, you may now close this window.");
            OAuthService.UpdateUser(Convert.ToUInt64(discordId), response);
        }
        else
        {
            await context.Response.WriteAllAsync(response.ErrorDescription);
            return false;
        }

        return true;
    }
}