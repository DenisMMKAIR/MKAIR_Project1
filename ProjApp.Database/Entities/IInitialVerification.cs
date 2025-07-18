using System.Linq.Expressions;
using ProjApp.Database.EntitiesStatic;

namespace ProjApp.Database.Entities;

public interface IInitialVerification : IVerificationBase
{
    // string DeviceTypeNumber { get; set; } - inherited
    // string DeviceSerial { get; set; } - inherited
    // DateOnly VerificationDate { get; set; } - inherited
    string VerificationTypeName { get; set; }
    string Owner { get; set; }

    // Optional
    VerificationGroup? VerificationGroup { get; set; }
    string? ProtocolNumber { get; set; }
    ulong? OwnerINN { get; set; }
    string? Worker { get; set; }
    DeviceLocation? Location { get; set; }
    string? Pressure { get; set; }
    double? Temperature { get; set; }
    double? Humidity { get; set; }
    double? MeasurementMin { get; set; }
    double? MeasurementMax { get; set; }
    string? MeasurementUnit { get; set; }
    double? Accuracy { get; set; }

    // Navigation properties
    // Device? Device { get; set; } - inherited
    // VerificationMethod? VerificationMethod { get; set; } - inherited
    // ICollection<Etalon>? Etalons { get; set; } - inherited
}

public class SuccessInitialVerification : DatabaseEntity, IInitialVerification
{
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required string Owner { get; set; }
    public required string VerificationTypeName { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required DateOnly VerifiedUntilDate { get; set; }

    // Optional
    public VerificationGroup? VerificationGroup { get; set; }
    public string? ProtocolNumber { get; set; }
    public ulong? OwnerINN { get; set; }
    public string? Worker { get; set; }
    public DeviceLocation? Location { get; set; }
    public string? Pressure { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }
    public double? MeasurementMin { get; set; }
    public double? MeasurementMax { get; set; }
    public string? MeasurementUnit { get; set; }
    public double? Accuracy { get; set; }


    // Navigation properties
    public Device? Device { get; set; }
    public VerificationMethod? VerificationMethod { get; set; }
    public ICollection<Etalon>? Etalons { get; set; }
}

public class FailedInitialVerification : DatabaseEntity, IInitialVerification
{
    public required string DeviceTypeNumber { get; set; }
    public required string DeviceSerial { get; set; }
    public required string Owner { get; set; }
    public required string VerificationTypeName { get; set; }
    public required DateOnly VerificationDate { get; set; }
    public required string FailedDocNumber { get; set; }

    // Optional
    public VerificationGroup? VerificationGroup { get; set; }
    public string? ProtocolNumber { get; set; }
    public ulong? OwnerINN { get; set; }
    public string? Worker { get; set; }
    public DeviceLocation? Location { get; set; }
    public string? Pressure { get; set; }
    public double? Temperature { get; set; }
    public double? Humidity { get; set; }
    public double? MeasurementMin { get; set; }
    public double? MeasurementMax { get; set; }
    public string? MeasurementUnit { get; set; }
    public double? Accuracy { get; set; }

    // Navigation properties
    public Device? Device { get; set; }
    public VerificationMethod? VerificationMethod { get; set; }
    public ICollection<Etalon>? Etalons { get; set; }
}

public static class InitialVerificationExtensions
{
    private static readonly Expression<Func<IInitialVerification, bool>> _allFieldFilledExpression =
        v => v.VerificationGroup != null &&
             v.ProtocolNumber != null &&
             v.OwnerINN != null &&
             v.Worker != null &&
             v.Location != null &&
             v.Pressure != null &&
             v.Temperature != null &&
             v.Humidity != null &&
             v.MeasurementMin != null &&
             v.MeasurementMax != null &&
             v.MeasurementUnit != null &&
             v.Accuracy != null &&
             v.Device != null &&
             v.Device!.DeviceType! != null &&
             v.VerificationMethod != null &&
             v.Etalons != null && v.Etalons.Count > 0;

    private static readonly Func<IInitialVerification, bool> _compiledAllFieldFilled =
        _allFieldFilledExpression.Compile();

    public static IQueryable<T> VerificationIsFilled<T>(this IQueryable<T> query) where T : IInitialVerification
    {
        return query.Cast<IInitialVerification>()
            .Where(_allFieldFilledExpression)
            .Cast<T>();
    }

    public static bool VerificationIsFilled<T>(this T verification) where T : IInitialVerification
    {
        return _compiledAllFieldFilled(verification);
    }
}
