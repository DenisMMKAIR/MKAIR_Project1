using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjApp.BackgroundServices;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;

namespace ProjApp.Services;

public class InitialVerificationJobService
{
    private readonly ILogger<InitialVerificationJobService> _logger;
    private readonly ProjDatabase _database;
    private readonly AddInitialVerificationJobCommand _addCommand;
    private readonly EventKeeper _eventKeeper;

    public InitialVerificationJobService(ILogger<InitialVerificationJobService> logger,
                                          ProjDatabase database,
                                          AddInitialVerificationJobCommand addCommand,
                                          EventKeeper eventKeeper)
    {
        _logger = logger;
        _database = database;
        _addCommand = addCommand;
        _eventKeeper = eventKeeper;
    }

    public async Task<ServicePaginatedResult<InitialVerificationJob>> GetJobs(int pageIndex, int pageSize)
    {
        var result = await _database.InitialVerificationJobs.ToPaginatedAsync(pageIndex, pageSize);
        return ServicePaginatedResult<InitialVerificationJob>.Success(result);
    }

    public async Task<ServiceResult> AddJob(int year, int month)
    {
        if (year < 2024) return ServiceResult.Fail("Неверно задан год. От 2024");
        if (year > DateTime.Now.Year) return ServiceResult.Fail("Неверно задан год. До текущего года");
        if (month < 1 || month > 12) return ServiceResult.Fail("Неверно задан месяц. От 1 до 12");

        var cur = new DateOnly(year, month, 1);
        var from = new DateOnly(2024, 2, 1);
        var to = DateOnly.FromDateTime(DateTime.Now);
        if (cur < from || cur > to) return ServiceResult.Fail("Неверно задана дата. От 2024.02 До текущей даты");

        var incomingJob = new InitialVerificationJob { Date = $"{year}.{month}", LoadedVerifications = 0 };
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