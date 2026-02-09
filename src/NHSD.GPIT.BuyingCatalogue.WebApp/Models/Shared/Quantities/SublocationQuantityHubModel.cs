using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;

public sealed class SublocationQuantityHubModel : NavBaseModel
{
    public const string TitleText = "Quantity of {0}";
    public const string AdviceText = "Select a sublocation to enter the amount you want to order for your practices.";
    public const string AdviceTextPatient = "Review the practice list sizes for the organisations you have added. These numbers will be used to calculate the cost of the solution or service.";

    public SublocationQuantityHubModel(
        Organisation organisation,
        CatalogueItem catalogueItem,
        IPrice price,
        IEnumerable<ServiceRecipientQuantityDto> serviceRecipients)
    {
        Caption = catalogueItem.Name;
        Title = string.Format(TitleText, catalogueItem.CatalogueItemType.Name().ToLowerInvariant());
        Advice = price.ProvisioningType switch
        {
            ProvisioningType.Patient => AdviceTextPatient,
            _ => AdviceText,
        };

        OrderingPartyName = organisation.Name;
        SubLocations = CreateSubLocations(serviceRecipients ?? []);
    }

    public string OrderingPartyName { get; init; }

    public SubLocationModel[] SubLocations { get; set; }

    public RoutingSource? Source { get; set; }

    public RoutingFields RoutingFields { get; init; }

    private static SubLocationModel[] CreateSubLocations(IEnumerable<ServiceRecipientQuantityDto> recipients)
    {
        return recipients
            .GroupBy(x => x.Location)
            .Select(x => new SubLocationModel(
                x.Key,
                x.Select(CreateServiceRecipient).ToArray()))
            .ToArray();
    }

    private static ServiceRecipientQuantityModel CreateServiceRecipient(ServiceRecipientQuantityDto recipientQuantity)
    {
        var recipientQuantityModel = new ServiceRecipientQuantityModel
        {
            ParentSublocationOdsCode = recipientQuantity.ParentSublocationOdsCode,
            RecipientOdsCode = recipientQuantity.RecipientOdsCode,
            Name = recipientQuantity.Name,
            Quantity = recipientQuantity.Quantity ?? 0,
            InputQuantity = recipientQuantity.Quantity.HasValue
                ? $"{recipientQuantity.Quantity}"
                : string.Empty,
        };

        return recipientQuantityModel;
    }
}
