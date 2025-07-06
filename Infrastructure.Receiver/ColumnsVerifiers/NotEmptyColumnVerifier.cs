namespace Infrastructure.Receiver.ColumnsVerifiers;

public class NotEmptyColumnVerifier : IColumnVerifier
{
    public string ErrorMessage => "Ячейка не должна быть пустой";

    public bool Verify(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
}
