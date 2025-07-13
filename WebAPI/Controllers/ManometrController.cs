using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<ServicePaginatedResult<Manometr1VerificationDto>> GetVerifications([Required][FromQuery] GetPaginatedRequest request)
    {
        return await _service.GetVerificationsAsync(request.PageIndex, request.PageSize);
    }

    [HttpGet]
    public async Task<ServiceResult> ExportToPdf([Required] IReadOnlyList<Guid> ids)
    {
        return await _service.ExportToPdfAsync(ids);
    }

    //     public async Task<ServicePaginatedResult<Manometr1VerificationDto>> GetVerificationsAsync(int page, int pageSize)
    //    public async Task<ServiceResult> ExportToPdfAsync(IReadOnlyList<Guid> ids)
}
