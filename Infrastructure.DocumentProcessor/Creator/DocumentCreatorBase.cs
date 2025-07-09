using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Infrastructure.SharedCode;
using System.Reflection;

namespace Infrastructure.DocumentProcessor.Creator;

// TODO: Replace reflection with the source generator
// TODO: Cache Sings
internal abstract class DocumentCreatorBase<T> where T : IDocumentData
{
    private readonly string _fileContent;
    private readonly string _signsDirPath;
    private readonly IDictionary<string, string> _signsCache = new Dictionary<string, string>();
    protected abstract IReadOnlyList<PropertyInfo> TypeProps { get; init; }
    protected abstract int VerificationLineLength { get; init; }
    protected abstract int EtalonLineLength { get; init; }
    protected abstract int AdditionalLineLength { get; init; }

    public DocumentCreatorBase(string signDirPath, string filePath)
        => (_signsDirPath, _fileContent) = (signDirPath, File.ReadAllText(filePath));

    public async Task<CreationResult> CreateAsync(T data)
    {
        var config = Configuration.Default.WithDefaultLoader();
        using var context = BrowsingContext.New(config);
        using var document = await context.OpenAsync(r => r.Content(_fileContent));

        foreach (var idValueElement in document.QuerySelectorAll("[id]").Where(e => e.Id!.StartsWith("setValue_")))
        {
            var id = idValueElement.Id!["setValue_".Length..].ToLower();
            var prop = TypeProps.FirstOrDefault(t => t.Name.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (prop == null) return CreationResult.Failure($"Data property {id} not found");
            idValueElement.InnerHtml = prop.GetValue(data)!.ToString()!;
        }

        var result = SetVerification(document, data);
        if (result != null) return CreationResult.Failure(result);

        result = SetEtalons(document, data);
        if (result != null) return CreationResult.Failure(result);

        result = await SetSignAsync(document, data);
        if (result != null) return CreationResult.Failure(result);

        var specifiResult = await SetSpecificAsync(document, data);
        if (specifiResult != null) return CreationResult.Failure(specifiResult);

        return CreationResult.Success(document.DocumentElement.OuterHtml);
    }

    protected abstract Task<string?> SetSpecificAsync(IDocument document, T data);

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
        var verificationList = (IReadOnlyList<string>)TypeProps.First(p => p.Name == "VerificationsName").GetValue(data)!;
        var verificationsText = string.Join("; ", verificationList);

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
        var etalonsList = (IReadOnlyList<string>)TypeProps.First(p => p.Name == "Etalons").GetValue(data)!;
        var etalonsText = string.Join("; ", etalonsList);

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
        var signsCount = Directory.EnumerateFiles(_signsDirPath, $"{worker}*.png").Count();

        if (signsCount == 0) return "Сотрудник не найден в списке подписей.";

        var randomSignIndex = Random.Shared.Next(1, signsCount);
        randomSignIndex = 1;
        var key = $"{worker} {randomSignIndex}";

        if (!_signsCache.TryGetValue(key, out var signBase64))
        {
            var filePath = $"{_signsDirPath}\\{key}.png";

            if (!File.Exists(filePath)) return $"Файл подписи {key}.png не найден.";

            var bytes = await File.ReadAllBytesAsync(filePath);
            var base64 = Convert.ToBase64String(bytes);
            signBase64 = $"data:image/png;base64,{base64}";
            _signsCache[key] = signBase64;
        }

        var imgElement = document.QuerySelector<IHtmlImageElement>("#sign")!;
        imgElement.Source = signBase64;

        return null;
    }
}
