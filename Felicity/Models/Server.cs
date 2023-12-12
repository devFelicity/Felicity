using System.ComponentModel.DataAnnotations;
using DotNetBungieAPI.Models;
using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedMember.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Felicity.Models;

public class Server
{
    [Key]
    public ulong ServerId { get; set; }

    public BungieLocales BungieLocale { get; set; }
    public ulong? AnnouncementChannel { get; set; }
    public ulong? StaffChannel { get; set; }
    public ulong? D2Daily { get; set; }
    public ulong? D2Weekly { get; set; }
    public ulong? D2Ada { get; set; }
    public ulong? D2Gunsmith { get; set; }
    public ulong? D2Xur { get; set; }
    public ulong? MemberLogChannel { get; set; }
    public bool? MemberJoined { get; set; }
    public bool? MemberLeft { get; set; }
}

public class ServerDb : DbContext
{
    private readonly string? _connectionString;

    public ServerDb(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MySQLDb");
    }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public DbSet<Server> Servers { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var serverVersion = new MariaDbServerVersion(new Version(10, 2, 21));
        if (_connectionString != null)
            optionsBuilder.UseMySql(_connectionString, serverVersion, builder => builder.EnableRetryOnFailure());
    }
}
