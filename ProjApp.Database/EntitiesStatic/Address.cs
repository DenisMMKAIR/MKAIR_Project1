namespace ProjApp.Database.EntitiesStatic;

public static class MKAIRInfo
{
    private static readonly IReadOnlyList<(DateOnly addressDate, string address)> _addresses =
    [
        (new DateOnly(2024, 05, 07),
         "628609, Российская Федерация, Ханты-Мансийский автономный округ-Югра, город Нижневартовск, Западный промышленный узел, улица Индустриальная, дом 14, строение 11"),
        (new DateOnly(2024, 05, 08),
         "Ханты-Мансийский автономный округ - Югра, г.о. Нижневартовск, г Нижневартовск, ул Индустриальная, зд. 32, стр. 1, кабинет 14"),
    ];

    public static string? GetAddress(DateOnly date)
    {
        foreach (var (addressDate, address) in _addresses)
        {
            if (date <= addressDate) return address;
        }
        return _addresses.Last().address;
    }
}
