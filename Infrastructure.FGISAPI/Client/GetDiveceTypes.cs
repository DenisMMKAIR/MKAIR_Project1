using Infrastructure.FGISAPI.RequestResponse;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Commands;
using ProjDeviceType = ProjApp.Database.Entities.DeviceType;

namespace Infrastructure.FGISAPI.Client;

public partial class FGISAPIClient
{
    [Obsolete(message:"Method need to be rewritten using cache")]
    public async Task<IReadOnlyList<ProjDeviceType>> GetDeviceTypesAsync(IReadOnlyList<string> deviceNumbers)
    {
        const string endpoint = "mit";
        const uint rows = 100;
        var result = new HashSet<ProjDeviceType>(new DeviceTypeUniqComparer());

        foreach (var deviceNumbersChunk in deviceNumbers.SplitBy(rows))
        {
            var search = deviceNumbersChunk.Aggregate((a, c) => $"{a}%20{c}");
            var response = await GetItemListAsync<ListResponse<ProjDeviceType>>(endpoint, search, rows);
            result = [.. result.Union(response.Result.Items)];
        }

        if (deviceNumbers.Count != result.Count)
        {
            _logger.LogError("Количество входящих и полученных типов не совпадает {InCount}-{OutCount}", deviceNumbers.Count, result.Count);
        }

        return [.. result];
    }
}