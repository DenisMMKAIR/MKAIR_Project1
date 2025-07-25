using System.Reflection;
using AngleSharp.Dom;
using Infrastructure.DocumentProcessor.Forms;
using ProjApp.ProtocolForms;

namespace Infrastructure.DocumentProcessor.Creator;

internal class DavlenieSuccessDocumentCreator : DocumentCreatorBase<DavlenieForm>
{
    protected override IReadOnlyList<PropertyInfo> TypeProps { get; init; } = typeof(DavlenieForm).GetProperties();
    protected override int VerificationLineLength { get; init; } = 75;
    protected override int EtalonLineLength { get; init; } = 100;
    protected override int AdditionalLineLength { get; init; } = 130;

    public DavlenieSuccessDocumentCreator(Dictionary<string, string> signsCache, string signsDirPath) : base(signsCache, signsDirPath, HTMLForms.Davlenie) { }

    protected override string? SetSpecific(IDocument document, DavlenieForm data)
    {
        var measurement1Element = document.QuerySelector("#measurementUnit1");
        if (measurement1Element == null) return "measurementUnit1 not found";
        var prop = TypeProps.First(p => p.Name == "MeasurementUnit");
        SetElementValue(measurement1Element, prop.GetValue(data)!.ToString()!);

        var table = document.QuerySelector("#valuesTable");
        if (table == null)
        {
            return "Таблица не найдена";
        }

        var tbody = table.QuerySelector("tbody");
        if (tbody == null)
        {
            return "Таблица не найдена";
        }

        var columnFormats = new Dictionary<int, string>
        {
            { 1, "N1" }, // Давление
            { 2, "N3" }, // Показания эталона
            { 3, "N3" }, // Показания устройства (прямой ход)
            { 4, "N3" }, // Показания устройства (обратный ход)
            { 5, "N3" }, // Приведенная погрешность (прямой ход)
            { 6, "N3" }, // Приведенная погрешность (обратный ход)
            { 7, "N3" }, // Допустимая приведенная погрешность
            { 8, "N3" }, // Вариация показаний
        };

        for (int rowIndex = 0; rowIndex < 5; rowIndex++)
        {
            var row = tbody.QuerySelectorAll("tr")[rowIndex];
            if (row == null) return "Строка таблицы не найдена";

            for (int colIndex = 1; colIndex < 9; colIndex++)
            {
                var cell = row.QuerySelector($"td:nth-child({colIndex + 1})");
                if (cell == null) return "Ячейка таблицы не найдена";

                double value;
                switch (colIndex)
                {
                    case 1:
                        value = data.PressureInputs[rowIndex];
                        break;
                    case 2:
                        value = data.EtalonValues[rowIndex];
                        break;
                    case 3:
                        value = data.DeviceValues[0][rowIndex];
                        break;
                    case 4:
                        value = data.DeviceValues[1][rowIndex];
                        break;
                    case 5:
                        value = data.ActualError[0][rowIndex];
                        break;
                    case 6:
                        value = data.ActualError[1][rowIndex];
                        break;
                    case 7:
                        value = data.ValidError;
                        break;
                    case 8:
                        value = data.Variations[rowIndex];
                        break;
                    default:
                        return "Некорректный индекс столбца";
                }

                SetElementValue(cell, value.ToString(), columnFormats[colIndex]);
            }
        }

        return null;
    }
}
