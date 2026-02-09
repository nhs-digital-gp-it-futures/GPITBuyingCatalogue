using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;

public sealed class SelectServiceRecipientQuantityModel : NavBaseModel
{
    public const string AdviceText = "Enter the amount you want to order for your practices.";
    public const string AdviceTextMergerSplit = "Review the quantity you’ll be ordering based on the service recipients you’ve selected.";
    public const string AdviceTextPatient =
        "Review the practice list sizes for the organisations you have added."
        + " These numbers will be used to calculate the cost of the solution.";

    public const string QuantityColumnTitleText = "Amount";
    public const string QuantityColumnTitleTextPatient = "Practice list size";
    public const string TitleText = "Quantity of {0}";
    public const string TitleTextPatient = "Review patient list sizes";

    public SelectServiceRecipientQuantityModel()
    {
    }

    public SelectServiceRecipientQuantityModel(
        CatalogueItem catalogueItem,
        IPrice price,
        IEnumerable<ServiceRecipientQuantityDto> serviceRecipients)
    {
        Caption = catalogueItem.Name;
        (Title, Advice) = price.ProvisioningType switch
        {
            ProvisioningType.Patient => (TitleTextPatient, AdviceTextPatient),
            _ => (string.Format(TitleText, catalogueItem.CatalogueItemType.Name()), AdviceText),
        };

        CatalogueItemType = catalogueItem.CatalogueItemType;
        ProvisioningType = price.ProvisioningType;
        BillingPeriod = price.BillingPeriod;

        SubLocations = CreateSubLocations(serviceRecipients ?? []);
    }

    public SelectServiceRecipientQuantityModel(
        OrderType orderType,
        OdsOrganisation practiceReorganisationRecipient,
        CatalogueItem catalogueItem,
        IPrice price,
        IEnumerable<ServiceRecipientQuantityDto> serviceRecipients,
        IEnumerable<ServiceRecipientQuantityDto> previousRecipients)
        : this(catalogueItem, price, serviceRecipients)
    {
        OrderType = orderType;
        PracticeReorganisationRecipient = $"{practiceReorganisationRecipient?.Name} ({practiceReorganisationRecipient?.Id})";
        PreviouslySelected = CreateSubLocations(previousRecipients ?? []) ?? [];

        if (orderType.MergerOrSplit)
        {
            Advice = AdviceTextMergerSplit;
        }
    }

    public OrderType OrderType { get; set; }

    public CatalogueItemType CatalogueItemType { get; set; }

    public string PracticeReorganisationRecipient { get; set; }

    public ProvisioningType ProvisioningType { get; set; }

    public TimeUnit? BillingPeriod { get; set; }

    public SubLocationModel[] PreviouslySelected { get; set; } = [];

    public SubLocationModel[] SubLocations { get; set; }

    public RoutingSource? Source { get; set; }

    public RoutingFields RoutingFields { get; init; }

    public string OrderingPartyName { get; init; }

    public bool ShouldShowInset => ProvisioningType == ProvisioningType.Patient;

    public string QuantityColumnTitle => ProvisioningType switch
    {
        ProvisioningType.Patient => QuantityColumnTitleTextPatient,
        _ => QuantityColumnTitleText,
    };

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
