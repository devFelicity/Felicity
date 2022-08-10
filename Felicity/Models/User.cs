using System.ComponentModel.DataAnnotations;
using DotNetBungieAPI.Models;
using DotNetBungieAPI.Models.Authorization;
using DotNetBungieAPI.Service.Abstractions;
using Microsoft.EntityFrameworkCore;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Felicity.Models;

public class User
{
    [Key] public ulong DiscordId { get; set; }

    public string OAuthToken { get; set; } = string.Empty;
    public DateTime OAuthTokenExpires { get; set; }
    public string OAuthRefreshToken { get; set; } = string.Empty;
    public DateTime OAuthRefreshExpires { get; set; }
    public long BungieMembershipId { get; set; }
    public string BungieName { get; set; } = string.Empty;
    public long DestinyMembershipId { get; set; }
    public BungieMembershipType DestinyMembershipType { get; set; }
}

public static class UserExtensions
{
    public static async Task<User> RefreshToken(this User user, IBungieClient bungieClient, DateTime nowTime)
    {
        var refreshedUser = await bungieClient.Authorization.RenewToken(user.GetTokenData());

        user.OAuthToken = refreshedUser.AccessToken;
        user.OAuthTokenExpires = nowTime.AddSeconds(refreshedUser.ExpiresIn);
        user.OAuthRefreshToken = refreshedUser.RefreshToken;
        user.OAuthRefreshExpires = nowTime.AddSeconds(refreshedUser.RefreshExpiresIn);

        return user;
    }

    public static AuthorizationTokenData GetTokenData(this User user)
    {
        return new AuthorizationTokenData
        {
            AccessToken = user.OAuthToken,
            RefreshToken = user.OAuthRefreshToken,
            ExpiresIn = (int)(user.OAuthTokenExpires - DateTime.UtcNow).TotalSeconds,
            MembershipId = user.BungieMembershipId,
            RefreshExpiresIn = (int)(user.OAuthRefreshExpires - DateTime.UtcNow).TotalSeconds,
            TokenType = "Bearer"
        };
    }
}

public class UserDb : DbContext
{
    private readonly string _connectionString;

    public UserDb(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MySQLDb");
    }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var serverVersion = new MariaDbServerVersion(new Version(10, 2, 21));
        optionsBuilder.UseMySql(_connectionString, serverVersion, builder => builder.EnableRetryOnFailure());
    }
}