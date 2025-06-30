using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProjApp.Database.Entities;
using ProjApp.Services;
using ProjApp.Services.ServiceResults;
using WebAPI.Controllers.Requests;

namespace WebAPI.Controllers;

public class PendingManometrVerificationsController : ApiControllerBase
{
    private readonly ILogger<PendingManometrVerificationsController> _logger;
    private readonly PendingManometrVerificationsService _verificationService;

    public PendingManometrVerificationsController(ILogger<PendingManometrVerificationsController> logger,
        PendingManometrVerificationsService verificationService)
    {
        _logger = logger;
        _verificationService = verificationService;
    }

    [HttpPost]
    public async Task<ServiceResult> AcceptExcelVerifications([Required][FromForm] ExcelVerificationsRequest request, CancellationToken cancellationToken)
    {
        var (file, sheetName, dataRange, location) = request;
        using var memory = new MemoryStream();
        await file.CopyToAsync(memory, cancellationToken);
        memory.Position = 0;
        var result = await _verificationService.ProcessIncomingExcelAsync(memory, file.FileName, sheetName, dataRange, location);
        return result;
    }

    [HttpGet]
    public async Task<ServicePaginatedResult<PendingManometrVerification>> GetPandingVerificationsPaginated([FromQuery] GetPaginatedRequest request)
    {
        return await _verificationService.GetPaginatedAsync(request.PageIndex, request.PageSize);
    }
}
