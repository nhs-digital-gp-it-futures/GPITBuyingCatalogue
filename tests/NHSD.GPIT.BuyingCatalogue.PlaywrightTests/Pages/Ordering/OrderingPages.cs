using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Login;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.Dashboard;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.OrderType;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepFour;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepOne;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepThree;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepTwo;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering;

public class OrderingPages
{
    private readonly ITestOutputHelper _output;
    private readonly OrderTestData _data;

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

    public OrderingPages(IPage page, ITestOutputHelper output, OrderTestData data)
    {
        _output = output;
        _data = data;

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

    public async Task LoginAsync()
    {
        _output.WriteLine("Login");
        await Login.NavigateAsync(_data.BaseUrl);
        await Login.LoginAsync(_data.Email, _data.Password);
        await Login.AssertLoginSuccessfulAsync();
    }

    public async Task CreateNewOrderAsync()
    {
        _output.WriteLine("Create new order");
        await Dashboard.GoToOrdersAsync();
        await Dashboard.CreateNewOrderAsync();
        await OrderType.SelectOrderTypeAsync(_data.OrderType);
        await OrderType.SelectFrameworkAsync(_data.Framework);
    }

    public async Task StepOnePrepareOrderAsync()
    {
        _output.WriteLine("Step 1 — prepare order");

        await Description.NavigateAsync();
        await Description.EnterDescriptionAsync(_data.Description);

        await PrimaryContact.NavigateAsync();
        await PrimaryContact.EnterContactDetailsAsync(_data.FirstName, _data.LastName, _data.Phone, _data.ContactEmail);

        await Supplier.NavigateAsync();
        await Supplier.SearchAndSelectSupplierAsync(_data.Supplier);
        await Supplier.ConfirmSupplierAsync();

        await Timescales.NavigateAsync();
        await Timescales.EnterTimescalesAsync(_data.StartDay, _data.StartMonth, _data.StartYear, _data.InitialPeriod, _data.Duration);
    }

    /// <summary>
    /// Completes step 2 of the order journey by selecting the catalogue solution,
    /// entering quantities, and optionally adding associated or additional services.
    /// </summary>
    /// <param name="solutionName">The catalogue solution to add to the order.</param>
    /// <param name="associatedService">Optional associated service to add.</param>
    /// <param name="additionalService">Optional additional service to add.</param>
    public async Task StepTwoAddSolutionsAndServicesAsync(
        string solutionName,
        string associatedService = "",
        string additionalService = "")
    {
        _output.WriteLine("Step 2 — add solutions and services");

        var hasAddOns =
            !string.IsNullOrWhiteSpace(associatedService) ||
            !string.IsNullOrWhiteSpace(additionalService);

        await ServiceRecipients.NavigateAsync();
        await ServiceRecipients.SelectRecipientsManuallyAsync(_data.Sublocation, _data.Practices);

        await SolutionsAndServices.NavigateAsync();
        await SolutionsAndServices.SelectCatalogueSolutionAsync(solutionName);
        await SolutionsAndServices.SelectPriceAsync();

        // Enter base solution quantities. If add-ons are required, remain on the
        // edit page so the associated/additional service links stay available.
        await Quantity.EnterQuantitiesAsync(_data.Quantities, completeEdit: !hasAddOns);

        if (!string.IsNullOrWhiteSpace(additionalService))
        {
            await SolutionsAndServices.AddOnServiceToCatalogueAsync(AddOnServiceType.Additional, additionalService);
            await Quantity.EnterAddOnQuantitiesAsync(
                _data.Quantities,
                AddOnServiceType.Additional.QuantityHeading(),
                completeEdit: string.IsNullOrWhiteSpace(associatedService));
        }

        if (!string.IsNullOrWhiteSpace(associatedService))
        {
            await SolutionsAndServices.AddOnServiceToCatalogueAsync(AddOnServiceType.Associated, associatedService);
            await Quantity.EnterAddOnQuantitiesAsync(
                _data.Quantities,
                AddOnServiceType.Associated.QuantityHeading(),
                completeEdit: true);
        }

        await PlannedDeliveryDates.NavigateAsync();
        await PlannedDeliveryDates.EnterDeliveryDateAsync(_data.DeliveryDay, _data.DeliveryMonth, _data.DeliveryYear);

        var fundingFilters = new List<string> { solutionName };
        if (!string.IsNullOrWhiteSpace(additionalService)) fundingFilters.Add(additionalService);
        if (!string.IsNullOrWhiteSpace(associatedService)) fundingFilters.Add(associatedService);

        await FundingSources.NavigateAsync();
        await FundingSources.SelectFundingForSourcesAsync(_data.FundingType, fundingFilters.ToArray());
    }

    public async Task StepThreeCompleteContractAsync(string associatedService = "")
    {
        _output.WriteLine("Step 3 — complete contract");

        await ImplementationMilestones.NavigateAndContinueAsync();

        if (!string.IsNullOrWhiteSpace(associatedService))
        {
            await AssociatedServiceMilestones.NavigateAndContinueAsync();
            await AssociatedServiceRequirements.NavigateAndContinueAsync();
        }

        await DataProcessing.NavigateAndContinueAsync();
        await Declaration.NavigateAndAgreeAsync();
    }

    // TODO: Re-enable order completion once the error has been resolved.
    public async Task StepFourReviewAndCompleteOrderAsync()
    {
        _output.WriteLine("Step 4 — review and complete");
        await ReviewOrder.NavigateAsync();
        //await ReviewOrder.CompleteOrderAsync();
    }

    /// <summary>
    /// Starts an associated-service-only order journey for the supported
    /// scenarios such as Something Else and Merger.
    /// </summary>
    public async Task CreateNewAssociatedServiceOrderAsync(AssociatedServiceTestData data)
    {
        _output.WriteLine("Create new Associated Service order");
        await Dashboard.GoToOrdersAsync();
        await Dashboard.CreateNewOrderAsync();
        await OrderType.SelectOrderTypeAsync(data.OrderType);
        await ServiceCategory.SelectServiceCategoryAsync(data.AssociatedServiceCategory);
        await OrderType.SelectFrameworkAsync(data.Framework);
    }

    /// <summary>
    /// Completes step 1 of the associated-service order journey, including
    /// supplier selection for merger and non-merger scenarios.
    /// </summary>
    public async Task StepOnePrepareAssociatedServiceOrderAsync(AssociatedServiceTestData data)
    {
        _output.WriteLine("Step 1 — prepare associated service order");

        await Description.NavigateAsync();
        await Description.EnterDescriptionAsync(data.Description);

        await PrimaryContact.NavigateAsync();
        await PrimaryContact.EnterContactDetailsAsync(data.FirstName, data.LastName, data.Phone, data.ContactEmail);

        await Supplier.NavigateAsync();

        // Merger journey uses the radio selection path instead of supplier search.
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
            await SolutionsAndServices.SelectAssociatedServiceSomeThingElseAsync(data.SolutionWithAssociatedService, data.AssociatedService);
        else
            await SolutionsAndServices.SelectAssociatedServiceMergerAsync(data.SolutionWithAssociatedService);

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
