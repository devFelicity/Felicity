using DotNetBungieAPI.Models;
using Felicity.Models;

namespace Felicity.Util;

public static class MiscUtils
{
    public static double GetTimestamp(this DateTime dateTime)
    {
        return dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }

    public static DateTime TimeStampToDateTime(double unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }

    public static Server GetServer(ServerDb serverDb, ulong guildId)
    {
        var server = serverDb.Servers.FirstOrDefault(x => x.ServerId == guildId);
        if (server != null)
            return server;

        server = new Server
        {
            ServerId = guildId,
            BungieLocale = BungieLocales.EN
        };
        serverDb.Servers.Add(server);

        return server;
    }
}