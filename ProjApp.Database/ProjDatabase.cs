using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProjApp.Database.Entities;
using ProjApp.Database.SupportTypes;

namespace ProjApp.Database;

public class ProjDatabase : DbContext
{
    public ProjDatabase(DbContextOptions<ProjDatabase> options) : base(options) { }

    public DbSet<InitialVerificationJob> InitialVerificationJobs => Set<InitialVerificationJob>();
    public DbSet<DeviceType> DeviceTypes => Set<DeviceType>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Etalon> Etalons => Set<Etalon>();
    public DbSet<InitialVerification> InitialVerifications => Set<InitialVerification>();
    public DbSet<InitialVerificationFailed> FailedInitialVerifications => Set<InitialVerificationFailed>();
    public DbSet<ProtocolTemplate> ProtocolTemplates => Set<ProtocolTemplate>();
    public DbSet<VerificationMethod> VerificationMethods => Set<VerificationMethod>();

    public DbSet<PendingManometrVerification> PendingManometrVerifications => Set<PendingManometrVerification>();
    public DbSet<CompleteVerificationSuccess> CompleteVerificationSuccesses => Set<CompleteVerificationSuccess>();
    public DbSet<CompleteVerificationFail> CompleteVerificationFails => Set<CompleteVerificationFail>();

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InitialVerificationJob>()
            .Property(e => e.Date)
            .HasConversion(new YearMonthConverter());

        modelBuilder.Entity<ProtocolTemplate>()
        .Property(e => e.Checkups)
            .HasConversion(new ValueConverter<IDictionary<string, string>, string>(
                v => JsonSerializer.Serialize(v, _jsonSerializerOptions),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, _jsonSerializerOptions) ??
                     new Dictionary<string, string>()));

        modelBuilder.Entity<ProtocolTemplate>()
        .Property(e => e.Values)
            .HasConversion(new ValueConverter<IDictionary<string, object>, string>(
                v => JsonSerializer.Serialize(v, _jsonSerializerOptions),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, _jsonSerializerOptions) ??
                     new Dictionary<string, object>()));
    }
}
