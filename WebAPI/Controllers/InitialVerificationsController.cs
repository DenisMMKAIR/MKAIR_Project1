using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProjApp.Database.EntitiesStatic;
using ProjApp.Database.SupportTypes;
using ProjApp.Mapping;
using ProjApp.Services;
using ProjApp.Services.ServiceResults;
using WebAPI.Controllers.Requests;

namespace WebAPI.Controllers;

public class InitialVerificationsController : ApiControllerBase
{
    private readonly InitialVerificationService _service;

    public InitialVerificationsController(InitialVerificationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ServicePaginatedResult<InitialVerificationDto>> GetVerifications([Required][FromQuery] GetPaginatedRequest request, [FromQuery] GetVerificationsFilters? filters)
    {
        YearMonth? yearMonthFilter;
        try
        {
            yearMonthFilter = YearMonth.Parse(filters?.YearMonth);
        }
        catch (Exception e)
        {
            return ServicePaginatedResult<InitialVerificationDto>.Fail(e.Message);
        }

        return await _service.GetInitialVerifications(request.PageIndex, request.PageSize, yearMonthFilter, filters?.TypeInfo, filters?.Location);
    }

    [HttpPatch]
    public Task<ServiceResult> SetValues([Required] IFormFile excelFile, [Required][FromForm] SetValuesRequest request)
    {
        var mem = new MemoryStream();
        excelFile.CopyTo(mem);
        mem.Position = 0;
        return _service.SetValues(mem, request);
    }
}

public class GetVerificationsFilters
{
    public string? YearMonth { get; init; }
    public string? TypeInfo { get; init; }
    public DeviceLocation? Location { get; init; }
}
