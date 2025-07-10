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
            .HasConversion(new ValueConverter<Dictionary<string, string>, string>(
                v => JsonSerializer.Serialize(v, _jsonSerializerOptions),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, _jsonSerializerOptions) ??
                    new Dictionary<string, string>()));
    }

    private static void ConfigureInitialVerifications(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SuccessInitialVerification>()
            .Property(e => e.AdditionalInfo)
            .HasConversion(new ValueConverter<Dictionary<string, object>, string>(
                v => JsonSerializer.Serialize(v, _jsonSerializerOptions),
                v => DeserializeDict(v)));

        modelBuilder.Entity<FailedInitialVerification>()
            .Property(e => e.AdditionalInfo)
            .HasConversion(new ValueConverter<Dictionary<string, object>, string>(
                v => JsonSerializer.Serialize(v, _jsonSerializerOptions),
                v => DeserializeDict(v)));
    }

    private static void ConfigureVerifications(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SuccessVerification>()
            .Property(e => e.AdditionalInfo)
            .HasConversion(new ValueConverter<Dictionary<string, object>, string>(
                v => JsonSerializer.Serialize(v, _jsonSerializerOptions),
                v => DeserializeDict(v)));

        modelBuilder.Entity<FailedVerification>()
            .Property(e => e.AdditionalInfo)
            .HasConversion(new ValueConverter<Dictionary<string, object>, string>(
                v => JsonSerializer.Serialize(v, _jsonSerializerOptions),
                v => DeserializeDict(v)));
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

    private static Dictionary<string, object> DeserializeDict(string serializedDict)
    {
        var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(serializedDict, _jsonSerializerOptions);
        if (dict == null)
            return new Dictionary<string, object>();

        var result = new Dictionary<string, object>();
        foreach (var kvp in dict)
        {
            var value = ConvertJsonElementToObject(kvp.Value);
            ArgumentNullException.ThrowIfNull(value);
            result[kvp.Key] = value;
        }
        return result;
    }

    private static object? ConvertJsonElementToObject(JsonElement jsonElement)
    {
        switch (jsonElement.ValueKind)
        {
            case JsonValueKind.Number:
                double rawValue = jsonElement.GetDouble();
                if (rawValue == Math.Truncate(rawValue))
                {
                    if (rawValue >= int.MinValue && rawValue <= int.MaxValue)
                        return (int)rawValue;
                    else
                        return (long)rawValue;
                }
                else
                    return rawValue;
            case JsonValueKind.String:
                return jsonElement.GetString();
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
                return false;
            case JsonValueKind.Null:
                return null;
            default:
                throw new InvalidOperationException($"Unsupported JSON value kind: {jsonElement.ValueKind}");
        }
    }
}
