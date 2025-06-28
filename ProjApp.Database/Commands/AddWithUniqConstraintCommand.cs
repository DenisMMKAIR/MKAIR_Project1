using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public abstract class AddWithUniqConstraintCommand<T> where T : DatabaseEntity
{
    private readonly ProjDatabase _db;
    private readonly ILogger<AddWithUniqConstraintCommand<T>> _logger;
    private readonly IEqualityComparer<T> _comparer;

    public AddWithUniqConstraintCommand(ILogger<AddWithUniqConstraintCommand<T>> logger,
                                        ProjDatabase db,
                                        IEqualityComparer<T> comparer)
    {
        _logger = logger;
        _db = db;
        _comparer = comparer;
    }

    public async Task<Result> ExecuteAsync(T item)
    {
        _db.Add(item);
        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var affectedRows = await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result.Ok((uint)affectedRows, 0);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            await transaction.RollbackAsync();
            var e = (PostgresException)ex.InnerException!;
            _logger.LogError("Не удалось добавить. Обнаружены дубликаты. {Details}. {MessageText}",
                e.Detail, e.MessageText);
            return Result.Failed("Не удалось добавить. Обнаружены дубликаты");
        }
    }

    public async Task<Result> ExecuteAsync(IReadOnlyList<T> items)
    {
        if (items.Count == 0) return Result.Ok(0, 0);

        // TODO: optimize. Problem: Cant use async because EF cant translate _comparer(or _comparer.Equal) to SQL
        // Cant get another approach
        var dbItems = _db.Set<T>()
            .AsEnumerable()
            .Where(e => items.Contains(e, _comparer))
            .ToList();

        var newItems = items.Except(dbItems, _comparer).ToList();

        if (newItems.Count == 0) return Result.Ok(0, (uint)dbItems.Count);

        _db.AddRange(newItems);
        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var affectedRows = await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result.Ok((uint)newItems.Count, (uint)dbItems.Count);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            await transaction.RollbackAsync();
            var e = (PostgresException)ex.InnerException!;
            _logger.LogError("Не удалось добавить. Обнаружены дубликаты. {Details}. {MessageText}",
                e.Detail, e.MessageText);
            return Result.Failed("Не удалось добавить. Обнаружены дубликаты");
        }
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        return ex.InnerException is PostgresException postgresEx &&
               postgresEx.SqlState == PostgresErrorCodes.UniqueViolation;
    }

    public class Result
    {
        public uint NewCount { get; init; }
        public uint DuplicatesCount { get; init; }
        public string? Error { get; init; }

        private Result() { }

        public static Result Ok(uint newCount, uint duplicatesCount) => new() { NewCount = newCount, DuplicatesCount = duplicatesCount };
        public static Result Failed(string error) => new() { Error = error };
    }
}