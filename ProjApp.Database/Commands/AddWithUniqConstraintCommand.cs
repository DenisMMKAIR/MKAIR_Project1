using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public abstract class AddWithUniqConstraintCommand<T> where T : DatabaseEntity
{
    private readonly ProjDatabase _db;
    private readonly ILogger _logger;
    private readonly IEqualityComparer<T> _uniqComparer;

    public AddWithUniqConstraintCommand(ILogger logger,
                                        ProjDatabase db,
                                        IEqualityComparer<T> uniqComparer)
    {
        _logger = logger;
        _db = db;
        _uniqComparer = uniqComparer;
    }

    public virtual async Task<Result> ExecuteAsync(params IReadOnlyList<T> items)
    {
        if (items.Count == 0) return Result.Ok("Список пуст, добавление не требуется", items, newCount: 0, duplicateCount: 0);

        // TODO: optimize. Problem: Cant use async because EFCore cant translate _comparer(or _comparer.Equal) to SQL
        // Cant get another approach. So we put equality check out of EFCore by UniqComparer
        var dbItems = _db.Set<T>()
            .AsEnumerable()
            .Where(e => items.Contains(e, _uniqComparer))
            .ToList();

        var existingItems = new List<T>();
        var newItems = new List<T>();

        foreach (var item in items)
        {
            var existingItem = dbItems.FirstOrDefault(dbItem => _uniqComparer.Equals(item, dbItem));
            if (existingItem != null) existingItems.Add(existingItem);
            else newItems.Add(item);
        }

        if (newItems.Count == 0)
        {
            return Result.Ok("Все записи уже существуют в базе данных", existingItems, newCount: 0, (uint)existingItems.Count);
        }

        _db.AddRange(newItems);
        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var affectedRows = await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            var savedItems = existingItems.Concat(newItems).ToList();

            return Result.Ok($"Добавлено новых элементов {newItems.Count}. Отсеяно дубликатов {existingItems.Count}",
                            savedItems,
                            (uint)newItems.Count,
                            (uint)existingItems.Count);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            await transaction.RollbackAsync();
            var e = (PostgresException)ex.InnerException!;
            _logger.LogError("Не удалось добавить. Обнаружены дубликаты. {Details}. {MessageText}",
                e.Detail, e.MessageText);
            return Result.Failed("Не удалось добавить. Обнаружены дубликаты");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Не удалось добавить. {Message}", ex.Message);
            return Result.Failed(ex.Message);
        }
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        return ex.InnerException is PostgresException postgresEx &&
               postgresEx.SqlState == PostgresErrorCodes.UniqueViolation;
    }

    public class Result
    {
        public string? Message { get; init; }
        public IReadOnlyList<T>? Items { get; init; }
        public uint? NewCount { get; init; }
        public uint? DuplicateCount { get; init; }
        public string? Error { get; init; }

        private Result() { }

        public static Result Ok(string message, IReadOnlyList<T> items, uint newCount, uint duplicateCount) => new()
        {
            Message = message,
            Items = items,
            NewCount = newCount,
            DuplicateCount = duplicateCount
        };
        public static Result Failed(string error) => new() { Error = error };
    }
}