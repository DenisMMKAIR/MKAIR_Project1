using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProjApp.Database.Entities;
using ProjApp.Services;
using WebAPI.Controllers.Requests;

namespace WebAPI.Controllers;

public class InitialVerificationJobController : ApiControllerBase
{
    private readonly InitialVerificationJobService _service;

    public InitialVerificationJobController(InitialVerificationJobService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ServicePaginatedResult<InitialVerificationJob>> GetJobs([Required][FromQuery] GetPaginatedRequest request)
    {
        return await _service.GetJobs(request.PageIndex, request.PageSize);
    }

    [HttpPost]
    public async Task<ServiceResult> AddJob([Required][FromForm] AddJobRequest request)
    {
        return await _service.AddJob((request.Year, request.Month));
    }

    public record AddJobRequest(int Year, int Month);
}
