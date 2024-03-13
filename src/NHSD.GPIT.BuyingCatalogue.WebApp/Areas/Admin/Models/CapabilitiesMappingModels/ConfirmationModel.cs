using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsConfirmationBanner;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilitiesMappingModels;

public class ConfirmationModel
{
    private readonly bool isSuccessful;

    public ConfirmationModel(
        bool isSuccessful)
    {
        this.isSuccessful = isSuccessful;
    }

    public NhsConfirmationBannerModel.BannerColour Colour => isSuccessful
        ? NhsConfirmationBannerModel.BannerColour.Blue
        : NhsConfirmationBannerModel.BannerColour.Grey;

    public string Title =>
        isSuccessful ? "Capabilities and Epics successfully updated" : "Capabilities and Epics update failed";

    public string Content => isSuccessful
        ? "The updated Capabilities and Epics have been successfully mapped to the solutions and services on the Buying Catalogue."
        : "The Capabilities and Epics have not been mapped to the solutions and services on the Buying Catalogue. Go back to your admin homepage and try again.";
}
