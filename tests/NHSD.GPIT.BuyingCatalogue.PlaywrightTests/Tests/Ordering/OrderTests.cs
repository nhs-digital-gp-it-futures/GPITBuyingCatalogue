using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData.Builders;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData.Generators;
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
    [
        "BANKFIELD SURGERY",
        "BEECHWOOD MEDICAL CENTRE",
        "BRIG ROYD SURGERY"
    ];

    public OrderTests(TestServerFixture fixture, ITestOutputHelper output)
        : base(fixture, output)
    {
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task CatalogueSolutionOnly()
    {
        await OrderPages.LoginAsync();
        await OrderPages.CreateNewOrderAsync();
        await OrderPages.StepOnePrepareOrderAsync();
        await OrderPages.StepTwoAddSolutionsAndServicesAsync(solutionName: SolutionName);
        await OrderPages.StepTwoDeliveryAndFundingAsync();
        await OrderPages.StepThreeCompleteContractAsync();
        await OrderPages.StepFourReviewAndCompleteOrderAsync();
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task CatalogueSolutionWithAssociatedService()
    {
        await OrderPages.LoginAsync();
        await OrderPages.CreateNewOrderAsync();
        await OrderPages.StepOnePrepareOrderAsync();
        await OrderPages.StepTwoAddSolutionsAndServicesAsync(solutionName: SolutionName, associatedService: AssociatedService);
        await OrderPages.StepTwoDeliveryAndFundingAsync(associatedService: AssociatedService);
        await OrderPages.StepThreeCompleteContractAsync(associatedService: AssociatedService);
        await OrderPages.StepFourReviewAndCompleteOrderAsync();
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task CatalogueSolutionWithAdditionalService()
    {
        await OrderPages.LoginAsync();
        await OrderPages.CreateNewOrderAsync();
        await OrderPages.StepOnePrepareOrderAsync();
        await OrderPages.StepTwoAddSolutionsAndServicesAsync(solutionName: SolutionName, additionalService: AdditionalService);
        await OrderPages.StepTwoDeliveryAndFundingAsync(additionalService: AdditionalService);
        await OrderPages.StepThreeCompleteContractAsync();
        await OrderPages.StepFourReviewAndCompleteOrderAsync();
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task CatalogueSolutionWithAssociatedAndAdditionalService()
    {
        await OrderPages.LoginAsync();
        await OrderPages.CreateNewOrderAsync();
        await OrderPages.StepOnePrepareOrderAsync();
        await OrderPages.StepTwoAddSolutionsAndServicesAsync(
            solutionName: SolutionName,
            associatedService: AssociatedService,
            additionalService: AdditionalService);
        await OrderPages.StepTwoDeliveryAndFundingAsync(
            associatedService: AssociatedService,
            additionalService: AdditionalService);
        await OrderPages.StepThreeCompleteContractAsync(associatedService: AssociatedService);
        await OrderPages.StepFourReviewAndCompleteOrderAsync();
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task CatalogueSolutionUploadServiceRecipientsUsingCsv()
    {
        await OrderPages.LoginAsync();
        await OrderPages.CreateNewOrderAsync();
        await OrderPages.StepOnePrepareOrderAsync();
        await OrderPages.StepTwoAddSolutionsAndServicesAsync(
            solutionName: SolutionName,
            serviceRecipientsCsv: CsvFileName);
        await OrderPages.StepTwoDeliveryAndFundingAsync();
        await OrderPages.StepThreeCompleteContractAsync();
        await OrderPages.StepFourReviewAndCompleteOrderAsync();
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task CatalogueSolutionChangeMidOrder()
    {
        await OrderPages.LoginAsync();
        await OrderPages.CreateNewOrderAsync();
        await OrderPages.StepOnePrepareOrderAsync();
        await OrderPages.StepTwoAddSolutionsAndServicesAsync(solutionName: AlternativeSolution);
        await OrderPages.StepTwoChangeCatalogueSolutionAsync(SolutionName);
        await OrderPages.StepTwoDeliveryAndFundingAsync();
        await OrderPages.StepThreeCompleteContractAsync();
        await OrderPages.StepFourReviewAndCompleteOrderAsync();
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task EditOrderWithCatalogueSolutionAdditionalAndAssociatedService()
    {
        await OrderPages.LoginAsync();
        await OrderPages.CreateNewOrderAsync();
        await OrderPages.StepOnePrepareOrderAsync();

        await OrderPages.StepTwoAddSolutionsAndServicesAsync(
            solutionName: SolutionName,
            associatedService: AssociatedService,
            additionalService: AdditionalService);

        await OrderPages.StepTwoEditSolutionsAndServicesAsync(
            oldAdditionalService: AdditionalService, newAdditionalService: NewAdditionalService,
            oldAssociatedService: AssociatedService, newAssociatedService: NewAssociatedService);

        await OrderPages.StepTwoDeliveryAndFundingAsync(
            associatedService: NewAssociatedService,
            additionalService: NewAdditionalService);

        await OrderPages.StepThreeCompleteContractAsync(associatedService: NewAssociatedService);
        await OrderPages.StepFourReviewAndCompleteOrderAsync();
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task AssociatedServiceOnly_SomethingElse()
    {
        var data = new AssociatedServiceTestDataBuilder()
            .WithServiceCategory("Something else")
            .WithSupplier("EMIS Health")
            .WithCatalogueSolutionAndAssociatedService("Emis Web GP", "Engineering")
            .WithFundingFilter("Engineering")
            .Build();

        await OrderPages.LoginAsync();
        await OrderPages.CreateNewAssociatedServiceOrderAsync(data);
        await OrderPages.StepOnePrepareAssociatedServiceOrderAsync(data);
        await OrderPages.StepTwoAddAssociatedServiceAsync(data);
        await OrderPages.StepThreeCompleteAssociatedServiceContractAsync();
        await OrderPages.StepFourReviewAndCompleteOrderAsync();
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task AssociatedServiceOnly_Merger()
    {
        var data = new AssociatedServiceTestDataBuilder()
            .WithServiceCategory("Merger")
            .WithSupplier("EMIS Health", isMerger: true)
            .WithCatalogueSolutionForMerger("Video Consult")
            .WithPracticesAndRecipientToBeMerged(MergerPractices, "BANKFIELD SURGERY")
            .WithFundingFilter("Merger")
            .Build();

        await OrderPages.LoginAsync();
        await OrderPages.CreateNewAssociatedServiceOrderAsync(data);
        await OrderPages.StepOnePrepareAssociatedServiceOrderAsync(data);
        await OrderPages.StepTwoAddAssociatedServiceAsync(data);
        await OrderPages.StepThreeCompleteAssociatedServiceContractAsync();
        await OrderPages.StepFourReviewAndCompleteOrderAsync();
    }

    [Fact]
    [Trait("Category", Categories.OrderJourney)]
    public async Task CreateOrderWithBespokeImplementationMilestone()
    {
        var milestoneName = TestDataGenerator.MilestoneName();
        var paymentTrigger = TestDataGenerator.PaymentTrigger();

        await OrderPages.LoginAsync();
        await OrderPages.CreateNewOrderAsync();
        await OrderPages.StepOnePrepareOrderAsync();
        await OrderPages.StepTwoAddSolutionsAndServicesAsync(solutionName: SolutionName);
        await OrderPages.StepTwoDeliveryAndFundingAsync();
        await OrderPages.StepThreeCompleteContractAsync(
            addBespokeEntries: true,
            implementationMilestoneName: milestoneName,
            implementationPaymentTrigger: paymentTrigger);
        await OrderPages.StepFourReviewAndCompleteOrderAsync();
        await OrderPages.ReviewOrder.AssertOrderCompletedAsync();
    }
}
