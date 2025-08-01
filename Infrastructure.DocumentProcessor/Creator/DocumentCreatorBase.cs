using System.Reflection;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace Infrastructure.DocumentProcessor.Creator;

// TODO: Replace reflection with the source generator
internal abstract class DocumentCreatorBase<T>
{
    private readonly string _fileContent;
    private readonly Dictionary<string, string> _signsCache;
    private readonly string _signsDirPath;
    protected abstract IReadOnlyList<PropertyInfo> TypeProps { get; init; }
    // TODO: FIX THIS. Precise line definition needs
    protected int FullLineLength { get; init; } = 105;
    protected virtual int VerificationLineLength { get; init; } = 70;
    protected virtual int EtalonLineLength { get; init; } = 80;

    public DocumentCreatorBase(Dictionary<string, string> signsCache, string signsDirPath, string formPath)
    {
        _signsCache = signsCache;
        _signsDirPath = signsDirPath;
        _fileContent = File.ReadAllText(formPath);
    }

    public async Task<HTMLCreationResult> CreateAsync(T data, CancellationToken? cancellationToken = null)
    {
        var config = Configuration.Default.WithDefaultLoader();
        using var context = BrowsingContext.New(config);
        using var document = await context.OpenAsync(r => r.Content(_fileContent), cancellationToken ?? CancellationToken.None);

        var deviceInfoResult = SetDeviceInfo(document, data);
        if (deviceInfoResult != null) return HTMLCreationResult.Failure(deviceInfoResult);

        foreach (var idValueElement in document.QuerySelectorAll("[id]").Where(e => e.Id!.StartsWith("setValue_")))
        {
            var id = idValueElement.Id!["setValue_".Length..].ToLower();
            var prop = TypeProps.FirstOrDefault(t => t.Name.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (prop == null) return HTMLCreationResult.Failure($"Data property {id} not found");
            SetElementValue(idValueElement, prop.GetValue(data)!.ToString()!);
        }

        var result = SetVerification(document, data);
        if (result != null) return HTMLCreationResult.Failure(result);

        result = SetEtalons(document, data);
        if (result != null) return HTMLCreationResult.Failure(result);

        result = SetCheckups(document, data);
        if (result != null) return HTMLCreationResult.Failure(result);

        if (cancellationToken.HasValue &&
        cancellationToken.Value.IsCancellationRequested)
        {
            return HTMLCreationResult.Failure("Отмена");
        }

        result = await SetSignAsync(document, data);
        if (result != null) return HTMLCreationResult.Failure(result);

        var specifiResult = SetSpecific(document, data);
        if (specifiResult != null) return HTMLCreationResult.Failure(specifiResult);

        return HTMLCreationResult.Success(document.DocumentElement.OuterHtml);
    }

    protected abstract string? SetSpecific(IDocument document, T data);

    protected static void SetElementValue(IElement element, string value, string? format = null)
    {
        format ??= element.GetAttribute("data-format");
        if (format != null)
        {
            if (format.StartsWith('N'))
            {
                element.InnerHtml = double.Parse(value).ToString(format);
            }
            else if (format == "0%")
            {
                element.InnerHtml = double.Parse(value).ToString(format);
            }
            else
            {
                throw new Exception($"Неподдерживаемый формат: {format}");
            }
        }
        else
        {
            element.InnerHtml = value;
        }
    }

    protected string? SetLine(IHtmlParagraphElement element, int lineLength, ref string text)
    {
        if (text.Length <= lineLength)
        {
            // Text fits entirely
            element.InnerHtml = text;
            text = string.Empty;
            return null;
        }

        // Find the last space within maxLength+1 characters
        int splitPos = text.Substring(0, lineLength + 1).LastIndexOf(' ');

        if (splitPos > 0)
        {
            // Split at the found space
            element.InnerHtml = text.Substring(0, splitPos);
            text = text.Substring(splitPos + 1);
            return null;
        }

        return "Нет возможности разделить текст. Пробелы отсутствуют.";
    }

    private string? SetDeviceInfo(IDocument document, T data)
    {
        var deviceTypeNameElement = document.QuerySelector<IHtmlParagraphElement>("#manual_deviceInfo");
        if (deviceTypeNameElement == null) return "manual_deviceInfo not found";
        var insertAfterDeviceNameElement = deviceTypeNameElement.NextSibling;
        if (insertAfterDeviceNameElement == null) return "manual_deviceInfo sibling not found";
        var prop = TypeProps.FirstOrDefault(p => p.Name.Equals("deviceInfo", StringComparison.OrdinalIgnoreCase));
        if (prop == null) return "Data property deviceInfo not found";
        var deviceInfo = prop.GetValue(data)!.ToString()!;
        var deviceNameError = SetLine(deviceTypeNameElement, FullLineLength, ref deviceInfo);
        if (deviceNameError != null) return deviceNameError;

        while (deviceInfo.Length > 0)
        {
            var newDeviceNameLineElement = (IHtmlParagraphElement)deviceTypeNameElement.Clone(false);
            deviceNameError = SetLine(newDeviceNameLineElement, FullLineLength, ref deviceInfo);
            if (deviceNameError != null) return deviceNameError;
            insertAfterDeviceNameElement.InsertAfter(newDeviceNameLineElement);
            insertAfterDeviceNameElement = newDeviceNameLineElement;
        }

        return null;
    }

    private string? SetVerification(IDocument document, T data)
    {
        var prop = TypeProps.FirstOrDefault(p => p.Name.Equals("verificationsinfo", StringComparison.OrdinalIgnoreCase));
        if (prop == null) return "Data property verificationsinfo not found";
        var verificationsText = prop.GetValue(data)!.ToString()!;
        var mainLine = document.QuerySelector<IHtmlParagraphElement>("#manual_verificationMethodMain")!;
        var result = SetLine(mainLine, VerificationLineLength, ref verificationsText);
        if (result != null) return result;
        if (verificationsText.Length == 0) return null;

        var vmAdditionalContainer = document.QuerySelector<IHtmlDivElement>("#manual_verificationMethodAdditional")!;

        while (verificationsText.Length > 0)
        {
            var element = document.CreateElement<IHtmlParagraphElement>();
            element.ClassName = "line";
            vmAdditionalContainer.AppendChild(element);
            SetLine(element, FullLineLength, ref verificationsText);
        }

        return null;
    }

    private string? SetEtalons(IDocument document, T data)
    {
        var prop = TypeProps.FirstOrDefault(p => p.Name.Equals("EtalonsInfo", StringComparison.OrdinalIgnoreCase));
        if (prop == null) return "Data property EtalonsInfo not found";
        var etalonsText = prop.GetValue(data)!.ToString()!;

        var mainLine = document.QuerySelector<IHtmlParagraphElement>("#mainLine_etalons")!;
        var result = SetLine(mainLine, EtalonLineLength, ref etalonsText);
        if (result != null) return result;
        if (etalonsText.Length == 0) return null;

        var additionalLine = document.QuerySelector<IHtmlParagraphElement>("#additionalLine_etalons")!;
        result = SetLine(additionalLine, FullLineLength, ref etalonsText);
        if (result != null) return result;
        if (etalonsText.Length == 0) return null;

        var addAfterElement = document.QuerySelector("#addAfter_etalons")!;

        while (etalonsText.Length > 0)
        {
            var newLine = (IHtmlParagraphElement)additionalLine.Clone(false);
            result = SetLine(newLine, FullLineLength, ref etalonsText);
            if (result != null) return result;
            addAfterElement.InsertAfter(newLine);
            addAfterElement = newLine;
        }

        return null;
    }

    private string? SetCheckups(IDocument document, T data)
    {
        var verificationInfoProp = TypeProps.FirstOrDefault(p => p.Name.Equals("VerificationsInfo", StringComparison.OrdinalIgnoreCase));
        if (verificationInfoProp == null) return "Data property VerificationsInfo not found";
        var verificationInfo = verificationInfoProp.GetValue(data)!.ToString()!;

        var checkupsProp = TypeProps.FirstOrDefault(p => p.Name.Equals("Checkups", StringComparison.OrdinalIgnoreCase));
        if (checkupsProp == null) return "Data property Checkups not found";
        var checkups = (Dictionary<string, string>)checkupsProp.GetValue(data)!;

        var checkupElement = document.QuerySelector<IHtmlParagraphElement>("#manual_checkup");
        if (checkupElement == null) return "manual_checkup not found";

        var checkupNum = 1;

        foreach (var (key, value) in checkups)
        {
            var checkupNumElement = checkupElement.QuerySelector("#manual_checkupNum");
            if (checkupNumElement == null) return "manual_checkupNum not found";
            checkupNumElement.TextContent = $"{checkupNum++}. ";

            var checkupTitleElement = checkupElement.QuerySelector("#manual_checkupTitle");
            if (checkupTitleElement == null) return "manual_checkupTitle not found";
            checkupTitleElement.TextContent = key;

            var checkupValueElement = checkupElement.QuerySelector("#manual_checkupValue");
            if (checkupValueElement == null) return "manual_checkupValue not found";
            checkupValueElement.TextContent = value;

            var checkupVerificationElement = checkupElement.QuerySelector("#manual_checkupVerification");
            if (checkupVerificationElement != null)
            {
                checkupVerificationElement.TextContent = verificationInfo;
            }

            var newCheckupElement = (IHtmlParagraphElement)checkupElement.Clone(true);
            checkupElement.After(newCheckupElement);
            checkupElement = newCheckupElement;
        }

        checkupElement.Remove();

        return null;
    }

    private async Task<string?> SetSignAsync(IDocument document, T data)
    {
        var prop = TypeProps.FirstOrDefault(p => p.Name == "Worker");
        if (prop == null) return "Data property Worker not found";
        var worker = (string)prop.GetValue(data)!;
        worker = worker.ToLower();

        if (!_signsCache.TryGetValue(worker, out var _))
        {
            var signFilesPaths = Directory.GetFiles(_signsDirPath, $"{worker}*.png");

            if (signFilesPaths.Length < 12) return $"Подпись сотрудника {worker} не найдена. Или вариантов меньше 12";

            foreach (var filePath in signFilesPaths)
            {
                var bytes = await File.ReadAllBytesAsync(filePath);
                var base64 = Convert.ToBase64String(bytes);
                var signBase64 = $"data:image/png;base64,{base64}";
                var cacheKey = Path.GetFileNameWithoutExtension(filePath);
                _signsCache[cacheKey] = signBase64;
            }
        }

        var signsCount = _signsCache.Where(s => s.Key.StartsWith(worker)).Count();
        var randomSignIndex = Random.Shared.Next(1, signsCount);
        var key = $"{worker} {randomSignIndex}";
        var _ = _signsCache.TryGetValue(key, out var sign);

        var imgElement = document.QuerySelector<IHtmlImageElement>("#sign")!;
        var randomTop = Random.Shared.Next(20, 30);
        var randomLeft = Random.Shared.Next(30, 150);
        var randomHeight = Random.Shared.Next(31, 33);
        imgElement.SetAttribute("style", $"top: {randomTop}px; left: {randomLeft}px;height: {randomHeight}px;");
        imgElement.Source = sign;

        return null;
    }
}
