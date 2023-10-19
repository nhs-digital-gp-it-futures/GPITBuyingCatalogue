using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

public record OrderType(OrderTypeEnum Value)
{
    public CatalogueItemType? ToCatalogueItemType
    {
        get
        {
            return Value switch
            {
                OrderTypeEnum.Unknown => null,
                OrderTypeEnum.Solution => (CatalogueItemType?)CatalogueItemType.Solution,
                OrderTypeEnum.AssociatedServiceOther or OrderTypeEnum.AssociatedServiceMerger or OrderTypeEnum.AssociatedServiceSplit => (CatalogueItemType?)CatalogueItemType.AssociatedService,
                _ => throw new InvalidOperationException($"Unhandled {nameof(OrderTypeEnum)} {Value}"),
            };
        }
    }

    public PracticeReorganisationTypeEnum ToPracticeReorganisationType
    {
        get
        {
            return Value switch
            {
                OrderTypeEnum.AssociatedServiceMerger => PracticeReorganisationTypeEnum.Merger,
                OrderTypeEnum.AssociatedServiceSplit => PracticeReorganisationTypeEnum.Split,
                _ => PracticeReorganisationTypeEnum.None,
            };
        }
    }

    public bool UsesSupplierSearch
    {
        get
        {
            return Value switch
            {
                OrderTypeEnum.Solution or OrderTypeEnum.AssociatedServiceOther => true,
                _ => false,
            };
        }
    }
}
