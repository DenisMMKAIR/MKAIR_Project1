using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProjApp.Database.Entities;
using ProjApp.Services;
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
    public async Task<ServicePaginatedResult<InitialVerification>> GetVerifications([Required][FromQuery] GetPaginatedRequest request)
    {
        return await _service.GetInitialVerifications(request.PageIndex, request.PageSize);
    }
}
