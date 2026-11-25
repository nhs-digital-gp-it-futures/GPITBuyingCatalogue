using Microsoft.Data.SqlClient;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData.Database;

/// <summary>
/// Creates a fresh completed order for Amend Journey tests to avoid modifying shared baseline test data.
/// </summary>
public static class TestOrderSeeder
{
    private static readonly string ScriptPath =
        Path.Combine(AppContext.BaseDirectory, "TestData", "Files", "CreateCompletedOrder.sql");

    public static async Task<int> CreateCompletedOrderAsync(
        string connectionString,
        string description,
        bool hasAddOnServices)
    {
        var script = await File.ReadAllTextAsync(ScriptPath);

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(script, connection);
        command.Parameters.AddWithValue("@OrderDescription", description);
        command.Parameters.AddWithValue("@HasAddOnServices", hasAddOnServices);

        await using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();

        var orderId = reader.GetInt32(reader.GetOrdinal("OrderId"));

        return (orderId);
    }
}
