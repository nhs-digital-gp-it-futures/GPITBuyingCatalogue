using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepTwo;

public class ServiceRecipientsPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "service recipients" });
    private ILocator ManualOption => Page.GetByLabel("Add service recipients manually");
    private ILocator UploadOption => Page.GetByLabel("Upload a CSV file");
    private ILocator FileInput => Page.Locator("#File");
    private ILocator SelectLink => Page.GetByRole(AriaRole.Link, new() { Name = "Select" });

    public ServiceRecipientsPage(IPage page) : base(page) { }

    public async Task NavigateAsync() =>
        await NavigationLink.ClickAsync();

    public async Task SelectRecipientsManuallyAsync(string sublocation, string[] practices, string serviceCategory = "order")
    {
        await ManualOption.CheckAsync();
        await ClickSaveAndContinueAsync();
        await SelectSublocationAsync(sublocation, serviceCategory);
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

    // Upload a CSV file of service recipients instead of selecting them manually.
    // The file lives in TestData/Files/ in the project, copied to the build output.
    public async Task UploadServiceRecipientsCsvAsync(string fileName)
    {
        await UploadOption.CheckAsync();
        await ClickSaveAndContinueAsync();

        var filePath = Path.Combine(AppContext.BaseDirectory, "TestData", "Files", fileName);
        await FileInput.SetInputFilesAsync(filePath);
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Upload validated");
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Confirm sublocations");
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Confirm service recipients");
        await ClickSaveAndContinueLinkAsync();
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

    public async Task AssertOnPageAsync() =>
    await AssertHeadingAsync("Service recipients");

    public async Task ChooseUploadOptionAsync()
    {
        await UploadOption.CheckAsync();
        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Upload service recipients");
    }

    public async Task GoToAddRecipientsPageAsync(string sublocation)
    {
        await ManualOption.CheckAsync();
        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Select sublocations for this order");
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = sublocation }).CheckAsync();
        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Confirm sublocations");
        await SelectLink.ClickAsync();

        await AssertHeadingAsync("Add service recipients");
    }

    public async Task AssertRadioGroupHasLegendAsync()
    {
        var fieldset = Page.Locator("fieldset:has(input[type='radio'])");
        var fieldsetCount = await fieldset.CountAsync();

        Assert.True(fieldsetCount > 0,
            "Recipient option radio group is not wrapped in a fieldset (WAVE: fieldset missing legend, WCAG 1.3.1).");

        var legend = fieldset.Locator("legend");
        var legendCount = await legend.CountAsync();

        Assert.True(legendCount > 0,
            "Recipient option fieldset has no legend (WAVE: fieldset missing legend, WCAG 1.3.1).");
    }
}
