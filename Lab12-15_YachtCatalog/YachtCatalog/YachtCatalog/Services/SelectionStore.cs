using YachtCatalog.Models;
using Microsoft.Data.Sqlite;
namespace YachtCatalog.Services;

public sealed class SelectionStore(string path)
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    public Task<List<SavedSelection>> GetAllAsync() => RunAsync(db =>
    {
        using var command = db.CreateCommand();
        command.CommandText = "SELECT * FROM Selections ORDER BY Id DESC";
        using var reader = command.ExecuteReader();
        var rows = new List<SavedSelection>();
        while (reader.Read()) rows.Add(Read(reader));
        return rows;
    });

    public Task<SavedSelection?> GetAsync(int id) => RunAsync(db =>
    {
        using var command = db.CreateCommand();
        command.CommandText = "SELECT * FROM Selections WHERE Id=$id";
        command.Parameters.AddWithValue("$id", id);
        using var reader = command.ExecuteReader();
        return reader.Read() ? Read(reader) : null;
    });

    public Task<int> SaveAsync(SavedSelection row)
    {
        row.Validate();
        return RunAsync(db =>
        {
            using var command = db.CreateCommand();
            command.CommandText = row.Id == 0
                ? "INSERT INTO Selections(ProductName,UnitPriceKop,OptionPriceKop,OptionName,Quantity,IncludeOption,Note) VALUES($name,$price,$extra,$option,$qty,$include,$note) RETURNING Id"
                : "UPDATE Selections SET ProductName=$name,UnitPriceKop=$price,OptionPriceKop=$extra,OptionName=$option,Quantity=$qty,IncludeOption=$include,Note=$note WHERE Id=$id RETURNING Id";
            command.Parameters.AddWithValue("$id", row.Id);
            command.Parameters.AddWithValue("$name", row.ProductName);
            command.Parameters.AddWithValue("$price", row.UnitPriceKop);
            command.Parameters.AddWithValue("$extra", row.OptionPriceKop);
            command.Parameters.AddWithValue("$option", row.OptionName);
            command.Parameters.AddWithValue("$qty", row.Quantity);
            command.Parameters.AddWithValue("$include", row.IncludeOption ? 1 : 0);
            command.Parameters.AddWithValue("$note", row.Note);
            var result = command.ExecuteScalar();
            if (result is null) throw new InvalidOperationException("Запис уже видалено. Поверніться до списку.");
            return Convert.ToInt32(result);
        });
    }

    public Task<int> DeleteAsync(int id) => RunAsync(db =>
    {
        using var command = db.CreateCommand();
        command.CommandText = "DELETE FROM Selections WHERE Id=$id";
        command.Parameters.AddWithValue("$id", id);
        return command.ExecuteNonQuery();
    });

    private async Task<T> RunAsync<T>(Func<SqliteConnection, T> operation)
    {
        await _gate.WaitAsync();
        try
        {
            return await Task.Run(() =>
            {
                using var db = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = path }.ToString());
                db.Open();
                using var schema = db.CreateCommand();
                schema.CommandText = """
                    CREATE TABLE IF NOT EXISTS Selections(
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ProductName TEXT NOT NULL,
                        UnitPriceKop INTEGER NOT NULL CHECK(UnitPriceKop>=0),
                        OptionPriceKop INTEGER NOT NULL CHECK(OptionPriceKop>=0),
                        OptionName TEXT NOT NULL,
                        Quantity INTEGER NOT NULL CHECK(Quantity BETWEEN 1 AND 14),
                        IncludeOption INTEGER NOT NULL CHECK(IncludeOption IN (0,1)),
                        Note TEXT NOT NULL CHECK(length(Note)<=200))
                    """;
                schema.ExecuteNonQuery();
                return operation(db);
            });
        }
        finally { _gate.Release(); }
    }

    private static SavedSelection Read(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(0), ProductName = reader.GetString(1),
        UnitPriceKop = reader.GetInt64(2), OptionPriceKop = reader.GetInt64(3),
        OptionName = reader.GetString(4), Quantity = reader.GetInt32(5),
        IncludeOption = reader.GetInt32(6) != 0, Note = reader.GetString(7)
    };
}
