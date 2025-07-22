namespace Infrastructure.Receiver.ColumnsVerifiers;

public partial class FloatPercentageVerifier : IColumnVerifier
{
    public string ErrorMessage { get; } = "Ошибка ввода процента";

    public bool Verify(string value)
    {
        if (!float.TryParse(value, out var result)) return false;
        return result >= 0 && result <= 1;
    }
}
