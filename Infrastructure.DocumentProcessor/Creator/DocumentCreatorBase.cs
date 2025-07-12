using System.Reflection;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace Infrastructure.DocumentProcessor.Creator;

// TODO: Replace reflection with the source generator
// TODO: Cache Sings
internal abstract class DocumentCreatorBase<T>
{
    private readonly string _fileContent;
    private readonly Dictionary<string, string> _signsCache;
    private readonly string _signsDirPath;
    protected abstract IReadOnlyList<PropertyInfo> TypeProps { get; init; }
    protected abstract int VerificationLineLength { get; init; }
    protected abstract int EtalonLineLength { get; init; }
    protected abstract int AdditionalLineLength { get; init; }

    public DocumentCreatorBase(Dictionary<string, string> signsCache, string signsDirPath, string formPath)
    {
        _signsCache = signsCache;
        _signsDirPath = signsDirPath;
        _fileContent = File.ReadAllText(formPath);
    }

    public async Task<HTMLCreationResult> CreateAsync(T data)
    {
        var config = Configuration.Default.WithDefaultLoader();
        using var context = BrowsingContext.New(config);
        using var document = await context.OpenAsync(r => r.Content(_fileContent));

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

    private string? SetVerification(IDocument document, T data)
    {
        var prop = TypeProps.First(p => p.Name.Equals("verificationsinfo", StringComparison.OrdinalIgnoreCase));
        if (prop == null) return "Data property verificationsinfo not found";
        var verificationsText = prop.GetValue(data)!.ToString()!;
        var mainLine = document.QuerySelector<IHtmlParagraphElement>("#mainLine_verifications")!;
        var result = SetLine(mainLine, VerificationLineLength, ref verificationsText);
        if (result != null) return result;
        if (verificationsText.Length == 0) return null;

        var additionalLine = document.QuerySelector<IHtmlParagraphElement>("#additionalLine_verifications")!;
        result = SetLine(additionalLine, AdditionalLineLength, ref verificationsText);
        if (result != null) return result;
        if (verificationsText.Length == 0) return null;

        var addInElement = document.QuerySelector("#addIn_verifications")!;

        while (verificationsText.Length > 0)
        {
            var newLine = (IHtmlParagraphElement)additionalLine.Clone(false);
            result = SetLine(newLine, AdditionalLineLength, ref verificationsText);
            if (result != null) return result;
            addInElement.Append(newLine);
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
        result = SetLine(additionalLine, AdditionalLineLength, ref etalonsText);
        if (result != null) return result;
        if (etalonsText.Length == 0) return null;

        var addAfterElement = document.QuerySelector("#addAfter_etalons")!;

        while (etalonsText.Length > 0)
        {
            var newLine = (IHtmlParagraphElement)additionalLine.Clone(false);
            result = SetLine(newLine, AdditionalLineLength, ref etalonsText);
            if (result != null) return result;
            addAfterElement.InsertAfter(newLine);
            addAfterElement = newLine;
        }

        return null;
    }

    private async Task<string?> SetSignAsync(IDocument document, T data)
    {
        var worker = (string)TypeProps.First(p => p.Name == "Worker").GetValue(data)!;
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
        var randomLeft = Random.Shared.Next(0, 200);
        imgElement.SetAttribute("style", $"position: absolute; top: 50%; transform: translateY(-50%); left: {randomLeft}px; height: 20px; width: auto;");

        imgElement.Source = sign;

        return null;
    }
}
