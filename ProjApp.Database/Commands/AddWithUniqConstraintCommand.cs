using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using ProjApp.Database.Entities;

namespace ProjApp.Database.Commands;

public abstract class AddWithUniqConstraintCommand<T> where T : DatabaseEntity
{
    private readonly ProjDatabase _db;
    private readonly ILogger _logger;
    private readonly IEqualityComparer<T> _comparer;

    public AddWithUniqConstraintCommand(ILogger logger,
                                        ProjDatabase db,
                                        IEqualityComparer<T> comparer)
    {
        _logger = logger;
        _db = db;
        _comparer = comparer;
    }

    public virtual async Task<Result> ExecuteAsync(params IReadOnlyList<T> items)
    {
        if (items.Count == 0) return Result.Ok(items, "Список пуст, добавление не требуется");

        // TODO: optimize. Problem: Cant use async because EFCore cant translate _comparer(or _comparer.Equal) to SQL
        // Cant get another approach
        var dbItems = _db.Set<T>()
            .AsEnumerable()
            .Where(e => items.Contains(e, _comparer))
            .ToList();

        var existingItems = new List<T>();
        var newItems = new List<T>();

        foreach (var item in items)
        {
            var existingItem = dbItems.FirstOrDefault(dbItem => _comparer.Equals(item, dbItem));
            if (existingItem != null) existingItems.Add(existingItem);
            else newItems.Add(item);
        }

        if (newItems.Count == 0)
        {
            return Result.Ok(existingItems, "Все элементы уже существуют в базе данных");
        }

        _db.AddRange(newItems);
        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var affectedRows = await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            var savedItems = existingItems.Concat(newItems).ToList();
            return Result.Ok(savedItems, $"Добавлено {newItems.Count} новых элементов");
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
        public string? Message { get; init; }
        public IReadOnlyList<T>? SavedItems { get; init; }
        public string? Error { get; init; }

        private Result() { }

        public static Result Ok(IReadOnlyList<T> savedItems, string message) => new() { SavedItems = savedItems, Message = message };
        public static Result Failed(string error) => new() { Error = error };
    }
}