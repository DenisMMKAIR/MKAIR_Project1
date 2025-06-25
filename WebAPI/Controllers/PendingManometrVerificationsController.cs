using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProjApp.Database;
using ProjApp.Database.Entities;
using ProjApp.Services;
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

    /// <summary>
    /// Принимает поверки виде excel документа
    /// </summary>
    /// <param name="file">Файл excel</param>
    /// <param name="sheetName">Название листа</param>
    /// <param name="dataRange">Диапазон данных включая заголовки. Например A1:B2</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ServiceResult> AcceptExcelVerifications([Required][FromForm] ExcelVerificationsRequest request, CancellationToken cancellationToken)
    {
        var (file, sheetName, dataRange, location) = request;
        using var memory = new MemoryStream();
        await file.CopyToAsync(memory, cancellationToken);
        memory.Position = 0;
        _logger.LogDebug("Получен файл для обработки: {FileName}", file.FileName);
        var result = await _verificationService.ProcessIncomingExcelAsync(memory, file.FileName, sheetName, dataRange, location);
        return result;
    }

    [HttpGet]
    public async Task<ServicePaginatedResult<PendingManometrVerification>> GetPandingVerificationsPaginated([FromQuery] GetPaginatedRequest request)
    {
        return await _verificationService.GetPaginatedAsync(request.PageIndex, request.PageSize);
    }

    public record ExcelVerificationsRequest(IFormFile File, string SheetName, string DataRange, DeviceLocation DeviceLocation);
}
