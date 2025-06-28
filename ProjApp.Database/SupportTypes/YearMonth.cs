using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ProjApp.Database.SupportTypes;

public readonly record struct YearMonth(int Year, int Month)
{
    public DateOnly ToDateOnly() => new(Year, Month, 1);
    public DateOnly ToEndMonthDate() => new(Year, Month, DateTime.DaysInMonth(Year, Month));
    public static implicit operator YearMonth((int year, int month) tuple) => new(tuple.year, tuple.month);
    public static implicit operator YearMonth(DateOnly date) => new(date.Year, date.Month);
    // public static implicit operator DateOnly(YearMonth date) => new(date.Year, date.Month, 1);
    public static bool operator ==(YearMonth left, DateOnly right) => left.Year == right.Year && left.Month == right.Month;
    public static bool operator !=(YearMonth left, DateOnly right) => left.Year != right.Year || left.Month != right.Month;
    public static bool operator ==(DateOnly left, YearMonth right) => left.Year == right.Year && left.Month == right.Month;
    public static bool operator !=(DateOnly left, YearMonth right) => left.Year != right.Year || left.Month != right.Month;

    public override string ToString() => $"{Year}.{Month}";
}

public class YearMonthConverter : ValueConverter<YearMonth, DateOnly>
{
    public YearMonthConverter() : base(yearMonth => yearMonth.ToDateOnly(), dateOnly => dateOnly) { }
}
