using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ProjApp.Database.SupportTypes;

public readonly record struct YearMonth(int Year, int Month) : IComparable<YearMonth>
{
    public DateOnly ToDateOnly() => new(Year, Month, 1);
    public DateOnly ToEndMonthDate() => new(Year, Month, DateTime.DaysInMonth(Year, Month));
    public static YearMonth? Parse(string? str)
    {
        if (str == null) return null;
        if (str.Length < 6) throw new FormatException($"Некорректная длина строки '{str}'. Пример: 2024.01");
        var s = str.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (s.Length != 2) throw new FormatException($"Некорректная строка '{str}'. Пример: 2024.01");
        if (!int.TryParse(s[0], out var year) || !int.TryParse(s[1], out var month))
        {
            throw new FormatException($"Некорректная строка '{str}'. Пример: 2024.01");
        }
        return new(year, month);
    }
    public static implicit operator YearMonth((int year, int month) tuple) => new(tuple.year, tuple.month);
    public static implicit operator YearMonth(DateOnly date) => new(date.Year, date.Month);
    // public static implicit operator DateOnly(YearMonth date) => new(date.Year, date.Month, 1);
    public static bool operator ==(YearMonth left, DateOnly right) => left.Year == right.Year && left.Month == right.Month;
    public static bool operator !=(YearMonth left, DateOnly right) => left.Year != right.Year || left.Month != right.Month;
    public static bool operator ==(DateOnly left, YearMonth right) => left.Year == right.Year && left.Month == right.Month;
    public static bool operator !=(DateOnly left, YearMonth right) => left.Year != right.Year || left.Month != right.Month;

    public override string ToString() => $"{Year}.{Month}";

    public int CompareTo(YearMonth other)
    {
        int yearComparison = Year.CompareTo(other.Year);
        if (yearComparison != 0)
            return yearComparison;
        return Month.CompareTo(other.Month);
    }
}

public class YearMonthConverter : ValueConverter<YearMonth, DateOnly>
{
    public YearMonthConverter() : base(yearMonth => yearMonth.ToDateOnly(), dateOnly => dateOnly) { }
}
