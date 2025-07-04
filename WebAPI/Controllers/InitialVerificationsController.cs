using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<ServicePaginatedResult<InitialVerificationDto>> GetVerifications([Required][FromQuery] GetPaginatedRequest request, [FromQuery] GetVerificationsRequest? query)
    {
        return await _service.GetInitialVerifications(request.PageIndex, request.PageSize, YearMonth.Parse(query?.VerificationYearMonth));
    }

    public class GetVerificationsRequest
    {
        public string? VerificationYearMonth { get; set; }
    }
}
