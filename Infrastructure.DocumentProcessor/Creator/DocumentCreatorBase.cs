using System.Reflection;
using PuppeteerSharp;

namespace Infrastructure.DocumentProcessor.Creator;

// TODO: Replace reflection with the source generator
internal abstract class DocumentCreatorBase<T>
{
    private readonly Browser _browser;
    private readonly string _htmlForm;
    private readonly Dictionary<string, string> _signsCache;
    private readonly string _signsDirPath;
    protected abstract IReadOnlyList<PropertyInfo> TypeProps { get; init; }

    public DocumentCreatorBase(Browser browser, Dictionary<string, string> signsCache, string signsDirPath, string formPath)
    {
        _browser = browser;
        _signsCache = signsCache;
        _signsDirPath = signsDirPath;
        _htmlForm = File.ReadAllText(formPath);
    }

    public async Task<HTMLCreationResult> CreateAsync(T data)
    {
        var page = await _browser.CreatePageAsync();
        await page.SetContentAsync(_htmlForm);

        var result = await SetDeviceAsync(page, data);
        if (result != null) return HTMLCreationResult.Failure(result);

        result = await SetAutoElementsAsync(page, data);
        if (result != null) return HTMLCreationResult.Failure(result);

        result = await SetVerificationAsync(page, data);
        if (result != null) return HTMLCreationResult.Failure(result);

        result = await SetEtalonsAsync(page, data);
        if (result != null) return HTMLCreationResult.Failure(result);

        result = await SetCheckupsAsync(page, data);
        if (result != null) return HTMLCreationResult.Failure(result);

        result = await SetSignAsync(page, data);
        if (result != null) return HTMLCreationResult.Failure(result);

        result = await SetSpecificAsync(page, data);
        if (result != null) return HTMLCreationResult.Failure(result);

        return HTMLCreationResult.Success(page);
    }

    protected abstract Task<string?> SetSpecificAsync(IPage page, T data);

    private async Task<string?> SetDeviceAsync(IPage page, T data)
    {
        var prop = TypeProps.FirstOrDefault(p => p.Name.Equals("deviceInfo", StringComparison.OrdinalIgnoreCase));
        if (prop == null) return "Data property deviceInfo not found";
        var deviceInfo = prop.GetValue(data)!.ToString()!;

        var result = await page.SetElementTextAsync("#manual_deviceInfo", deviceInfo);
        if (!result) return "Cannot set text in manual_deviceInfo";

        return null;
    }

    private async Task<string?> SetAutoElementsAsync(IPage page, T data)
    {
        var elements = await page.QuerySelectorAllAsync("[id]");
        foreach (var element in elements)
        {
            var id = await element.EvaluateFunctionAsync<string>("el => el.id");
            if (id.StartsWith("setValue_"))
            {
                var propName = id["setValue_".Length..].ToLower();
                var prop = TypeProps.FirstOrDefault(t => t.Name.Equals(propName, StringComparison.OrdinalIgnoreCase));
                if (prop == null)
                {
                    return $"Data property {propName} not found";
                }

                var value = prop.GetValue(data)?.ToString() ?? string.Empty;
                await element.SetElementValueAsync(value);
            }
        }

        return null;
    }

    private async Task<string?> SetVerificationAsync(IPage page, T data)
    {
        var prop = TypeProps.FirstOrDefault(p => p.Name.Equals("verificationsinfo", StringComparison.OrdinalIgnoreCase));
        if (prop == null) return "Data property verificationsinfo not found";
        var verificationsText = prop.GetValue(data)!.ToString()!;

        var (success, remainingText) = await page.SetElementTextExactAsync("#manual_verificationMethodMain", verificationsText);
        if (!success) return "Verifications text cannot be set";
        if (remainingText.Length == 0)
        {
            var removeResult = await page.RemoveElementAsync("#manual_verificationMethodAdditional");
            if (removeResult != null) return removeResult;
            return null;
        }

        var result = await page.SetElementTextAsync("#manual_verificationMethodAdditional", remainingText);
        if (!result) return "Verifications text cannot be set";

        return null;
    }

    private async Task<string?> SetEtalonsAsync(IPage page, T data)
    {
        var prop = TypeProps.FirstOrDefault(p => p.Name.Equals("EtalonsInfo", StringComparison.OrdinalIgnoreCase));
        if (prop == null) return "Data property EtalonsInfo not found";
        var etalonsText = prop.GetValue(data)!.ToString()!;

        var (success, remainingText) = await page.SetElementTextExactAsync("#manual_etalonsMain", etalonsText);
        if (!success) return "Etalons text cannot be set";
        if (remainingText.Length == 0) return null;

        var result = await page.SetElementTextAsync("#manual_etalonsAdditional", remainingText);
        if (!result) return "Etalons text cannot be set";

        return null;
    }

    private async Task<string?> SetCheckupsAsync(IPage page, T data)
    {
        try
        {
            var verificationInfoProp = TypeProps.FirstOrDefault(p => p.Name.Equals("VerificationsInfo", StringComparison.OrdinalIgnoreCase));
            if (verificationInfoProp == null) return "Data property VerificationsInfo not found";

            var checkupsProp = TypeProps.FirstOrDefault(p => p.Name.Equals("Checkups", StringComparison.OrdinalIgnoreCase));
            if (checkupsProp == null) return "Data property Checkups not found";
            var checkups = (Dictionary<string, string>)checkupsProp.GetValue(data)!;

            return await page.EvaluateFunctionAsync<string?>(@"(checkupsData) => {
            const template = document.querySelector('#manual_checkup');
            if (!template) return 'template not found';

            const container = template.parentElement;
            if (!container) return 'container not found';

            const checkups = Object.entries(checkupsData);

            // Clear container and recreate template (to reset numbering)
            container.innerHTML = '';
            container.appendChild(template);

            let checkupNum = 1;

            for (const [key, value] of checkups) {
                // Get or create current element
                const currentElement = checkupNum === 1
                    ? template
                    : template.cloneNode(true);

                // Update elements
                const numEl = currentElement.querySelector('#manual_checkupNum');
                const titleEl = currentElement.querySelector('#manual_checkupTitle');
                const valueEl = currentElement.querySelector('#manual_checkupValue');

                if (!numEl || !titleEl || !valueEl) {
                    return 'Required checkup elements not found';
                }

                numEl.textContent = `${checkupNum++}. `;
                titleEl.textContent = key;
                valueEl.textContent = value;

                // Append if not the first element
                if (checkupNum > 2) {
                    container.appendChild(currentElement);
                }
            }

            return null;
        }", checkups);
        }
        catch (Exception ex)
        {
            return $"Error setting checkups: {ex.Message}";
        }
    }

    private async Task<string?> SetSignAsync(IPage page, T data)
    {
        var prop = TypeProps.FirstOrDefault(p => p.Name == "Worker");
        if (prop == null) return "Data property Worker not found";
        var worker = (string)prop.GetValue(data)!;
        worker = worker.ToLower();

        var strictSignsCount = 12;
        var randomSignIndex = Random.Shared.Next(1, strictSignsCount);
        var key = $"{worker} {randomSignIndex}";

        if (!_signsCache.TryGetValue(key, out var sign))
        {
            var filePath = Path.Combine(_signsDirPath, $"{key}.png");
            if (!File.Exists(filePath)) return $"Подпись сотрудника {worker} не найдена. Или вариантов меньше 12";
            var bytes = await File.ReadAllBytesAsync(filePath);
            var base64 = Convert.ToBase64String(bytes);
            var signBase64 = $"data:image/png;base64,{base64}";
            sign = _signsCache[key] = signBase64;
        }

        try
        {
            var randomTop = Random.Shared.Next(20, 30);
            var randomLeft = Random.Shared.Next(30, 150);
            var randomHeight = Random.Shared.Next(31, 33);

            await page.EvaluateFunctionAsync(@"(signSrc, top, left, height) => {
                const imgElement = document.querySelector('#sign');
                if (imgElement) {
                    imgElement.style.cssText = `top: ${top}px; left: ${left}px; height: ${height}px;`;
                    imgElement.src = signSrc;
                }
            }", sign, randomTop, randomLeft, randomHeight);
        }
        catch (Exception ex)
        {
            return $"Error setting signature image: {ex.Message}";
        }

        return null;
    }
}
