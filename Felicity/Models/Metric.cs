using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Felicity.Models;

public class Metric
{
    [Key] public int Id { get; set; }
    public int TimeStamp { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class MetricDb : DbContext
{
    private readonly string? _connectionString;

    public MetricDb(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MySQLDb");
    }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public DbSet<Metric> Metrics { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var serverVersion = new MariaDbServerVersion(new Version(10, 2, 21));
        if (_connectionString != null)
            optionsBuilder.UseMySql(_connectionString, serverVersion, builder => builder.EnableRetryOnFailure());
    }
}