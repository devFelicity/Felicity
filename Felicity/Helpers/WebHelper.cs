using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using APIHelper;
using Ceen;
using Ceen.Httpd;
using RestSharp;

namespace Felicity.Helpers;

internal class WebHelper
{

    public static async Task Start()
    {
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
}

public class AuthorizationHandler : IHttpModule
{
    public async Task<bool> HandleAsync(IHttpContext context)
    {
        context.Response.SetNonCacheable();
        await context.Response.WriteAllAsync("You may now close this window.");
        var data = context.Request.QueryString.ToList();

        // TODO: add some state checking to prevent attacks
        var code = data.FirstOrDefault(pair => pair.Key == "code").Value;
        Console.WriteLine($"code: {code}");

        var client = new RestClient("https://www.bungie.net/");
        var request = new RestRequest("Platform/App/OAuth/Token/", Method.Post);
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

        // TODO: fill these from config
        request.AddHeader("Authorization", $"Basic {Hash.CalculateBase64($"{ConfigHelper.GetBotSettings().BungieClientId}:{ConfigHelper.GetBotSettings().BungieClientSecret}")}");
        request.AddParameter("grant_type", "authorization_code");
        request.AddParameter("code", code);

        var response = await client.ExecuteAsync(request);

        Console.WriteLine(response.Content);

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