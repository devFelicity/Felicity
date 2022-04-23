using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using APIHelper;
using BungieSharper.Entities;
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
            State = Hash.Base64Encode($"{discordId}:{DateTime.Now:d}")
        };

        File.WriteAllText($"Users/{discordId}.json", OAuthConfig.ToJson(newConfig));

        return newConfig;
    }

    public static OAuthConfig GetUser(ulong discordId)
    {
        var path = $"Users/{discordId}.json";

        // TODO: add check here for oauth refresh requirement

        return !File.Exists(path) ? null : OAuthConfig.FromJson(File.ReadAllText(path));
    }

    public static void UpdateUser(ulong discordId, OAuthResponse oauthResponse)
    {
        var path = $"Users/{discordId}.json";

        var userConfig = OAuthConfig.FromJson(File.ReadAllText(path));

        userConfig.TokenType = oauthResponse.TokenType;
        userConfig.AccessToken = oauthResponse.AccessToken;
        userConfig.ExpiresAt = DateTime.Now.AddSeconds(oauthResponse.ExpiresIn);
        userConfig.RefreshExpiresAt = DateTime.Now.AddSeconds(oauthResponse.RefreshExpiresIn);
        userConfig.RefreshToken = oauthResponse.RefreshToken;
        userConfig.MembershipId = Convert.ToInt64(oauthResponse.MembershipId);

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

        var newUser = OAuthResponse.FromJson(response.Content);
        OAuthService.UpdateUser(Convert.ToUInt64(discordId), newUser);

        await context.Response.WriteAllAsync("Registration successful, you may now close this window.");

        var userCard = APIService.GetApiClient().Api
            .User_GetMembershipDataById(Convert.ToInt64(newUser.MembershipId),
                BungieMembershipType.BungieNext).Result.DestinyMemberships.First();

        var bungieTag = $"{userCard.BungieGlobalDisplayName}#{userCard.BungieGlobalDisplayNameCode}";
        LogHelper.LogToDiscord($"Registered `{discordId}` to {bungieTag}.");

        try
        {
            var dmChannel = await OAuthService.DiscordClient.GetUser(Convert.ToUInt64(discordId)).CreateDMChannelAsync();
            
            await dmChannel.SendMessageAsync(
                $"You successfully linked your profile to Felicity with the Bungie Name: **{bungieTag}**\n" +
                "If this information is incorrect, please contact a staff member.");
        }
        catch (Exception ex)
        {
            var msg = $"{ex.GetType()}: {ex.Message}";
            await Log.ErrorAsync(msg);
            LogHelper.LogToDiscord($"Error registering user `{discordId}`\n"+ Format.Code(msg));
        }

        return true;
    }

    // TODO: implement refresh token
    /*
       POST https://www.bungie.net/Platform/App/OAuth/Token/ HTTP/1.1
       Authorization: Basic {base64encoded(client-id:client-secret)}
       Content-Type: application/x-www-form-urlencoded
       
       grant_type=refresh_token&refresh_token={refresh-token}
     */
}