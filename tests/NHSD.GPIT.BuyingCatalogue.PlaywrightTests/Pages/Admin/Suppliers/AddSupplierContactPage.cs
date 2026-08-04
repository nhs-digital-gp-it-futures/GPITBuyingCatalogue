using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.Suppliers;

public class AddSupplierContactPage : BasePage
{
    private ILocator FirstNameInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "First name" });
    private ILocator LastNameInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Last name" });
    private ILocator DepartmentInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Department name" });
    private ILocator PhoneInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Telephone number" });
    private ILocator EmailInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Email address" });

    public AddSupplierContactPage(IPage page) : base(page) { }

    public async Task AddAsync(SupplierContact contact)
    {
        await FirstNameInput.FillAsync(contact.FirstName);
        await LastNameInput.FillAsync(contact.LastName);
        await DepartmentInput.FillAsync(contact.Department);
        await PhoneInput.FillAsync(contact.Phone);
        await EmailInput.FillAsync(contact.Email);
        await ClickSaveAndContinueAsync();
    }
}
