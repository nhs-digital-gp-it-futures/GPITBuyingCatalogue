using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData.Builders;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Tests.Ordering;

public class OrderTests : BaseTest
{
    public OrderTests(TestServerFixture fixture, ITestOutputHelper output)
        : base(fixture, output)
    {
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task CatalogueSolutionOrder()
    {
        var data = new OrderTestDataBuilder()
            .WithBaseUrl(Fixture.BaseUrl)
            .Build();

        await orderPages.LoginAsync(data);
        await orderPages.CreateNewOrderAsync(data);
        await orderPages.StepOnePrepareOrderAsync(data);
        await orderPages.StepTwoAddSolutionsAndServicesAsync(data);
        await orderPages.StepThreeCompleteContractAsync();
        await orderPages.StepFourReviewAndCompleteOrderAsync();
    }
}
