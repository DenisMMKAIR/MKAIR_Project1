using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.FGIS.Database;

public class FGISDatabaseFactory
{
    public static FGISDatabase Create()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<FGISDatabase>()
            .Build();
        var options = new DbContextOptionsBuilder<FGISDatabase>()
            .UseNpgsql(configuration.GetConnectionString("FGISCache"))
            .UseSnakeCaseNamingConvention()
            .Options;
        return new FGISDatabase(options);
    }
}

public class FGISDatabase : DbContext
{
    public FGISDatabase(DbContextOptions<FGISDatabase> options) : base(options) { }

    public DbSet<DeviceType> DeviceTypes => Set<DeviceType>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Etalon> Etalons => Set<Etalon>();
    public DbSet<Verification> Verifications => Set<Verification>();
}

public record DeviceType(
    Guid Id,
    string DeviceTypeNumber,
    string DeviceTypeNotation,
    string DeviceTypeTitle);

public record Device(
    Guid Id,
    string DeviceSerialNumber,
    uint DeviceManufacturedYear,
    string DeviceModification);

public record Etalon(
    Guid Id,
    string RegNumber,
    string TypeNumber,
    string TypeTitle,
    string Notation,
    string Modification,
    string ManufactureNum,
    uint ManufactureYear,
    string RankCode,
    string RankTitle,
    string SchemaTitle);

public record Verification(
    Guid Id,
    string OwnerName,
    DateOnly Date,
    DateOnly NextDate,
    string VerificationName,
    bool Applicable,
    string AdditionalInfo)
{
    public DeviceType? DeviceType { get; set; }
    public Device? Device { get; set; }
    public IReadOnlyList<Etalon>? Etalons { get; set; }
}