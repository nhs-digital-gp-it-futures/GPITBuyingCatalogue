﻿using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;

public class SelectServiceRecipientQuantityModel : NavBaseModel
{
    public const string AdviceText = "Enter the quantity you want for each practice for the duration of your order.";
    public const string AdviceTextPatient = "We’ve included the latest practice list sizes published by the NHS.";
    public const string AdviceTextMergerSplit = "Review the quantity you’ll be ordering based on the Service Recipients you’ve selected.";
    public const string QuantityColumnTitleText = "Quantity";
    public const string QuantityColumnTitleTextPatient = "Practice list size";
    public const string TitleText = "Quantity of {0}";

    public SelectServiceRecipientQuantityModel()
    {
    }

    public SelectServiceRecipientQuantityModel(
        CatalogueItem catalogueItem,
        IPrice price,
        IEnumerable<ServiceRecipientDto> serviceRecipients)
    {
        Caption = catalogueItem.Name;
        Title = string.Format(TitleText, catalogueItem.CatalogueItemType.Name());
        Advice = price.ProvisioningType switch
        {
            ProvisioningType.Patient => AdviceTextPatient,
            _ => AdviceText,
        };
        ProvisioningType = price.ProvisioningType;
        BillingPeriod = price.BillingPeriod;

        SubLocations = CreateSubLocations(serviceRecipients ?? []);
    }

    public SelectServiceRecipientQuantityModel(
        OrderType orderType,
        OdsOrganisation practiceReorganisationRecipient,
        CatalogueItem catalogueItem,
        IPrice price,
        IEnumerable<ServiceRecipientDto> serviceRecipients,
        IEnumerable<ServiceRecipientDto> previousRecipients)
        : this(catalogueItem, price, serviceRecipients)
    {
        OrderType = orderType;
        PracticeReorganisationRecipient = $"{practiceReorganisationRecipient?.Name} ({practiceReorganisationRecipient?.Id})";
        if (orderType.MergerOrSplit)
        {
            Advice = AdviceTextMergerSplit;
        }

        PreviouslySelected = CreateSubLocations(previousRecipients ?? []) ?? [];
    }

    public OrderType OrderType { get; set; }

    public string PracticeReorganisationRecipient { get; set; }

    public ProvisioningType ProvisioningType { get; set; }

    public TimeUnit? BillingPeriod { get; set; }

    public SubLocationModel[] PreviouslySelected { get; set; } = [];

    public SubLocationModel[] SubLocations { get; set; }

    public RoutingSource? Source { get; set; }

    public bool ShouldShowInset => ProvisioningType is ProvisioningType.Patient;

    public string QuantityColumnTitle => ProvisioningType switch
    {
        ProvisioningType.Patient => QuantityColumnTitleTextPatient,
        _ => QuantityColumnTitleText,
    };

    private static SubLocationModel[] CreateSubLocations(IEnumerable<ServiceRecipientDto> recipients)
    {
        return recipients
            .GroupBy(x => x.Location)
            .Select(
                x => new SubLocationModel(
                    x.Key,
                    x.Select(CreateServiceRecipient).ToArray()))
            .ToArray();
    }

    private static ServiceRecipientQuantityModel CreateServiceRecipient(ServiceRecipientDto recipient)
    {
        var recipientQuantityModel = new ServiceRecipientQuantityModel
        {
            OdsCode = recipient.OdsCode,
            Name = recipient.Name,
            Quantity = recipient.Quantity ?? 0,
            InputQuantity = recipient.Quantity.HasValue
                ? $"{recipient.Quantity}"
                : string.Empty,
        };

        return recipientQuantityModel;
    }
}
