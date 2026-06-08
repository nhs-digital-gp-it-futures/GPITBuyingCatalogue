using System.Threading.Tasks;
using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepTwo;

public class ServiceRecipientsPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "service recipients" });
    private ILocator ManualOption => Page.GetByLabel("Add service recipients manually");
    private ILocator SelectLink => Page.GetByRole(AriaRole.Link, new() { Name = "Select" });

    public ServiceRecipientsPage(IPage page) : base(page) { }

    public async Task NavigateAsync() =>
        await NavigationLink.ClickAsync();

    public async Task SelectRecipientsManuallyAsync(string sublocation, string[] practices)
    {
        await ManualOption.CheckAsync();
        await ClickSaveAndContinueAsync();
        await SelectSublocationAsync(sublocation, "order");
        await SelectPracticesAsync(practices);
        await ConfirmRecipientsAsync();
    }

    public async Task SelectRecipientsWithRecipientToBeMergedAsync(string sublocation, string[] practices, string recipientToBeMerged, string serviceCategory)
    {
        await ManualOption.CheckAsync();
        await ClickSaveAndContinueAsync();
        await SelectSublocationAsync(sublocation, serviceCategory);
        await SelectPracticesAsync(practices);

        await AssertHeadingAsync("Service recipient to be retained");
        await Page.GetByRole(AriaRole.Radio, new() { Name = recipientToBeMerged }).CheckAsync();
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Confirm service recipients");
        await ClickSaveAndContinueAsync();
    }

    private async Task SelectSublocationAsync(string sublocation, string serviceCategory)
    {
        await AssertHeadingAsync($"Select sublocations for this {serviceCategory}");
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = sublocation }).CheckAsync();
        await ClickSaveAndContinueAsync();
    }

    private async Task SelectPracticesAsync(string[] practices)
    {
        await AssertHeadingAsync("Confirm sublocations");
        await SelectLink.ClickAsync();

        foreach (var practice in practices)
            await Page.GetByRole(AriaRole.Checkbox, new() { Name = practice }).CheckAsync();

        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Confirm sublocations");
        await ClickSaveAndContinueAsync();
    }

    private async Task ConfirmRecipientsAsync()
    {
        await AssertHeadingAsync("Confirm service recipients");
        await ClickSaveAndContinueLinkAsync();
    }
}
