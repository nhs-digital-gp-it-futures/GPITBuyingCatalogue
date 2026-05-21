using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit.Abstractions;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Login;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.Dashboard;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.OrderType;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepOne;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepTwo;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepThree;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepFour;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering;

public class OrderingPages
{
    private readonly ITestOutputHelper _output;

    public LoginPage Login { get; }
    public OrderingDashboardPage Dashboard { get; }
    public OrderTypePage OrderType { get; }
    public ServiceCategoryPage ServiceCategory { get; }
    public OrderDescriptionPage Description { get; }
    public PrimaryContactPage PrimaryContact { get; }
    public SupplierPage Supplier { get; }
    public TimescalesPage Timescales { get; }
    public ServiceRecipientsPage ServiceRecipients { get; }
    public SolutionsAndServicesPage SolutionsAndServices { get; }
    public QuantityPage Quantity { get; }
    public PlannedDeliveryDatesPage PlannedDeliveryDates { get; }
    public FundingSourcesPage FundingSources { get; }
    public ImplementationMilestonesPage ImplementationMilestones { get; }
    public AssociatedServiceMilestonesPage AssociatedServiceMilestones { get; }
    public AssociatedServiceRequirementsPage AssociatedServiceRequirements { get; }
    public DataProcessingPage DataProcessing { get; }
    public DeclarationPage Declaration { get; }
    public ReviewOrderPage ReviewOrder { get; }

    public OrderingPages(IPage page, ITestOutputHelper output)
    {
        _output = output;

        Login = new LoginPage(page);
        Dashboard = new OrderingDashboardPage(page);
        OrderType = new OrderTypePage(page);
        ServiceCategory = new ServiceCategoryPage(page);
        Description = new OrderDescriptionPage(page);
        PrimaryContact = new PrimaryContactPage(page);
        Supplier = new SupplierPage(page);
        Timescales = new TimescalesPage(page);
        ServiceRecipients = new ServiceRecipientsPage(page);
        SolutionsAndServices = new SolutionsAndServicesPage(page);
        Quantity = new QuantityPage(page);
        PlannedDeliveryDates = new PlannedDeliveryDatesPage(page);
        FundingSources = new FundingSourcesPage(page);
        ImplementationMilestones = new ImplementationMilestonesPage(page);
        AssociatedServiceMilestones = new AssociatedServiceMilestonesPage(page);
        AssociatedServiceRequirements = new AssociatedServiceRequirementsPage(page);
        DataProcessing = new DataProcessingPage(page);
        Declaration = new DeclarationPage(page);
        ReviewOrder = new ReviewOrderPage(page);
    }

    // ------------------------------------------------------------------------
    // Catalogue Solution journey
    // ------------------------------------------------------------------------

    public async Task LoginAsync(OrderTestData data)
    {
        _output.WriteLine("Login");
        await Login.NavigateAsync(data.BaseUrl);
        await Login.LoginAsync(data.Email, data.Password);
        await Login.AssertLoginSuccessfulAsync();
    }

    public async Task CreateNewOrderAsync(OrderTestData data)
    {
        _output.WriteLine("Create new order");
        await Dashboard.GoToOrdersAsync();
        await Dashboard.CreateNewOrderAsync();
        await OrderType.SelectOrderTypeAsync(data.OrderType);
        await OrderType.SelectFrameworkAsync(data.Framework);
    }

    public async Task StepOnePrepareOrderAsync(OrderTestData data)
    {
        _output.WriteLine("Step 1 — prepare order");

        await Description.NavigateAsync();
        await Description.EnterDescriptionAsync(data.Description);

        await PrimaryContact.NavigateAsync();
        await PrimaryContact.EnterContactDetailsAsync(data.FirstName, data.LastName, data.Phone, data.ContactEmail);

        await Supplier.NavigateAsync();
        await Supplier.SearchAndSelectSupplierAsync(data.Supplier);
        await Supplier.ConfirmSupplierAsync();

        await Timescales.NavigateAsync();
        await Timescales.EnterTimescalesAsync(data.StartDay, data.StartMonth, data.StartYear, data.InitialPeriod, data.Duration);
    }

    public async Task StepTwoAddSolutionsAndServicesAsync(OrderTestData data)
    {
        _output.WriteLine("Step 2 — add solutions and services");

        await ServiceRecipients.NavigateAsync();
        await ServiceRecipients.SelectRecipientsManuallyAsync(data.Sublocation, data.Practices);

        await SolutionsAndServices.NavigateAsync();
        await SolutionsAndServices.SelectCatalogueSolutionAsync(data.Solution);
        await SolutionsAndServices.SelectPriceAsync();

        await Quantity.EnterQuantitiesAsync(data.Quantities);

        await PlannedDeliveryDates.NavigateAsync();
        await PlannedDeliveryDates.EnterDeliveryDateAsync(data.DeliveryDay, data.DeliveryMonth, data.DeliveryYear);

        await FundingSources.NavigateAsync();
        await FundingSources.SelectFundingAsync(data.Solution, data.FundingType);
    }

    public async Task StepThreeCompleteContractAsync()
    {
        _output.WriteLine("Step 3 — complete contract");
        await ImplementationMilestones.NavigateAndContinueAsync();
        await DataProcessing.NavigateAndContinueAsync();
        await Declaration.NavigateAndAgreeAsync();
    }

    public async Task StepFourReviewAndCompleteOrderAsync()
    {
        _output.WriteLine("Step 4 — review and complete");
        await ReviewOrder.NavigateAsync();
        //await ReviewOrder.CompleteOrderAsync();
    }

    // ------------------------------------------------------------------------
    // Associated Service journey
    // ------------------------------------------------------------------------

    public async Task CreateNewAssociatedServiceOrderAsync(AssociatedServiceTestData data)
    {
        _output.WriteLine("Create new Associated Service order");
        await Dashboard.GoToOrdersAsync();
        await Dashboard.CreateNewOrderAsync();
        await OrderType.SelectOrderTypeAsync(data.OrderType);
        await ServiceCategory.SelectServiceCategoryAsync(data.AssociatedServiceCategory);
        await OrderType.SelectFrameworkAsync(data.Framework);
    }

    public async Task StepOnePrepareAssociatedServiceOrderAsync(AssociatedServiceTestData data)
    {
        _output.WriteLine("Step 1 — prepare associated service order");

        await Description.NavigateAsync();
        await Description.EnterDescriptionAsync(data.Description);

        await PrimaryContact.NavigateAsync();
        await PrimaryContact.EnterContactDetailsAsync(data.FirstName, data.LastName, data.Phone, data.ContactEmail);

        await Supplier.NavigateAsync();
        if (data.IsMerger)
            await Supplier.SelectSupplierByRadioAsync(data.Supplier);
        else
            await Supplier.SearchAndSelectSupplierAsync(data.Supplier);
        await Supplier.ConfirmSupplierAsync();

        await Timescales.NavigateAsync();
        await Timescales.EnterTimescalesAsync(data.StartDay, data.StartMonth, data.StartYear, data.InitialPeriod, data.Duration);
    }

    public async Task StepTwoAddAssociatedServiceAsync(AssociatedServiceTestData data)
    {
        _output.WriteLine("Step 2 — add associated service");

        await ServiceRecipients.NavigateAsync();
        if (data.RequiresRecipientToBeMerged)
            await ServiceRecipients.SelectRecipientsWithRecipientToBeMergedAsync(
                data.Sublocation,
                data.Practices,
                data.RecipientToBeMerged,
                data.AssociatedServiceCategory);
        else
            await ServiceRecipients.SelectRecipientsManuallyAsync(data.Sublocation, data.Practices);

        await SolutionsAndServices.NavigateAsync();
        if (data.HasServiceVariant)
            await SolutionsAndServices.SelectAssociatedServiceWithVariantAsync(data.SolutionWithAssociatedService, data.AssociatedService);
        else
            await SolutionsAndServices.SelectAssociatedServiceAsync(data.SolutionWithAssociatedService);

        await SolutionsAndServices.SelectAssociatedServicePriceAsync();

        if (data.RequiresQuantities)
            await Quantity.EnterAssociatedServiceQuantitiesAsync(data.Quantities);
        else
            await SolutionsAndServices.ContinuePastEditAsync();

        await PlannedDeliveryDates.NavigateAsync();
        await PlannedDeliveryDates.EnterDeliveryDateAsync(data.DeliveryDay, data.DeliveryMonth, data.DeliveryYear);

        await FundingSources.NavigateAsync();
        await FundingSources.SelectFundingAsync(data.FundingFilter, data.FundingType);
    }

    public async Task StepThreeCompleteAssociatedServiceContractAsync()
    {
        _output.WriteLine("Step 3 — complete associated service contract");
        await AssociatedServiceMilestones.NavigateAndContinueAsync();
        await AssociatedServiceRequirements.NavigateAndContinueAsync();
        await DataProcessing.NavigateAndContinueAsync();
        await Declaration.NavigateAndAgreeAsync();
    }
}
