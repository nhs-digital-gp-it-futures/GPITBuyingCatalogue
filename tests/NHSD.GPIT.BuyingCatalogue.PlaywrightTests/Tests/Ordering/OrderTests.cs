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

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task AssociatedServiceOnlyOrder_SomethingElse()
    {
        var data = new AssociatedServiceTestDataBuilder()
            .WithBaseUrl(Fixture.BaseUrl)
            .WithServiceCategory("Something else")
            .WithSupplier("EMIS Health")
            .WithCatalogueSolutionAndAssociatedService("Emis Web GP", "Engineering")
            .WithFundingFilter("Engineering")
            .Build();

        await orderPages.LoginAsync(data);
        await orderPages.CreateNewAssociatedServiceOrderAsync(data);
        await orderPages.StepOnePrepareAssociatedServiceOrderAsync(data);
        await orderPages.StepTwoAddAssociatedServiceAsync(data);
        await orderPages.StepThreeCompleteAssociatedServiceContractAsync();
        await orderPages.StepFourReviewAndCompleteOrderAsync();
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task AssociatedServiceOnlyOrder_Merger()
    {
        var data = new AssociatedServiceTestDataBuilder()
            .WithBaseUrl(Fixture.BaseUrl)
            .WithServiceCategory("Merger")
            .WithSupplier("EMIS Health", isMerger: true)
            .WithCatalogueSolutionForMerger("Video Consult")
            .WithPracticesAndRecipientToBeMerged(
                new[] { "BANKFIELD SURGERY", "BEECHWOOD MEDICAL CENTRE", "BRIG ROYD SURGERY" },
                "BANKFIELD SURGERY")
            .WithFundingFilter("Merger")
            .Build();

        await orderPages.LoginAsync(data);
        await orderPages.CreateNewAssociatedServiceOrderAsync(data);
        await orderPages.StepOnePrepareAssociatedServiceOrderAsync(data);
        await orderPages.StepTwoAddAssociatedServiceAsync(data);
        await orderPages.StepThreeCompleteAssociatedServiceContractAsync();
        await orderPages.StepFourReviewAndCompleteOrderAsync();
    }
}
