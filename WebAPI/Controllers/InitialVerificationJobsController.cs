using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProjApp.Database.Entities;
using ProjApp.Services;
using ProjApp.Services.ServiceResults;
using WebAPI.Controllers.Requests;

namespace WebAPI.Controllers;

public class InitialVerificationJobsController : ApiControllerBase
{
    private readonly InitialVerificationJobsService _service;

    public InitialVerificationJobsController(InitialVerificationJobsService service)
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

    [HttpDelete]
    public async Task<ServiceResult> DeleteJob([Required][FromQuery] Guid id)
    {
        return await _service.DeleteJob(id);
    }
}
