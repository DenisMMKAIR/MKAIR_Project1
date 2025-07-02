using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ProjApp.Database.EntitiesStatic;

namespace Infrastructure.Receiver;

internal class ExcelFileProcessor<TProcessData, TResultData> where TProcessData : IDataItem<TResultData>, new()
{
    private static readonly Type _itemProperties = typeof(TProcessData);
    private readonly IColumnsSetup _columnsSetup;
    private readonly ILogger _logger;

    public ExcelFileProcessor(ILogger logger, IColumnsSetup columnsSetup) => (_logger, _columnsSetup) = (logger, columnsSetup);

    public IReadOnlyList<TResultData> ReadVerificationFile(Stream fileStream, string fileName, string sheetName, string dataRange, DeviceLocation location)
    {
        using var wb = new XSSFWorkbook(fileStream);

        ISheet sheet = null!;
        for (var i = 0; i < wb.NumberOfSheets; i++)
        {
            if (wb.GetSheetName(i).Equals(sheetName, StringComparison.OrdinalIgnoreCase))
            {
                sheet = wb.GetSheetAt(i);
                break;
            }
        }

        if (sheet == null)
        {
            throw new Exception($"Файл '{fileName}'. Лист с именем '{sheetName}' не найден.");
        }

        var (startRowIndex, startColumnIndex, endRowIndex, endColumnIndex) = ParseRange(dataRange, fileName);

        var headerRow = sheet.GetRow(startRowIndex) ??
            throw new Exception($"Файл '{fileName}'. Строка с заголовками не найдена.");

        SetupColumnIndices(startColumnIndex, endColumnIndex, headerRow, fileName);

        var processedItems = new List<TResultData>(endRowIndex);

        for (var rowNum = startRowIndex + 1; rowNum <= endRowIndex; rowNum++)
        {
            var row = sheet.GetRow(rowNum);
            if (row == null) continue;

            var item = new TProcessData();
            FillItem(row, item, fileName);
            var newResultItem = item.PostProcess(fileName, rowNum, location);
            processedItems.Add(newResultItem);
        }

        return processedItems;
    }

    private static (int Row, int Col) ParseCellReference(string cellReference)
    {
        int col = 0;
        int row = 0;
        int i = 0;

        // Parse column letters
        while (i < cellReference.Length && char.IsLetter(cellReference[i]))
        {
            col = col * 26 + char.ToUpper(cellReference[i]) - 'A' + 1;
            i++;
        }

        // Parse row numbers
        while (i < cellReference.Length && char.IsDigit(cellReference[i]))
        {
            row = row * 10 + (cellReference[i] - '0');
            i++;
        }

        // Excel uses 1-based indexing for rows and columns
        return (row - 1, col - 1);
    }

    private static (int StartRow, int StartCol, int EndRow, int EndCol) ParseRange(string range, string fileName)
    {
        var cells = range.Split(':');

        if (cells.Length != 2)
        {
            throw new ArgumentException($"Файл '{fileName}'. Неверный формат диапазона. Заданный формат {range}. Ожидаемый формат A1:D100");
        }

        var (startRowIndex, startColumnIndex) = ParseCellReference(cells[0]);
        var (endRowIndex, endColumnIndex) = ParseCellReference(cells[1]);

        return (startRowIndex, startColumnIndex, endRowIndex, endColumnIndex);
    }

    private void SetupColumnIndices(int startColumnIndex, int lastColumnIndex, IRow headerRow, string fileName)
    {
        var processedColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var currentColumnIndex = startColumnIndex; currentColumnIndex <= lastColumnIndex; currentColumnIndex++)
        {
            var currentCell = headerRow.GetCell(currentColumnIndex);
            if (currentCell == null) continue;

            var cellValue = currentCell.ToString()?.Trim().ToLower() ??
                throw new Exception($"Файл '{fileName}'. Не удалось получить значение ячейки колонки {currentColumnIndex}");

            if (string.IsNullOrWhiteSpace(cellValue)) continue;

            foreach (var column in _columnsSetup.Columns)
            {
                if (column.IncomingNames.Contains(cellValue, StringComparer.OrdinalIgnoreCase))
                {
                    column.ColumnIndex = (uint)currentColumnIndex;
                    processedColumns.Add(column.InternalName);
                    break;
                }
            }
        }

        var missingColumns = _columnsSetup.Columns
            .Where(c => !processedColumns.Contains(c.InternalName))
            .Select(c => string.Join('|', c.IncomingNames))
            .ToList();

        if (missingColumns.Count != 0)
        {
            throw new Exception($"Файл '{fileName}'. Не удалось найти следующие столбцы: {string.Join(", ", missingColumns)}");
        }
    }

    private void FillItem(IRow row, TProcessData item, string fileName)
    {
        foreach (var column in _columnsSetup.Columns)
        {
            var cell = row.GetCell((int)column.ColumnIndex);
            string genErrorMsg(string msg) => $"Файл '{fileName}', столбец '{string.Join(",", column.IncomingNames)}', строка {row.RowNum + 1}. {msg}";

            var cellValue = cell?.ToString()?.Trim();

            if (cellValue == null)
            {
                _logger.LogWarning(genErrorMsg("Ячейка пуста"));
                continue;
            }

            foreach (var verifier in _columnsSetup.AllColumnsVerifiers)
            {
                if (!verifier.Verify(cellValue))
                {
                    throw new Exception(genErrorMsg(verifier.ErrorMessage));
                }
            }

            foreach (var verifier in column.Verifiers)
            {
                if (!verifier.Verify(cellValue))
                {
                    throw new Exception(genErrorMsg(verifier.ErrorMessage));
                }
            }

            foreach (var normalizer in column.Normalizers)
            {
                cellValue = normalizer.Normalize(cellValue);
            }

            var property = _itemProperties.GetProperty(column.InternalName)!;

            if (property.PropertyType == typeof(uint?))
            {
                try
                {
                    property.SetValue(item, uint.Parse(cellValue));
                }
                catch (FormatException)
                {
                    throw new FormatException(genErrorMsg($"Неверный формат числа '{cellValue}'"));
                }
            }
            else if (property.PropertyType == typeof(DateOnly?))
            {
                try
                {
                    var date = DateOnly.Parse(cellValue);
                    property.SetValue(item, date);
                }
                catch (FormatException)
                {
                    throw new FormatException(genErrorMsg($"Неверный формат даты '{cellValue}'"));
                }
            }
            else if (property.PropertyType == typeof(double?))
            {
                try
                {
                    var doubleValue = double.Parse(cellValue);
                    property.SetValue(item, doubleValue);
                }
                catch (FormatException)
                {
                    throw new FormatException(genErrorMsg($"Неверный формат числа '{cellValue}'"));
                }
            }
            else if (property.PropertyType.IsEnum)
            {
                try
                {
                    var enumValue = Enum.Parse(property.PropertyType, cellValue, true);
                    property.SetValue(item, enumValue);
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException(genErrorMsg($"Неверное значение перечисления '{cellValue}'"));
                }
            }
            else
            {
                property.SetValue(item, cellValue);
            }
        }
    }
}
