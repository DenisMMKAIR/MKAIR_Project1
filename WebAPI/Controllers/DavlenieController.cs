using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProjApp.Database.SupportTypes;
using ProjApp.Mapping;
using ProjApp.Services;
using ProjApp.Services.ServiceResults;
using WebAPI.Controllers.Requests;

namespace WebAPI.Controllers;

public class DavlenieController : ApiControllerBase
{
    private readonly DavlenieService _service;

    public DavlenieController(DavlenieService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ServicePaginatedResult<Davlenie1VerificationDTO>> GetVerifications([Required][FromQuery] GetPaginatedRequest request, [FromQuery] ManometrFilterQuery filters)
    {
        YearMonth? yearMonth;
        try
        {
            yearMonth = YearMonth.Parse(filters.YearMonth);
        }
        catch (Exception e)
        {
            return ServicePaginatedResult<Davlenie1VerificationDTO>.Fail(e.Message);
        }
        return await _service.GetVerificationsAsync(request.PageIndex, request.PageSize, filters.DeviceTypeNumber, filters.DeviceSerial, yearMonth, filters.Location);
    }

    [HttpPost]
    public async Task<ServiceResult> DeleteVerifications([Required][FromBody] IReadOnlyList<Guid> ids, CancellationToken cancellationToken)
    {
        return await _service.DeleteVerificationAsync(ids, cancellationToken);
    }
}
