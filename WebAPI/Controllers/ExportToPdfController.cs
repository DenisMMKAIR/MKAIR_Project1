using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProjApp.Database.EntitiesStatic;
using ProjApp.Services;
using ProjApp.Services.ServiceResults;

namespace WebAPI.Controllers;

public class ExportToPdfController : ApiControllerBase
{
    private readonly ExportToPDFService _service;

    public ExportToPdfController(ExportToPDFService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ServiceResult> ExportToPdf([Required][FromBody] ExportToPdfRequest request, CancellationToken cancellationToken)
    {
        (var group, var ids) = request;
        return await _service.ExportToPdfAsync(group, ids, cancellationToken);
    }

    [HttpGet]
    public async Task<ServiceResult> ExportAllToPdf([Required][FromQuery] VerificationGroup group, CancellationToken cancellationToken)
    {
        return await _service.ExportAllToPdfAsync(group, cancellationToken);
    }

    [HttpPost]
    public async Task<ServiceResult> ExportByExcelToPDF([Required][FromForm] ExportByExcelToPDFRequest request, CancellationToken cancellationToken)
    {
        var (group, excelFile, sheetName, dataRange) = request;
        var mem = new MemoryStream();
        excelFile.CopyTo(mem);
        mem.Position = 0;
        return await _service.ExportToPdfAsync(group, excelFile.FileName, mem, sheetName, dataRange, cancellationToken);
    }

    public record ExportToPdfRequest(VerificationGroup Group, IReadOnlyList<Guid> Ids);
    public record ExportByExcelToPDFRequest(VerificationGroup Group, IFormFile File, string SheetName, string DataRange);
}
