using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;
using ProjApp.Database.SupportTypes;

namespace ProjApp.Database;

public class ProjDatabase : DbContext
{
    public ProjDatabase(DbContextOptions<ProjDatabase> options) : base(options) { }

    public DbSet<InitialVerificationJob> InitialVerificationJobs => Set<InitialVerificationJob>();
    public DbSet<DeviceType> DeviceTypes => Set<DeviceType>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Etalon> Etalons => Set<Etalon>();
    public DbSet<Owner> Owners => Set<Owner>();
    public DbSet<VerificationMethod> VerificationMethods => Set<VerificationMethod>();
    public DbSet<VerificationMethodFile> VerificationMethodFiles => Set<VerificationMethodFile>();
    public DbSet<ProtocolTemplate> ProtocolTemplates => Set<ProtocolTemplate>();

    public DbSet<SuccessInitialVerification> SuccessInitialVerifications => Set<SuccessInitialVerification>();
    public DbSet<FailedInitialVerification> FailedInitialVerifications => Set<FailedInitialVerification>();
    public DbSet<SuccessVerification> SuccessVerifications => Set<SuccessVerification>();
    public DbSet<FailedVerification> FailedVerifications => Set<FailedVerification>();

    public DbSet<Manometr1Verification> Manometr1Verifications => Set<Manometr1Verification>();

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureInitialVerificationJob(modelBuilder);
        ConfigureVerificationMethod(modelBuilder);
        ConfigureInitialVerifications(modelBuilder);
        ConfigureVerifications(modelBuilder);
        ConfigureManometr1Verifications(modelBuilder);
    }

    private static void ConfigureInitialVerificationJob(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InitialVerificationJob>()
            .Property(e => e.Date)
            .HasConversion(new YearMonthConverter());
    }

    private static void ConfigureVerificationMethod(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VerificationMethod>()
            .HasMany(e => e.VerificationMethodFiles)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<VerificationMethod>()
            .Property(e => e.Checkups)
            .HasConversion(new ValueConverter<Dictionary<VerificationMethodCheckups, string>, string>(
                v => JsonSerializer.Serialize(v, _jsonSerializerOptions),
                v => JsonSerializer.Deserialize<Dictionary<VerificationMethodCheckups, string>>(v, _jsonSerializerOptions) ??
                    new Dictionary<VerificationMethodCheckups, string>()));
    }

    private static void ConfigureInitialVerifications(ModelBuilder modelBuilder)
    {
    }

    private static void ConfigureVerifications(ModelBuilder modelBuilder)
    {
    }

    private static void ConfigureManometr1Verifications(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manometr1Verification>()
            .Property(e => e.DeviceValues)
            .HasConversion(new ValueConverter<IReadOnlyList<IReadOnlyList<double>>, string>(
                v => JsonSerializer.Serialize(v, _jsonSerializerOptions),
                v => JsonSerializer.Deserialize<IReadOnlyList<IReadOnlyList<double>>>(v, _jsonSerializerOptions) ??
                    new List<IReadOnlyList<double>>()));

        modelBuilder.Entity<Manometr1Verification>()
            .Property(e => e.EtalonValues)
            .HasConversion(new ValueConverter<IReadOnlyList<IReadOnlyList<double>>, string>(
                v => JsonSerializer.Serialize(v, _jsonSerializerOptions),
                v => JsonSerializer.Deserialize<IReadOnlyList<IReadOnlyList<double>>>(v, _jsonSerializerOptions) ??
                    new List<IReadOnlyList<double>>()));

        modelBuilder.Entity<Manometr1Verification>()
            .Property(e => e.ActualError)
            .HasConversion(new ValueConverter<IReadOnlyList<IReadOnlyList<double>>, string>(
                v => JsonSerializer.Serialize(v, _jsonSerializerOptions),
                v => JsonSerializer.Deserialize<IReadOnlyList<IReadOnlyList<double>>>(v, _jsonSerializerOptions) ??
                    new List<IReadOnlyList<double>>()));
    }
}
