using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.Capabilities;

public class MapCapabilitiesPage : BasePage
{
    private ILocator FileInput => Page.Locator("#File");

    private ILocator UploadCapabilitiesButton => Page.GetByRole(AriaRole.Button, new() { Name = "Upload Capabilities" });
    private ILocator UploadEpicsButton => Page.GetByRole(AriaRole.Button, new() { Name = "Upload Epics" });
    private ILocator ReturnHomeLink => Page.GetByRole(AriaRole.Link, new() { Name = "Return to admin homepage" });

    public MapCapabilitiesPage(IPage page) : base(page) { }

    public async Task AssertOnCapabilitiesPageAsync() =>
        await AssertHeadingAsync("Map Capabilities to solutions and services");

    public async Task AssertOnEpicsPageAsync() =>
        await AssertHeadingAsync("Map Epics to solutions and services");

    public async Task UploadCapabilitiesAsync(string fileName)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "TestData", "Files", fileName);
        await FileInput.SetInputFilesAsync(filePath);
        await UploadCapabilitiesButton.ClickAsync();
    }

    public async Task UploadEpicsAsync(string fileName)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "TestData", "Files", fileName);
        await FileInput.SetInputFilesAsync(filePath);
        await UploadEpicsButton.ClickAsync();
    }

    public async Task ReturnToAdminHomeAsync() =>
        await ReturnHomeLink.ClickAsync();
}
