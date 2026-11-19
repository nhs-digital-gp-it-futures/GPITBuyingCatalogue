namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure;

public class TestServerFixture : IAsyncLifetime
{
    public string BaseUrl { get; } =
    Environment.GetEnvironmentVariable("TEST_BASE_URL")
    ?? "https://localhost:5001";

    public string DbConnectionString { get; } =
        Environment.GetEnvironmentVariable("TEST_DB_CONNECTION")
        ?? "Server=127.0.0.1,1432;Database=GPITBuyingCatalogue;User=SA;Password=Abc123Abc123;TrustServerCertificate=true";

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;
}
