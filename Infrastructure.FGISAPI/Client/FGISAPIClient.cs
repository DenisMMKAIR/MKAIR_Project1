using Infrastructure.FGIS.Database;
using Infrastructure.FGIS.Database.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjApp.Database.Entities;
using ProjApp.Database.SupportTypes;
using ProjApp.InfrastructureInterfaces;
using FIGSDeviceType = Infrastructure.FGIS.Database.Entities.DeviceType;
using FIGSEtalon = Infrastructure.FGIS.Database.Entities.Etalon;
using ProjDevice = ProjApp.Database.Entities.Device;
using ProjDeviceType = ProjApp.Database.Entities.DeviceType;
using ProjEtalon = ProjApp.Database.Entities.Etalon;

namespace Infrastructure.FGISAPI.Client;

public class FGISAPIClient : IFGISAPI
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly VerificationsClient _verificationsClient;
    private readonly EtalonsClient _etalonsClient;
    private readonly DeviceTypeClient _deviceTypesClient;

    public FGISAPIClient(ILogger<FGISAPIClient> logger, IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;

        var httpQueueManager = new HTTPQueueManager(logger);
        var httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(2) };
        httpClient.DefaultRequestHeaders.Add("User-Agent", "FGIS_API1");

        _verificationsClient = new VerificationsClient(logger, httpQueueManager, httpClient);
        _etalonsClient = new EtalonsClient(logger, httpQueueManager, httpClient);
        _deviceTypesClient = new DeviceTypeClient(logger, httpQueueManager, httpClient);
    }

    public Task<IReadOnlyList<ProjDeviceType>> GetDeviceTypesAsync(IReadOnlyList<string> deviceNumbers)
    {
        throw new NotImplementedException();
    }

    public async Task<FGISAPIResult> GetInitialVerificationsAsync(YearMonth date)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FGISDatabase>();

        var monthResult = await db.MonthResults.FindAsync(date);
        if (monthResult == null)
        {
            monthResult = new MonthResult(date);
            await db.MonthResults.AddAsync(monthResult);
            await db.SaveChangesAsync();
        }

        var vrfsResult = await _verificationsClient.GetVerificationsAsync(db, monthResult);
        if (vrfsResult.Error != null) return FGISAPIResult.Fail(vrfsResult.Error);

        var etalonsResult = await _etalonsClient.GetEtalonsAsync(db, monthResult, vrfsResult.Data!, vrfsResult.AllDataFetched!.Value);
        if (etalonsResult.Error != null) return FGISAPIResult.Fail(etalonsResult.Error);

        var deviceTypesResult = await _deviceTypesClient.GetDeviceTypesAsync(db, monthResult, vrfsResult.Data!, etalonsResult.AllDataFetched!.Value);
        if (deviceTypesResult.Error != null) return FGISAPIResult.Fail(deviceTypesResult.Error);

        var (successVrfs, failedVrfs) = MapAllVerifications(vrfsResult.Data!, etalonsResult.Data!, deviceTypesResult.Data!);
        return FGISAPIResult.Success(successVrfs, failedVrfs);
    }

    private static (IReadOnlyList<SuccessInitialVerification>, IReadOnlyList<FailedInitialVerification>) MapAllVerifications(
        IReadOnlyList<VerificationWithEtalon> fgisVerification,
        IReadOnlyList<FIGSEtalon> fgisEtalons,
        IReadOnlyList<FIGSDeviceType> fgisDeviceTypes)
    {

        var goodVrfs = fgisVerification
            .Where(v => v.VriInfo.Applicable != null)
            .ToList();

        var failedVrfs = fgisVerification
            .Where(v => v.VriInfo.Inapplicable != null)
            .ToList();

        static SuccessInitialVerification MapGood(VerificationWithEtalon v, ProjDeviceType deviceType, ProjDevice device, IReadOnlyList<ProjEtalon> etalons)
        {
            return new SuccessInitialVerification
            {
                DeviceTypeNumber = deviceType.Number,
                DeviceSerial = device.Serial,
                Owner = v.VriInfo.MiOwner,
                VerificationTypeName = v.VriInfo.DocTitle,
                VerificationDate = v.VriInfo.VrfDate,
                VerifiedUntilDate = v.VriInfo.ValidDate!.Value,
                Device = device,
                Etalons = [.. etalons],
            };
        }

        static FailedInitialVerification MapFailed(VerificationWithEtalon v, ProjDeviceType deviceType, ProjDevice device, IReadOnlyList<ProjEtalon> etalons)
        {
            return new FailedInitialVerification
            {
                DeviceTypeNumber = deviceType.Number,
                DeviceSerial = device.Serial,
                Owner = v.VriInfo.MiOwner,
                VerificationTypeName = v.VriInfo.DocTitle,
                VerificationDate = v.VriInfo.VrfDate,
                Device = device,
                Etalons = [.. etalons],
                FailedDocNumber = v.VriInfo.Inapplicable!.NoticeNum,
            };
        }

        return (MapToInitialVerifications(goodVrfs, fgisEtalons, fgisDeviceTypes, MapGood),
                MapToInitialVerifications(failedVrfs, fgisEtalons, fgisDeviceTypes, MapFailed));
    }

    private static IReadOnlyList<T> MapToInitialVerifications<T>(
        IReadOnlyList<VerificationWithEtalon> FGISVerifications,
        IReadOnlyList<FIGSEtalon> FGISEtalons,
        IReadOnlyList<FIGSDeviceType> FGISDeviceTypes,
        Func<VerificationWithEtalon, ProjDeviceType, ProjDevice, IReadOnlyList<ProjEtalon>, T> map)
        where T : IInitialVerification
    {
        var projEtalons = FGISEtalons
            .SelectMany(FGISEtalonToProjectEtalon)
            .ToArray();

        var projDeviceTypes = FGISDeviceTypes
            .Select(FGISDeviceTypeToProjectDeviceType)
            .ToArray();

        var projVerifications = FGISVerifications
            .Select(v =>
            {
                var deviceType = projDeviceTypes.Single(dt => dt.Number == v.MiInfo.SingleMI.MitypeNumber);

                var device = new ProjDevice { DeviceType = deviceType, DeviceTypeNumber = deviceType.Number, Serial = v.MiInfo.SingleMI.ManufactureNum, ManufacturedYear = (uint)v.MiInfo.SingleMI.ManufactureYear, Modification = v.MiInfo.SingleMI.Modification };

                var mietaNumbers = v.Means.Mieta
                    .Select(mi => mi.RegNumber)
                    .Distinct()
                    .ToList();

                var etalons = projEtalons
                    .Where(e => v.VriInfo.VrfDate >= e.Date && v.VriInfo.VrfDate <= e.ToDate)
                    .Where(e => mietaNumbers.Contains(e.Number))
                    .DistinctBy(e => e.Number)
                    .ToList();

                if (etalons.Count == 0 || etalons.Count < mietaNumbers.Count)
                {
                    throw new Exception($"Не найдены или найдены частично эталоны поверки [НомерТипа]{deviceType.Number} [СерийныйУстройства]{device.Serial} [Дата]{v.VriInfo.VrfDate}. Номер в базе фгис: {v.Vri_id}");
                }

                return map(v, deviceType, device, etalons);
            })
            .ToArray();

        return projVerifications;
    }

    private static IEnumerable<ProjEtalon> FGISEtalonToProjectEtalon(FIGSEtalon e)
    {
        return e.CResults
            .Select(cr =>
            {
                var fullInfo = $"{e.Number}; {e.MiType_Num}; {e.MiType}; {e.MiNotation}; {e.Modification}; {e.Factory_Num}; {e.Year}; {e.RankCode}; {e.RankClass}; {e.Schematitle}; свидетельство о поверке № {cr.Result_Docnum}; действительно до {cr.Valid_Date};";

                return new ProjEtalon
                {
                    Number = e.Number,
                    Date = DateOnly.Parse(cr.Verification_Date),
                    ToDate = DateOnly.Parse(cr.Valid_Date),
                    FullInfo = fullInfo,
                };
            });
    }

    private static ProjDeviceType FGISDeviceTypeToProjectDeviceType(FIGSDeviceType dt)
    {
        return new ProjDeviceType
        {
            Number = dt.Number,
            Title = dt.Title,
            Notation = string.Join("; ", dt.Notation),
            MethodUrls = dt.MethUrls?.ToArray(),
            SpecUrls = dt.SpecUrls?.ToArray(),
            Manufacturers = dt.Manufacturers?.ToArray(),
        };
    }
}
