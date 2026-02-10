using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;

public sealed class ConfirmQuantitiesModel : NavBaseModel
{
    internal const string AdviceText = "Review the total amount you have added to the practices in this {0}.";
    internal const string AdvicePatientText = "Review the patient totals for the organisations you have added in this {0}";

    public ConfirmQuantitiesModel()
    {
    }

    public ConfirmQuantitiesModel(
        CatalogueItem catalogueItem,
        OrderItemPrice price,
        List<ServiceRecipientQuantityDto> recipients)
        : this(catalogueItem, price, recipients, "order")
    {
    }

    public ConfirmQuantitiesModel(
        CatalogueItem catalogueItem,
        CompetitionCatalogueItemPrice price,
        List<ServiceRecipientQuantityDto> recipients)
        : this(catalogueItem, price, recipients, "competition")
    {
    }

    private ConfirmQuantitiesModel(
        CatalogueItem catalogueItem,
        IPrice price,
        List<ServiceRecipientQuantityDto> recipients,
        string process)
    {
        Title = "Confirm quantities";
        Advice = string.Format(price.ProvisioningType is ProvisioningType.Patient ? AdvicePatientText : AdviceText, process);
        Sublocations = recipients.GroupBy(x => x.Location)
            .ToDictionary(
                x => x.Key,
                v => v.ToList());

        Caption = catalogueItem.Name;
    }

    public IDictionary<string, List<ServiceRecipientQuantityDto>> Sublocations { get; set; }

    public string ContinueLink { get; set; }
}
