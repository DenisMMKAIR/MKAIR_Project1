using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProjApp.Database.SupportTypes;
using ProjApp.Services;
using ProjApp.Services.ServiceResults;
using WebAPI.Controllers.Requests;

namespace WebAPI.Controllers;

public class ManometrController : ApiControllerBase
{
    private readonly ManometrService _service;

    public ManometrController(ManometrService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ServicePaginatedResult<Manometr1VerificationDto>> GetVerifications([Required][FromQuery] GetPaginatedRequest request, [FromQuery] ManometrFilterQuery filters)
    {
        YearMonth? yearMonth;
        try
        {
            yearMonth = YearMonth.Parse(filters.YearMonth);
        }
        catch (Exception e)
        {
            return ServicePaginatedResult<Manometr1VerificationDto>.Fail(e.Message);
        }
        return await _service.GetVerificationsAsync(request.PageIndex, request.PageSize, filters.DeviceTypeNumber, filters.DeviceSerial, yearMonth);
    }

    [HttpPost]
    public async Task<ServiceResult> ExportToPdf([Required][FromBody] IReadOnlyList<Guid> ids, CancellationToken cancellationToken)
    {
        return await _service.ExportToPdfAsync(ids, cancellationToken);
    }

    [HttpPost]
    public async Task<ServiceResult> ExportAllToPdf(CancellationToken cancellationToken)
    {
        return await _service.ExportAllToPdfAsync(cancellationToken);
    }

    [HttpPost]
    public async Task<ServiceResult> DeleteVerifications([Required][FromBody] IReadOnlyList<Guid> ids, CancellationToken cancellationToken)
    {
        return await _service.DeleteVerificationAsync(ids, cancellationToken);
    }
}

public class ManometrFilterQuery
{
    public string? DeviceTypeNumber { get; init; }
    public string? DeviceSerial { get; init; }
    public string? YearMonth { get; init; }
}