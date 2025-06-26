using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjApp.BackgroundServices;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;

namespace ProjApp.Services;

public class InitiailVerificationJobService
{
    private readonly ILogger<InitiailVerificationJobService> _logger;
    private readonly ProjDatabase _database;
    private readonly AddInitiailVerificationJobCommand _addCommand;
    private readonly EventKeeper _eventKeeper;

    public InitiailVerificationJobService(ILogger<InitiailVerificationJobService> logger,
                                          ProjDatabase database,
                                          AddInitiailVerificationJobCommand addCommand,
                                          EventKeeper eventKeeper)
    {
        _logger = logger;
        _database = database;
        _addCommand = addCommand;
        _eventKeeper = eventKeeper;
    }

    public async Task<ServicePaginatedResult<InitiailVerificationJob>> GetJobs(int pageIndex, int pageSize)
    {
        var result = await _database.InitiailVerificationJobs.ToPaginatedAsync(pageIndex, pageSize);
        return ServicePaginatedResult<InitiailVerificationJob>.Success(result);
    }

    public async Task<ServiceResult> AddJob(int year, int month)
    {
        var cur = new DateOnly(year, month, 1);
        var end = DateOnly.FromDateTime(DateTime.Now);
        var begin = new DateOnly(2024, 2, 1);

        if (cur < begin || cur > end) return ServiceResult.Fail("Неверно задана дата. От 2024.02.01 до сегодня");

        var incomingJob = new InitiailVerificationJob { Date = $"{year}.{month}", LoadedVerifications = 0 };
        var result = await _addCommand.ExecuteAsync(incomingJob);
        if (result.Error != null)
        {
            _logger.LogError("{Msg}", result.Error);
            return ServiceResult.Fail(result.Error);
        }
        _logger.LogInformation("Задание добавлено");
        _eventKeeper.Signal(BackgroundEvents.NewInitialVerificationJob);
        return ServiceResult.Success("Задание добавлено");
    }
}