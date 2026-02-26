using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;

public sealed class SublocationQuantityHubModel : NavBaseModel
{
    public const string TitleText = "Quantity of {0}";
    public const string AdviceText = "Select a sublocation to enter the amount you want to order for your practices.";
    public const string AdviceTextPatient = "Review the practice list sizes for the organisations you have added. These numbers will be used to calculate the cost of the solution or service.";

    public SublocationQuantityHubModel()
    {
    }

    public SublocationQuantityHubModel(
        Organisation organisation,
        CatalogueItem catalogueItem,
        IPrice price = null)
    {
        Caption = catalogueItem.Name;
        Title = string.Format(TitleText, catalogueItem.CatalogueItemType.Name().ToLowerInvariant());
        Advice = price?.ProvisioningType switch
        {
            ProvisioningType.Patient => AdviceTextPatient,
            _ => AdviceText,
        };

        OrderingPartyName = organisation.Name;
    }

    public string OrderingPartyName { get; init; }

    public SubLocationModel[] SubLocations { get; set; }
}
