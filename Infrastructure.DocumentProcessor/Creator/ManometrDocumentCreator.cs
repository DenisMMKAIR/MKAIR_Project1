using System.Reflection;
using AngleSharp.Dom;
using ProjApp.Database.Entities;

namespace Infrastructure.DocumentProcessor.Creator;

internal class ManometrSuccessDocumentCreator : DocumentCreatorBase<Manometr1Verification>
{
    protected override IReadOnlyList<PropertyInfo> TypeProps { get; init; } = typeof(Manometr1Verification).GetProperties();
    protected override int VerificationLineLength { get; init; } = 75;
    protected override int EtalonLineLength { get; init; } = 100;
    protected override int AdditionalLineLength { get; init; } = 130;

    public ManometrSuccessDocumentCreator(Dictionary<string, string> signsCache, string signsDirPath) : base(signsCache, signsDirPath, GetFormPath()) { }

    protected override string? SetSpecific(IDocument document, Manometr1Verification data)
    {
        var measurement1Element = document.QuerySelector("#measurementUnit1");
        if (measurement1Element == null) return "measurementUnit1 not found";
        var prop = TypeProps.First(p => p.Name == nameof(Manometr1Verification.MeasurementUnit));
        SetElementValue(measurement1Element, prop.GetValue(data)!.ToString()!);
        var measurement2Element = document.QuerySelector("#measurementUnit2");
        if (measurement2Element == null) return "measurementUnit2 not found";
        SetElementValue(measurement2Element, prop.GetValue(data)!.ToString()!);

        var table = document.QuerySelector("#valuesTable");
        if (table == null)
        {
            return "Table not found";
        }

        var tbody = table.QuerySelector("tbody");
        if (tbody == null)
        {
            return "Table body not found";
        }

        var columnFormats = new Dictionary<int, string>
        {
            { 0, "N2" }, // Показания поверяемого СИ (прямой ход)
            { 1, "N2" }, // Показания поверяемого СИ (обратный ход)
            { 2, "N3" }, // Показания эталона (прямой ход)
            { 3, "N3" }, // Показания эталона (обратный ход)
            { 4, "N2" }, // Приведенная погрешность (прямой ход)
            { 5, "N2" }, // Приведенная погрешность (обратный ход)
            { 6, "N2" }, // Допустимая приведенная погрешность
            { 7, "N2" }, // Вариация показаний
            { 8, "N2" }  // Допустимая вариация
        };

        for (int rowIndex = 0; rowIndex < 8; rowIndex++)
        {
            var row = tbody.QuerySelectorAll("tr")[rowIndex];
            if (row == null)
            {
                continue;
            }

            for (int colIndex = 0; colIndex < 9; colIndex++)
            {
                var cell = row.QuerySelector($"td:nth-child({colIndex + 1})");
                if (cell == null || cell.InnerHtml.Trim() == "-")
                {
                    continue;
                }

                double value;
                switch (colIndex)
                {
                    case 0:
                        value = data.DeviceValues[0][rowIndex];
                        break;
                    case 1:
                        value = data.DeviceValues[1][rowIndex];
                        break;
                    case 2:
                        value = data.EtalonValues[0][rowIndex];
                        break;
                    case 3:
                        value = data.EtalonValues[1][rowIndex];
                        break;
                    case 4:
                        value = data.ActualError[0][rowIndex];
                        break;
                    case 5:
                        value = data.ActualError[1][rowIndex];
                        break;
                    case 6:
                        value = data.ValidError;
                        break;
                    case 7:
                        value = data.ActualVariation[rowIndex];
                        break;
                    case 8:
                        value = data.ValidError;
                        break;
                    default:
                        return "Некорректный индекс столбца";
                }

                SetElementValue(cell, value.ToString(), columnFormats[colIndex]);
            }
        }

        return null;
    }


    private static string GetFormPath() => Path.Combine(AppContext.BaseDirectory, "Forms", "Manometr.html");
}
