using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using APIHelper;
using BungieSharper.Entities;
using BungieSharper.Entities.Destiny;
using Ceen;
using Ceen.Httpd;
using Discord;
using Discord.WebSocket;
using Felicity.Configs;
using Felicity.Helpers;
using RestSharp;
using ServerConfig = Ceen.Httpd.ServerConfig;

namespace Felicity.Services;

internal class OAuthService
{
    public static DiscordSocketClient DiscordClient;

    public static async Task Start(DiscordSocketClient discordClient)
    {
        DiscordClient = discordClient;

        var tcs = new CancellationTokenSource();
        var config = new ServerConfig()
            .AddRoute("/authorize", new AuthorizationHandler());

        config.SSLCertificate = new X509Certificate2("certificate.pfx", ConfigHelper.GetBotSettings().PfxSecret);

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
            State = Hash.Base64Encode($"{discordId}:{DateTime.Now:T}")
        };

        File.WriteAllText($"Users/{discordId}.json", OAuthConfig.ToJson(newConfig));

        return newConfig;
    }

    public static async Task<OAuthConfig> GetUser(ulong discordId)
    {
        var path = $"Users/{discordId}.json";
        var discordUser = DiscordClient.GetUser(discordId);

        var oauthValues = ConfigHelper.GetUserSettings(discordId);
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
                var msg = $"{ex.GetType()}: {ex.Message}";
                await Log.ErrorAsync(msg);
                LogHelper.LogToDiscord($"Failed to message {Format.Code($"{discordUser} ({discordId})")}:\n{msg}");
            }

            return null;
        }

        // ReSharper disable once InvertIf
        if (oauthValues.ExpiresAt < DateTime.Now)
        {
            var client = new RestClient("https://www.bungie.net/");
            var request = new RestRequest("Platform/App/OAuth/Token/", Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            request.AddHeader("Authorization",
                $"Basic {Hash.Base64Encode($"{ConfigHelper.GetBotSettings().BungieClientId}:{ConfigHelper.GetBotSettings().BungieClientSecret}")}");
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("refresh_token", oauthValues.RefreshToken);

            var response = await client.ExecuteAsync(request);
            var refreshedUser = OAuthResponse.FromJson(response.Content);
            UpdateUser(Convert.ToUInt64(discordId), refreshedUser);

            LogHelper.LogToDiscord($"Refreshed OAuth token for {Format.Code(discordUser.ToString())}");
        }

        return !File.Exists(path) ? null : OAuthConfig.FromJson(await File.ReadAllTextAsync(path));
    }

    public static void UpdateUser(ulong discordId, OAuthResponse oauthResponse, long destinyMembershipId = 0,
        BungieMembershipType destinyMembershipType = BungieMembershipType.None, List<long> destinyCharacterIDs = null)
    {
        var path = $"Users/{discordId}.json";

        var userConfig = OAuthConfig.FromJson(File.ReadAllText(path));

        userConfig.TokenType = oauthResponse.TokenType;
        userConfig.AccessToken = oauthResponse.AccessToken;
        userConfig.ExpiresAt = DateTime.Now.AddSeconds(oauthResponse.ExpiresIn);
        userConfig.RefreshExpiresAt = DateTime.Now.AddSeconds(oauthResponse.RefreshExpiresIn);
        userConfig.RefreshToken = oauthResponse.RefreshToken;
        userConfig.MembershipId = Convert.ToInt64(oauthResponse.MembershipId);

        if (destinyMembershipId != 0 && destinyCharacterIDs != null)
            userConfig.DestinyMembership = new DestinyMembership
            {
                CharacterIds = destinyCharacterIDs.ToArray(),
                MembershipId = destinyMembershipId,
                MembershipType = destinyMembershipType
            };

        File.WriteAllText(path, OAuthConfig.ToJson(userConfig));
    }

    public static UserLinkStatus UserIsLinked(ulong discordId)
    {
        var path = $"Users/{discordId}.json";
        if (!File.Exists(path))
            return UserLinkStatus.NotRegistered;

        var user = OAuthConfig.FromJson(File.ReadAllText(path));

        return string.IsNullOrEmpty(user.AccessToken) ? UserLinkStatus.Incomplete : UserLinkStatus.Registered;
    }

    internal enum UserLinkStatus
    {
        NotRegistered,
        Incomplete,
        Registered
    }

    public static async Task PopulateDestinyMembership(string discordId, OAuthConfig currentUser)
    {
        if (currentUser.DestinyMembership != null)
            return;

        discordId = Regex.Match(discordId, @"\d+").Value;

        try
        {
            var linkedProfiles = await APIService.GetApiClient().Api
                .Destiny2_GetLinkedProfiles(Convert.ToInt64(currentUser.MembershipId), BungieMembershipType.BungieNext,
                    authToken: currentUser.AccessToken);

            var destinyMembershipId = linkedProfiles.Profiles.First().MembershipId;
            var destinyMembershipType = linkedProfiles.Profiles.First().MembershipType;
            var destinyCharacterIDs = new List<long>();

            var profile = APIService.GetApiClient().Api.Destiny2_GetProfile(destinyMembershipId, destinyMembershipType,
                new[]
                {
                    DestinyComponentType.Characters
                }, currentUser.AccessToken).Result;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var (key, _) in profile.Characters.Data)
                destinyCharacterIDs.Add(key);

            var bungieTag =
                $"{linkedProfiles.BnetMembership.BungieGlobalDisplayName}#{linkedProfiles.BnetMembership.BungieGlobalDisplayNameCode}";
            LogHelper.LogToDiscord($"Registered `{discordId}` to {bungieTag}.");

            var oAuthResponse = new OAuthResponse
            {
                AccessToken = currentUser.AccessToken, ExpiresIn = 3500,
                MembershipId = currentUser.MembershipId.ToString(), RefreshExpiresIn = 7775990,
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
            var msg = $"{ex.GetType()}: {ex.Message}";
            await Log.ErrorAsync(msg);
            LogHelper.LogToDiscord($"Error registering user `{discordId}`\n" + Format.Code(msg));
        }
    }
}

public class AuthorizationHandler : IHttpModule
{
    public async Task<bool> HandleAsync(IHttpContext context)
    {
        var data = context.Request.QueryString.ToList();

        var code = data.FirstOrDefault(pair => pair.Key == "code").Value;
        var state = data.FirstOrDefault(pair => pair.Key == "state").Value;
        var discordId = Hash.Base64Decode(state).Split(":").First();

        Console.WriteLine($"Received OAuth request for user id: {discordId}");

        context.Response.SetNonCacheable();

        if (!File.Exists($"Users/{discordId}.json"))
        {
            await context.Response.WriteAllAsync("Invalid user.");
            return false;
        }

        var client = new RestClient("https://www.bungie.net/");
        var request = new RestRequest("Platform/App/OAuth/Token/", Method.Post);
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

        request.AddHeader("Authorization",
            $"Basic {Hash.Base64Encode($"{ConfigHelper.GetBotSettings().BungieClientId}:{ConfigHelper.GetBotSettings().BungieClientSecret}")}");
        request.AddParameter("grant_type", "authorization_code");
        request.AddParameter("code", code);

        var response = await client.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            await context.Response.WriteAllAsync("Registration successful, you may now close this window.");
            var newUser = OAuthResponse.FromJson(response.Content);
            OAuthService.UpdateUser(Convert.ToUInt64(discordId), newUser);
        }
        else
        {
            await context.Response.WriteAllAsync(response.Content);
            return false;
        }
        
        // try
        // {
        //     
        // }
        // catch (Exception ex)
        // {
        //     var msg = $"{ex.GetType()}: {ex.Message}";
        //     await Log.ErrorAsync(msg);
        //     LogHelper.LogToDiscord($"Error registering user `{discordId}`\n" + Format.Code(msg));
        // }

        return true;
    }
}