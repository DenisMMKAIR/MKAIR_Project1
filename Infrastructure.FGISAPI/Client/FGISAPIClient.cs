using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Extensions.Logging;
using ProjApp.InfrastructureInterfaces;

namespace Infrastructure.FGISAPI.Client;

public partial class FGISAPIClient : IFGISAPI
{
    private readonly ILogger<FGISAPIClient> _logger;
    private readonly HTTPQueueManager _httpQueueManager;
    private readonly HttpClient _httpClient;

    public FGISAPIClient(ILogger<FGISAPIClient> logger)
    {
        _logger = logger;
        _httpQueueManager = new(logger);
        _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(2) };
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "FGIS_API1");
    }

    private async Task<T> GetItemListAsync<T>(string endpoint, string search, uint? rows = null)
    {
        var query = BuildQuery(search, rows: rows);
        return await RequestBaseAsync<T>($"/{endpoint}", query);
    }

    private async Task<T> GetItemListAsync<T>(string endpoint, string query)
    {
        return await RequestBaseAsync<T>($"/{endpoint}", query);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="search">Задает подстроку поиска. Подстановочный символ «*» используется для любых символов (в том числе их отсутствия). Если надо найти записи, в которых любой из вышеперечисленных атрибутов начинается с заданной строки, надо использовать: search=строка_поиска* Если надо найти подстроку, то надо использовать: search=*строка_поиска* Если необходимо выполнить поиск по нескольким значениям, то в параметре search необходимо передать строки, разделенные пробелом (%20): search=строка_поиска1%20строка_поиска2%20строка_поиска3 Если необходимо найти значение, включающее пробел, то вместо пробела следует использовать подстановочный символ «?» (обозначающий любой символ), и проверить, что возвращенные элементы содержат пробел на месте подстановочного символа: search=до_пробела?после_пробела Если необходимо найти значение, включающее подстановочный символ «*» или «?», то необходимо использовать тот же подход, что применяется при поиске значения, содержащего пробел.</param>
    /// <param name="sort">Задает атрибут и порядок сортировки, разделенные пробелом (для обозначения пробела в составе значения параметра URL также допускается использование символа + и %20). Перечень атрибутов для сортировки по каждому разделу ФИФ ОЕИ приведен в разделе 3.3. Порядок сортировки задается как asc (по возрастанию), либо desc (по убыванию). Например, для сортировки утвержденных типов СИ по номеру в реестре в порядке убывания необходимо использовать: sort=number+desc </param>
    /// <param name="start">Задает порядковый номер начальной записи (не более 99 999); значение по умолчанию 0</param>
    /// <param name="rows">Количество элементов на страницу (не более 100); значение по умолчанию 10</param>
    /// <param name="request">Задает фильтр по атрибуту. Например, для поиска сведений о результатах поверки средств измерений с регистрационным номером типа СИ «38760-08» и заводским номером «0 8109565 09» необходимо использовать: mit_number=38760-08&mi_number=0 8109565 09 Для строковых атрибутов допускается использование подстановочных символов «*» и «?» по аналогии с параметром search. Фильтр по датам задается в формате yyyy-MM-dd. Поддерживается поиск по диапазонам дат и целых чисел: для начала диапазона в конце кода атрибута необходимо добавить суффикс «_start», для конца диапазона – «_end». Например, для поиска сведений о результатах поверки средств измерений с датой поверки от 01.09.2022 по 31.09.2022 необходимо использовать: verification_date_start=2022-09- 01&verification_date_end=2022-09-31 Допускается указание в диапазонах как одновременно и начала, и конца диапазона, так и только начала диапазона либо только конца диапазона. Для поиска по логическим значениям необходимо использовать true либо false</param>
    /// <returns>Возвращает строку запроса. Может быть пустой</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static string BuildQuery(string? search = null, string? sort = null, uint? start = null, uint? rows = null)
    {
        StringBuilder? queryBuilder;

        if (search != null || sort != null || start != null || rows != null)
        {
            queryBuilder = new();
            queryBuilder.Append('?');
            if (!string.IsNullOrEmpty(search)) queryBuilder.Append($"search={search}&");
            if (!string.IsNullOrEmpty(sort)) queryBuilder.Append($"sort={sort}&");
            if (start.HasValue)
            {
                if (start > 99.999) throw new ArgumentOutOfRangeException($"{nameof(start)}", "значение должно быть не больше 99.999");
                queryBuilder.Append($"start={start}&");
            }
            if (rows.HasValue)
            {
                if (rows < 1 || rows > 100) throw new ArgumentOutOfRangeException($"{nameof(rows)}", "значение должно быть в диапазоне от 1 до 100");
                queryBuilder.Append($"rows={rows}&");
            }
            queryBuilder.Length--;
        }
        else
        {
            queryBuilder = null;
        }

        var query = queryBuilder?.ToString() ?? string.Empty;

        return query;
    }

    private async Task<TResponse> RequestBaseAsync<TResponse>(string uri, string query)
    {
        const int tryCount = 10;

        for (int i = 0; i < tryCount; i++)
        {
            try
            {
                await _httpQueueManager.WaitForQueue();
                _logger.LogInformation("Запрос {APIUrl}{uri}{query}", AppConfig.APIUrl, uri, query);
                return await _httpClient.GetFromJsonAsync<TResponse>($"{AppConfig.APIUrl}{uri}{query}", AppConfig.JsonOptions) ??
                    throw new SerializationException("Ошибка при десериализации ответа от сервера");
            }
            catch (HttpRequestException ex)
            {
                if (i < tryCount - 1)
                {
                    _logger.LogWarning("Попытка {Try} из {TryCount} не удалась. {ExMsg}. Повторная попытка", i + 1, ex.Message, tryCount);
                }
                else
                {
                    throw;
                }
            }
        }

        // TODO: Fix how we can get here
        throw new NotImplementedException("Unreachable code");
    }
}
