using Dapper;
using Npgsql;

namespace TestApiTask.Data;

public class ElementsRepository(IConfiguration configuration) : IElementsRepository
{
    private const string ConnectionStringName = "Postgres";

    private readonly string _connectionString = configuration.GetConnectionString(ConnectionStringName)
                                                ?? throw new InvalidOperationException(
                                                    $"Не найдена строка подключения '{ConnectionStringName}' в конфигурации.");
    public async Task SaveElementsAsync(IReadOnlyCollection<ElementRecord> elements, CancellationToken cancellationToken)
    {
        if (elements.Count == 0)
        {
            return;
        }

        const string sql = """
            INSERT INTO elements (attribute_value, html)
            VALUES (@AttributeValue, @Html);
            """;

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        var command = new CommandDefinition(
            sql,
            elements,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }
}
