using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProjApp.Database.EntitiesStatic;
using ProjApp.Database.SupportTypes;
using ProjApp.Mapping;
using ProjApp.Services;
using ProjApp.Services.ServiceResults;
using WebAPI.Controllers.Requests;

namespace WebAPI.Controllers;

public class VerificationsController : ApiControllerBase
{
    private readonly VerificationsService _service;

    public VerificationsController(VerificationsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ServicePaginatedResult<SuccessInitialVerificationDto>> GetInitialVerifications([Required][FromQuery] GetPaginatedRequest request, [FromQuery] VerificationsFiltersQuery filters)
    {
        YearMonth? yearMonthFilter;
        try
        {
            yearMonthFilter = YearMonth.Parse(filters.YearMonth);
        }
        catch (Exception e)
        {
            return ServicePaginatedResult<SuccessInitialVerificationDto>.Fail(e.Message);
        }

        return await _service.GetInitialVerifications(request.PageIndex, request.PageSize, filters.DeviceTypeNumber, yearMonthFilter, filters.TypeInfo, filters.Location);
    }

    [HttpGet]
    public async Task<ServicePaginatedResult<SuccessVerificationDto>> GetVerifications([Required][FromQuery] GetPaginatedRequest request, [FromQuery] VerificationsFiltersQuery filters)
    {
        YearMonth? yearMonthFilter;
        try
        {
            yearMonthFilter = YearMonth.Parse(filters.YearMonth);
        }
        catch (Exception e)
        {
            return ServicePaginatedResult<SuccessVerificationDto>.Fail(e.Message);
        }

        return await _service.GetVerifications(request.PageIndex, request.PageSize, filters.DeviceTypeNumber, yearMonthFilter, filters.TypeInfo, filters.Location);
    }

    [HttpPatch]
    public Task<ServiceResult> SetValues([Required] IFormFile excelFile, [Required][FromForm] SetValuesRequest request)
    {
        var mem = new MemoryStream();
        excelFile.CopyTo(mem);
        mem.Position = 0;
        return _service.SetValues(mem, request);
    }

    [HttpPatch]
    public Task<ServiceResult> SetVerificationNum([Required] IFormFile excelFile, [Required][FromQuery] string sheetName, [Required][FromForm] string dataRange)
    {
        var mem = new MemoryStream();
        excelFile.CopyTo(mem);
        mem.Position = 0;
        return _service.SetVerificationNum(mem, sheetName, dataRange);
    }
}

public class VerificationsFiltersQuery
{
    public string? DeviceTypeNumber { get; init; }
    public string? YearMonth { get; init; }
    public string? TypeInfo { get; init; }
    public DeviceLocation? Location { get; init; }
}
