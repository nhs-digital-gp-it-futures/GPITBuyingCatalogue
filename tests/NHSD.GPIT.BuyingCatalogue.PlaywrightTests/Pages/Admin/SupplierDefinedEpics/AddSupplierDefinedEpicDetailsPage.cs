using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.SupplierDefinedEpics;

public class AddSupplierDefinedEpicDetailsPage : BasePage
{
    private ILocator NameInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Name" });
    private ILocator DescriptionInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Description" });
    private ILocator ActiveStatus => Page.GetByText("Active", new() { Exact = true });

    public AddSupplierDefinedEpicDetailsPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() => await AssertHeadingAsync("Supplier defined Epic details");

    public async Task AddAsync(string name, string description)
    {
        await NameInput.FillAsync(name);
        await DescriptionInput.FillAsync(description);
        await ActiveStatus.ClickAsync();
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Supplier defined Epic information");
        await ClickSaveAndContinueLinkAsync();
    }
}
