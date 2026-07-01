using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData.Builders;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Tests.Ordering;

public class OrderTests : BaseTest
{
    private const string SolutionName = "Emis Web GP";
    private const string AlternativeSolution = "Anywhere Consult";
    private const string AssociatedService = "Engineering";
    private const string AdditionalService = "Automated Arrivals";
    private const string NewAssociatedService = "Installation";
    private const string NewAdditionalService = "Document Management";
    private const string CsvFileName = "valid_service_recipients.csv";

    private static readonly string[] MergerPractices =
    {
        "BANKFIELD SURGERY",
        "BEECHWOOD MEDICAL CENTRE",
        "BRIG ROYD SURGERY"
    };

    public OrderTests(TestServerFixture fixture, ITestOutputHelper output)
        : base(fixture, output)
    {
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task CatalogueSolutionOnly()
    {
        await orderPages.LoginAsync();
        await orderPages.CreateNewOrderAsync();
        await orderPages.StepOnePrepareOrderAsync();
        await orderPages.StepTwoAddSolutionsAndServicesAsync(solutionName: SolutionName);
        await orderPages.StepTwoDeliveryAndFundingAsync();
        await orderPages.StepThreeCompleteContractAsync();
        await orderPages.StepFourReviewAndCompleteOrderAsync();
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task CatalogueSolutionWithAssociatedService()
    {
        await orderPages.LoginAsync();
        await orderPages.CreateNewOrderAsync();
        await orderPages.StepOnePrepareOrderAsync();
        await orderPages.StepTwoAddSolutionsAndServicesAsync(solutionName: SolutionName, associatedService: AssociatedService);
        await orderPages.StepTwoDeliveryAndFundingAsync(associatedService: AssociatedService);
        await orderPages.StepThreeCompleteContractAsync(associatedService: AssociatedService);
        await orderPages.StepFourReviewAndCompleteOrderAsync();
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task CatalogueSolutionWithAdditionalService()
    {
        await orderPages.LoginAsync();
        await orderPages.CreateNewOrderAsync();
        await orderPages.StepOnePrepareOrderAsync();
        await orderPages.StepTwoAddSolutionsAndServicesAsync(solutionName: SolutionName, additionalService: AdditionalService);
        await orderPages.StepTwoDeliveryAndFundingAsync(additionalService: AdditionalService);
        await orderPages.StepThreeCompleteContractAsync();
        await orderPages.StepFourReviewAndCompleteOrderAsync();
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task CatalogueSolutionWithAssociatedAndAdditionalService()
    {
        await orderPages.LoginAsync();
        await orderPages.CreateNewOrderAsync();
        await orderPages.StepOnePrepareOrderAsync();
        await orderPages.StepTwoAddSolutionsAndServicesAsync(
            solutionName: SolutionName,
            associatedService: AssociatedService,
            additionalService: AdditionalService);
        await orderPages.StepTwoDeliveryAndFundingAsync(
            associatedService: AssociatedService,
            additionalService: AdditionalService);
        await orderPages.StepThreeCompleteContractAsync(associatedService: AssociatedService);
        await orderPages.StepFourReviewAndCompleteOrderAsync();
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task CatalogueSolutionUploadServiceRecipientsUsingCsv()
    {
        await orderPages.LoginAsync();
        await orderPages.CreateNewOrderAsync();
        await orderPages.StepOnePrepareOrderAsync();
        await orderPages.StepTwoAddSolutionsAndServicesAsync(
            solutionName: SolutionName,
            serviceRecipientsCsv: CsvFileName);
        await orderPages.StepTwoDeliveryAndFundingAsync();
        await orderPages.StepThreeCompleteContractAsync();
        await orderPages.StepFourReviewAndCompleteOrderAsync();
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task CatalogueSolutionChangeMidOrder()
    {
        await orderPages.LoginAsync();
        await orderPages.CreateNewOrderAsync();
        await orderPages.StepOnePrepareOrderAsync();
        await orderPages.StepTwoAddSolutionsAndServicesAsync(solutionName: AlternativeSolution);
        await orderPages.StepTwoChangeCatalogueSolutionAsync(SolutionName);
        await orderPages.StepTwoDeliveryAndFundingAsync();
        await orderPages.StepThreeCompleteContractAsync();
        await orderPages.StepFourReviewAndCompleteOrderAsync();
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task EditOrderWithCatalogueSolutionAdditionalAndAssociatedService()
    {
        await orderPages.LoginAsync();
        await orderPages.CreateNewOrderAsync();
        await orderPages.StepOnePrepareOrderAsync();

        await orderPages.StepTwoAddSolutionsAndServicesAsync(
            solutionName: SolutionName,
            associatedService: AssociatedService,
            additionalService: AdditionalService);

        await orderPages.StepTwoEditSolutionsAndServicesAsync(
            oldAdditionalService: AdditionalService, newAdditionalService: NewAdditionalService,
            oldAssociatedService: AssociatedService, newAssociatedService: NewAssociatedService);

        await orderPages.StepTwoDeliveryAndFundingAsync(
            associatedService: NewAssociatedService,
            additionalService: NewAdditionalService);

        await orderPages.StepThreeCompleteContractAsync(associatedService: NewAssociatedService);
        await orderPages.StepFourReviewAndCompleteOrderAsync();
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task AssociatedServiceOnly_SomethingElse()
    {
        var data = new AssociatedServiceTestDataBuilder()
            .WithBaseUrl(Fixture.BaseUrl)
            .WithServiceCategory("Something else")
            .WithSupplier("EMIS Health")
            .WithCatalogueSolutionAndAssociatedService("Emis Web GP", "Engineering")
            .WithFundingFilter("Engineering")
            .Build();

        await orderPages.LoginAsync();
        await orderPages.CreateNewAssociatedServiceOrderAsync(data);
        await orderPages.StepOnePrepareAssociatedServiceOrderAsync(data);
        await orderPages.StepTwoAddAssociatedServiceAsync(data);
        await orderPages.StepThreeCompleteAssociatedServiceContractAsync();
        await orderPages.StepFourReviewAndCompleteOrderAsync();
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task AssociatedServiceOnly_Merger()
    {
        var data = new AssociatedServiceTestDataBuilder()
            .WithBaseUrl(Fixture.BaseUrl)
            .WithServiceCategory("Merger")
            .WithSupplier("EMIS Health", isMerger: true)
            .WithCatalogueSolutionForMerger("Video Consult")
            .WithPracticesAndRecipientToBeMerged(MergerPractices, "BANKFIELD SURGERY")
            .WithFundingFilter("Merger")
            .Build();

        await orderPages.LoginAsync();
        await orderPages.CreateNewAssociatedServiceOrderAsync(data);
        await orderPages.StepOnePrepareAssociatedServiceOrderAsync(data);
        await orderPages.StepTwoAddAssociatedServiceAsync(data);
        await orderPages.StepThreeCompleteAssociatedServiceContractAsync();
        await orderPages.StepFourReviewAndCompleteOrderAsync();
    }
}
