namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure
{
    public class TestSettings
    {
        public bool Headless { get; init; } = false;
        public bool IgnoreHttpsErrors { get; init; } = true;

        public static TestSettings FromEnvironment() => new()
        {
            Headless = Environment.GetEnvironmentVariable("PLAYWRIGHT_HEADLESS") == "true",
            IgnoreHttpsErrors = true
        };
    }
}
