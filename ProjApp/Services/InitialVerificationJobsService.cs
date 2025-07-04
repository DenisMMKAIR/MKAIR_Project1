using Microsoft.Extensions.Logging;
using ProjApp.BackgroundServices;
using ProjApp.Database;
using ProjApp.Database.Commands;
using ProjApp.Database.Entities;
using ProjApp.Database.SupportTypes;
using ProjApp.Services.ServiceResults;

namespace ProjApp.Services;

public class InitialVerificationJobsService
{
    private readonly ILogger<InitialVerificationJobsService> _logger;
    private readonly ProjDatabase _database;
    private readonly AddInitialVerificationJobCommand _addCommand;
    private readonly EventKeeper _eventKeeper;

    public InitialVerificationJobsService(ILogger<InitialVerificationJobsService> logger,
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

    public async Task<ServiceResult> AddJob(YearMonth date)
    {
        if (date.Year < 2024 || date.Year > DateTime.Now.Year)
        {
            return ServiceResult.Fail("Год от 2024 до текущего");
        }

        if (date.Month < 1 || date.Month > 12)
        {
            return ServiceResult.Fail("Месяц от 1 до 12");
        }

        if (date.Year == DateTime.Now.Year && date.Month > DateTime.Now.Month)
        {
            return ServiceResult.Fail("Месяц не может быть больше текущего");
        }

        var incomingJob = new InitialVerificationJob { Date = date };
        var result = await _addCommand.ExecuteAsync(incomingJob);
        if (result.Error != null) return ServiceResult.Fail(result.Error);
        if (result.NewCount!.Value == 0) return ServiceResult.Fail("Задание уже существует");
        _eventKeeper.Signal(BackgroundEvents.NewInitialVerificationJob);
        return ServiceResult.Success("Задание добавлено");
    }

    public async Task<ServiceResult> DeleteJob(Guid id)
    {
        var job = await _database.InitialVerificationJobs.FindAsync(id);
        if (job == null) return ServiceResult.Fail("Задание не найдено");
        _database.InitialVerificationJobs.Remove(job);
        await _database.SaveChangesAsync();
        return ServiceResult.Success("Задание удалено");
    }
}
