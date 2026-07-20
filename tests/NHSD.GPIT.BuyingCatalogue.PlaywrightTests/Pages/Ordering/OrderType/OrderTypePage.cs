using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.OrderType;

public enum OrderTypeOption { CatalogueSolution, AssociatedService }
public enum FrameworkType { TechInnovation }

public class OrderTypePage : BasePage
{
    private ILocator StartOrderButton => Page.GetByRole(AriaRole.Button, new() { Name = "Start order" });
    private ILocator CatalogueOptionLink => Page.GetByText("Catalogue solution and other");
    private ILocator AssociatedOptionRadio => Page.GetByRole(AriaRole.Radio, new() { Name = "Associated service only" });

    public OrderTypePage(IPage page) : base(page) { }

    public async Task SelectOrderTypeAsync(OrderTypeOption orderType = OrderTypeOption.CatalogueSolution)
    {
        await StartOrderButton.ClickAsync();

        switch (orderType)
        {
            case OrderTypeOption.CatalogueSolution: await CatalogueOptionLink.ClickAsync(); break;
            case OrderTypeOption.AssociatedService: await AssociatedOptionRadio.CheckAsync(); break;
        }

        await ClickSaveAndContinueAsync();
    }

    public async Task SelectFrameworkAsync(FrameworkType framework = FrameworkType.TechInnovation)
    {
        var name = framework switch
        {
            FrameworkType.TechInnovation => "Tech Innovation",
            _ => throw new ArgumentOutOfRangeException(nameof(framework))
        };

        await Page.GetByRole(AriaRole.Radio, new() { Name = name }).CheckAsync();
        await ClickSaveAndContinueAsync();
    }

    public async Task StartOrderAsync()
    {
        await StartOrderButton.ClickAsync();
    }

    public async Task AssertOnPageAsync() =>
        await AssertHeadingAsync("What do you want to order?");
}
