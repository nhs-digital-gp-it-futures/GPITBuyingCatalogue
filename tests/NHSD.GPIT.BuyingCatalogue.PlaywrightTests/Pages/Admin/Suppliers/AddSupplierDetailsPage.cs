using System.Threading.Tasks;
using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.Suppliers;

public class AddSupplierDetailsPage : BasePage
{
    private ILocator NameInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Supplier name" });
    private ILocator LegalNameInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Supplier legal name" });
    private ILocator AboutInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "About supplier (optional)" });

    public AddSupplierDetailsPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() => await AssertHeadingAsync("Supplier details");

    public async Task AddAsync(string name, string legalName, string about)
    {
        await NameInput.FillAsync(name);
        await LegalNameInput.FillAsync(legalName);
        await AboutInput.FillAsync(about);
        await ClickSaveAndContinueAsync();
    }
}
