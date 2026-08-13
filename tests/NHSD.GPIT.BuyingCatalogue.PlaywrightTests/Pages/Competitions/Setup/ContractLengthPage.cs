using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Steps;

public class ContractLengthPage : BasePage
{
    private ILocator LengthInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "How long will your contract" });

    public ContractLengthPage(IPage page) : base(page) { }

    public async Task EnterLengthAndContinueAsync(string months)
    {
        await AssertHeadingAsync("Contract length");
        await LengthInput.FillAsync(months);
        await ClickSaveAndContinueAsync();
    }
}
