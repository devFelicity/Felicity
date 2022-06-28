using System.ComponentModel.DataAnnotations;
using DotNetBungieAPI.Models;
using Microsoft.EntityFrameworkCore;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Felicity.Models;

public class User
{
    [Key]
    public ulong DiscordId { get; set; }
    public string OAuthToken { get; set; } = string.Empty;
    public DateTime OAuthTokenExpires { get; set; }
    public string OAuthRefreshToken { get; set; } = string.Empty;
    public DateTime OAuthRefreshExpires { get; set; }
    public long BungieMembershipId { get; set; }
    public string BungieName { get; set; } = string.Empty;
    public long DestinyMembershipId { get; set; }
    public BungieMembershipType DestinyMembershipType { get; set; }
}

public class UserDb : DbContext
{
    private readonly string _connectionString;

    public UserDb(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MySQLDb");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var serverVersion = new MariaDbServerVersion(new Version(10, 2, 21));
        optionsBuilder.UseMySql(_connectionString, serverVersion);
    }
        
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public DbSet<User> Users { get; set; } = null!;
}