namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure;

public class TestServerFixture : IAsyncLifetime
{
    public string BaseUrl { get; } =
        Environment.GetEnvironmentVariable("TEST_BASE_URL")
        ?? "https://localhost:5001";

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;
}
