using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

    // ------------------------------------------------------------------------
    // Shared steps
    // ------------------------------------------------------------------------

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

    public async Task StepOnePrepareOrderAsync([CallerMemberName] string orderDescription = "")
    {
        _output.WriteLine($"Step 1 — prepare order: {orderDescription}");

        await Description.NavigateAsync();
        await Description.EnterDescriptionAsync(orderDescription);

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
        string additionalService = "",
        string serviceRecipientsCsv = "")
    {
        _output.WriteLine("Step 2 — add solutions and services");

        var hasAddOns =
            !string.IsNullOrWhiteSpace(associatedService) ||
            !string.IsNullOrWhiteSpace(additionalService);

        await ServiceRecipients.NavigateAsync();
        if (!string.IsNullOrWhiteSpace(serviceRecipientsCsv))
            await ServiceRecipients.UploadServiceRecipientsCsvAsync(serviceRecipientsCsv);
        else
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
    }

    public async Task StepTwoEditSolutionsAndServicesAsync(
    string oldAssociatedService = "", string newAssociatedService = "",
    string oldAdditionalService = "", string newAdditionalService = "")
    {
        _output.WriteLine("Step 2 — edit solutions and services");

        var hasAdditionalEdit =
            !string.IsNullOrWhiteSpace(oldAdditionalService) &&
            !string.IsNullOrWhiteSpace(newAdditionalService);

        var hasAssociatedEdit =
            !string.IsNullOrWhiteSpace(oldAssociatedService) &&
            !string.IsNullOrWhiteSpace(newAssociatedService);

        if (!hasAdditionalEdit && !hasAssociatedEdit) return;

        await SolutionsAndServices.NavigateAsync();

        if (hasAdditionalEdit)
        {
            await SolutionsAndServices.ReplaceAddOnServiceAsync(
                AddOnServiceType.Additional, oldAdditionalService, newAdditionalService);
            await Quantity.EnterAddOnQuantitiesAsync(
                _data.Quantities,
                AddOnServiceType.Additional.QuantityHeading(),
                completeEdit: !hasAssociatedEdit);
        }

        if (hasAssociatedEdit)
        {
            await SolutionsAndServices.ReplaceAddOnServiceAsync(
                AddOnServiceType.Associated, oldAssociatedService, newAssociatedService);
            await Quantity.EnterAddOnQuantitiesAsync(
                _data.Quantities,
                AddOnServiceType.Associated.QuantityHeading(),
                completeEdit: true);
        }
    }

    public async Task StepTwoChangeCatalogueSolutionAsync(string newSolutionName)
    {
        _output.WriteLine("Step 2 — change catalogue solution");
        await SolutionsAndServices.NavigateAsync();
        await SolutionsAndServices.ChangeCatalogueSolutionAsync(newSolutionName);
        await Quantity.EnterQuantitiesAsync(_data.Quantities, completeEdit: true);
    }

    public async Task StepTwoDeliveryAndFundingAsync(
        string associatedService = "",
        string additionalService = "",
        string catalogueSolutionOverride = "")
    {
        _output.WriteLine("Step 2 — delivery dates and funding");

        await PlannedDeliveryDates.NavigateAsync();
        await PlannedDeliveryDates.EnterDeliveryDateAsync(_data.DeliveryDay, _data.DeliveryMonth, _data.DeliveryYear);

        // Default to the baseline solution; override allows tests that changed the solution to fund the correct one
        var solutionForFunding = string.IsNullOrWhiteSpace(catalogueSolutionOverride)
            ? _data.Solution
            : catalogueSolutionOverride;

        var fundingFilters = new List<string> { solutionForFunding };
        if (!string.IsNullOrWhiteSpace(additionalService)) fundingFilters.Add(additionalService);
        if (!string.IsNullOrWhiteSpace(associatedService)) fundingFilters.Add(associatedService);

        await FundingSources.NavigateAsync();
        await FundingSources.SelectFundingForSourcesAsync(_data.FundingType, fundingFilters.ToArray());
    }

    public async Task StepThreeCompleteContractAsync(
    string associatedService = "",
    bool addBespokeEntries = false,
    string implementationMilestoneName = "",
    string implementationPaymentTrigger = "",
    string associatedServiceMilestoneName = "",
    string associatedServicePaymentTrigger = "",
    string associatedServiceRequirement = "")
    {
        _output.WriteLine("Step 3 — complete contract");

        if (addBespokeEntries && !string.IsNullOrWhiteSpace(implementationMilestoneName))
            await ImplementationMilestones.NavigateAndAddBespokeMilestoneAsync(
                implementationMilestoneName, implementationPaymentTrigger);
        else
            await ImplementationMilestones.NavigateAndContinueAsync();

        if (!string.IsNullOrWhiteSpace(associatedService))
        {
            if (addBespokeEntries && !string.IsNullOrWhiteSpace(associatedServiceMilestoneName))
                await AssociatedServiceMilestones.NavigateAndAddBespokeMilestoneAsync(
                    associatedService, associatedServiceMilestoneName, associatedServicePaymentTrigger);
            else
                await AssociatedServiceMilestones.NavigateAndContinueAsync();

            if (addBespokeEntries && !string.IsNullOrWhiteSpace(associatedServiceRequirement))
                await AssociatedServiceRequirements.NavigateAndAddRequirementAsync(
                    associatedService, associatedServiceRequirement);
            else
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
    public async Task StepOnePrepareAssociatedServiceOrderAsync(AssociatedServiceTestData data, [CallerMemberName] string orderDescription = "")
    {
        _output.WriteLine($"Step 1 — prepare associated service order: {orderDescription}");

        await Description.NavigateAsync();
        await Description.EnterDescriptionAsync(orderDescription);

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
    
    public async Task GoToOrderTypePageAsync()
    {
        _output.WriteLine("Go to order type page");
        await Dashboard.GoToOrdersAsync();
        await Dashboard.CreateNewOrderAsync();
        await OrderType.StartOrderAsync();
        await OrderType.AssertOnPageAsync();
    }

    public async Task GoToDeclarationPageAsync(string solutionName)
    {
        _output.WriteLine("Go to declaration page");
        await LoginAsync();
        await CreateNewOrderAsync();
        await StepOnePrepareOrderAsync();
        await StepTwoAddSolutionsAndServicesAsync(solutionName: solutionName);
        await StepTwoDeliveryAndFundingAsync();
        await ImplementationMilestones.NavigateAndContinueAsync();
        await DataProcessing.NavigateAndContinueAsync();
        await Declaration.NavigateAsync();
        await Declaration.AssertOnPageAsync();
    }

    public async Task GoToUploadServiceRecipientsPageAsync(string solutionName)
    {
        _output.WriteLine("Go to upload service recipients page");
        await LoginAsync();
        await CreateNewOrderAsync();
        await StepOnePrepareOrderAsync();
        await ServiceRecipients.NavigateAsync();
        await ServiceRecipients.ChooseUploadOptionAsync();
    }

    public async Task GoToAddServiceRecipientsPageAsync()
    {
        _output.WriteLine("Go to add service recipients page");
        await LoginAsync();
        await CreateNewOrderAsync();
        await StepOnePrepareOrderAsync();

        await ServiceRecipients.NavigateAsync();
        await ServiceRecipients.GoToAddRecipientsPageAsync(_data.Sublocation);
    }

    public async Task GoToServiceRecipientsOptionPageAsync()
    {
        _output.WriteLine("Go to service recipients option page");
        await LoginAsync();
        await CreateNewOrderAsync();
        await StepOnePrepareOrderAsync();
        await ServiceRecipients.NavigateAsync();
        await ServiceRecipients.AssertOnPageAsync();
    }
}
