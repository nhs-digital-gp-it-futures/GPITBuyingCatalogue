using Microsoft.AspNetCore.Mvc;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsConfirmationBanner;

public sealed class NhsConfirmationBannerViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string title, string body = null, NhsConfirmationBannerModel.BannerColour colour = NhsConfirmationBannerModel.BannerColour.Blue)
    {
        return View(
            "NhsConfirmationBanner",
            new NhsConfirmationBannerModel { Title = title, Body = body, Colour = colour, });
    }
}
