using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.ContractingVehicles;

public class AddContractingVehiclePage : BasePage
{
    private ILocator NameInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Name" });
    private ILocator DurationInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "What is the maximum duration" });

    public AddContractingVehiclePage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() => await AssertHeadingAsync("Add a contracting vehicle");

    public async Task AddAsync(string name, string maxDuration, string[] fundingTypes)
    {
        await NameInput.FillAsync(name);
        await DurationInput.FillAsync(maxDuration);

        foreach (var fundingType in fundingTypes)
            await Page.GetByRole(AriaRole.Checkbox, new() { Name = fundingType }).CheckAsync();

        await ClickSaveAndContinueAsync();
    }
}
