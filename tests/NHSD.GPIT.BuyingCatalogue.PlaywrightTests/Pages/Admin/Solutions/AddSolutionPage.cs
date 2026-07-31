using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;
using static Microsoft.Playwright.Assertions;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.Solutions;

public class AddSolutionPage : BasePage
{
    private const string TaskListText = "Explain what your catalogue solution is for";

    private ILocator SolutionNameInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Solution name" });
    private ILocator SupplierDropdown => Page.GetByLabel("Supplier name");

    public AddSolutionPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() =>
        await AssertHeadingAsync("Add a solution");

    public async Task CreateSolutionAsync(string solutionName, string supplierValue, string framework)
    {
        await SolutionNameInput.FillAsync(solutionName);
        await SupplierDropdown.SelectOptionAsync(new[] { supplierValue });
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = framework }).CheckAsync();
        await ClickSaveAndContinueAsync();
        await AssertOnTaskListAsync();
    }

    private async Task AssertOnTaskListAsync() =>
        await Expect(Page.GetByText(TaskListText)).ToBeVisibleAsync();
    
    private async Task EditSectionAsync(string sectionName) =>
        await Page.GetByRole(AriaRole.Row, new() { Name = sectionName })
            .GetByRole(AriaRole.Link, new() { Name = "Edit" })
            .ClickAsync();

    public async Task AddDescriptionAsync(string summary, string fullDescription)
    {
        await EditSectionAsync("Description");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Summary" }).FillAsync(summary);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Full description (optional)" }).FillAsync(fullDescription);
        await ClickSaveAndContinueAsync();
        await AssertOnTaskListAsync();
    }

    public async Task AddFeaturesAsync(string feature1, string feature2)
    {
        await EditSectionAsync("Features");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Feature 1 (optional)" }).FillAsync(feature1);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Feature 2 (optional)" }).FillAsync(feature2);
        await ClickSaveAndContinueAsync();
        await AssertOnTaskListAsync();
    }

    public async Task AddApplicationTypeAsync(string storageSpace, string processingPower)
    {
        await EditSectionAsync("Application type");
        await AssertHeadingAsync("Application type");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add an application type" }).ClickAsync();
        await AssertHeadingAsync("Add an application type");
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Desktop" }).CheckAsync();
        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Desktop application");

        // Supported operating systems
        await EditSectionAsync("Supported operating systems");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Supported operating systems" }).FillAsync("Windows");
        await ClickSaveAndContinueAsync();

        // Connectivity
        await EditSectionAsync("Connectivity");
        await Page.GetByLabel("Connection speed").SelectOptionAsync(new[] { "Higher than 30Mbps" });
        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Desktop application");

        // Memory, storage, processing and resolution
        await EditSectionAsync("Memory, storage, processing and resolution");
        await Page.GetByLabel("Memory size").SelectOptionAsync(new[] { "256MB" });
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Storage space" }).FillAsync(storageSpace);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Processing power" }).FillAsync(processingPower);
        await Page.GetByLabel("Screen resolution and aspect").SelectOptionAsync(new[] { "16:9 - 3840 x 2160" });
        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Desktop application");

        await ClickSaveAndContinueLinkAsync();
        await AssertHeadingAsync("Application type");
        await ClickSaveAndContinueLinkAsync();
        await AssertOnTaskListAsync();
    }

    public async Task AddHostingTypeAsync(string summary, string dataCentreModel)
    {
        await EditSectionAsync("Hosting type");
        await AssertHeadingAsync("Hosting type");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add a hosting type" }).ClickAsync();
        await Page.GetByText("On premise").ClickAsync();
        await ClickSaveAndContinueAsync();

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Summary" }).FillAsync(summary);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Data centre model" }).FillAsync(dataCentreModel);
        await Page.GetByText("Yes, devices must be").ClickAsync();
        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Hosting type");
        await ClickSaveAndContinueAsync();
        await AssertOnTaskListAsync();
    }

    public async Task AddListPriceAsync()
    {
        await EditSectionAsync("List price");
        await AssertHeadingAsync("List price");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add a list price" }).ClickAsync();
        await AssertHeadingAsync("List price type");
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Flat price" }).CheckAsync();
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Add a flat list price");
        await Page.GetByText("Per patient per year").ClickAsync();
        await Page.GetByText("Single fixed").ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Price" }).FillAsync("1.5");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Unit", Exact = true }).FillAsync("per patient");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Units" }).FillAsync("patients");
        await Page.GetByText("Publish").ClickAsync();
        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("List price");
        await ClickSaveAndContinueLinkAsync();
        await AssertOnTaskListAsync();
    }

    public async Task AddCapabilitiesAsync()
    {
        await EditSectionAsync("Capabilities and Epics");
        await AssertHeadingAsync("Capabilities and Epics");
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "(C5) Appointments Management" }).CheckAsync();
        await ClickSaveAndContinueAsync();
        await AssertOnTaskListAsync();
    }

    public async Task AddSupplierDetailsAsync(string contactName)
    {
        await EditSectionAsync("Supplier details");
        await AssertHeadingAsync("Supplier details");
        await Page.GetByText(contactName).ClickAsync();
        await ClickSaveAndContinueAsync();
        await AssertOnTaskListAsync();
    }

    public async Task AddServiceLevelAgreementAsync(
        string supportType, string serviceType, string serviceLevel, string howMeasured)
    {
        await EditSectionAsync("Service Level Agreement");
        await AssertHeadingAsync("Catalogue solution type");
        await Page.GetByText("Type 2 catalogue solution").ClickAsync();
        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Service Level Agreement");

        // Availability times
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add availability times" }).ClickAsync();
        await AssertHeadingAsync("Service availability times");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Type of support" }).FillAsync(supportType);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "From" }).FillAsync("09:00");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Until" }).FillAsync("18:00");
        foreach (var day in new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" })
            await Page.GetByRole(AriaRole.Checkbox, new() { Name = day }).CheckAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Yes, include Bank Holidays" }).CheckAsync();
        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Service Level Agreement");

        // Contact details
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add contact details" }).ClickAsync();
        await AssertHeadingAsync("Support contact details");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Contact channel" }).FillAsync("web chat");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Contact information" }).FillAsync("01274231312");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "From" }).FillAsync("09:00");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Until" }).FillAsync("18:00");
        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Service Level Agreement");

        // Service levels
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add service levels" }).ClickAsync();
        await AssertHeadingAsync("Service levels");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Type of service" }).FillAsync(serviceType);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Service level", Exact = true }).FillAsync(serviceLevel);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "How the service levels are" }).FillAsync(howMeasured);
        await Page.GetByText("No", new() { Exact = true }).ClickAsync();
        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Service Level Agreement");
        await ClickSaveAndContinueLinkAsync();
        await AssertOnTaskListAsync();
    }
    
    public async Task PublishSolutionAsync()
    {
        await Page.GetByText("Publish", new() { Exact = true }).ClickAsync();
        await ClickSaveAndContinueAsync();
    }
}
