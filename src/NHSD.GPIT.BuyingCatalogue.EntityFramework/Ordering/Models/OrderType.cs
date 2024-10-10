using System;
using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

public record OrderType(OrderTypeEnum Value) : IParsable<OrderType>
{
    public CatalogueItemType? ToCatalogueItemType
    {
        get
        {
            return Value switch
            {
                OrderTypeEnum.Unknown => null,
                OrderTypeEnum.Solution => CatalogueItemType.Solution,
                OrderTypeEnum.AssociatedServiceOther or OrderTypeEnum.AssociatedServiceMerger or OrderTypeEnum.AssociatedServiceSplit => CatalogueItemType.AssociatedService,
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

    public bool ImplementationPlanRequired
    {
        get
        {
            return Value switch
            {
                OrderTypeEnum.Solution => true,
                _ => false,
            };
        }
    }

    public bool AssociatedServicesOnly
    {
        get
        {
            return Value switch
            {
                OrderTypeEnum.AssociatedServiceOther or OrderTypeEnum.AssociatedServiceMerger or OrderTypeEnum.AssociatedServiceSplit => true,
                _ => false,
            };
        }
    }

    public bool MergerOrSplit
    {
        get
        {
            return Value switch
            {
                OrderTypeEnum.AssociatedServiceMerger or OrderTypeEnum.AssociatedServiceSplit => true,
                _ => false,
            };
        }
    }

    public static implicit operator OrderType(OrderTypeEnum o) => new(o);

    public string GetPracticeReorganisationRecipientTitle() => Value switch
    {
        OrderTypeEnum.AssociatedServiceSplit => "Service Recipient to be split",
        OrderTypeEnum.AssociatedServiceMerger => "Service Recipient to be retained",
        _ => throw new InvalidOperationException($"Unsupported orderType {Value}"),
    };

    public string GetServiceRecipientsTitle() => Value switch
    {
        OrderTypeEnum.AssociatedServiceSplit => "Service Recipients to receive patients",
        OrderTypeEnum.AssociatedServiceMerger => "Service Recipients to be merged",
        _ => "Service Recipients, planned delivery dates and quantities",
    };

    public static OrderType Parse(string s, IFormatProvider provider)
    {
        if (!TryParse(s, provider, out var result))
        {
            throw new ArgumentException("Could not parse supplied value.", nameof(s));
        }

        return result;
    }

    public static bool TryParse([NotNullWhen(true)] string s, IFormatProvider provider, [MaybeNullWhen(false)] out OrderType result)
    {
        if (Enum.TryParse<OrderTypeEnum>(s, out var value))
        {
            try
            {
                result = value;
                return true;
            }
            catch
            {
                result = OrderTypeEnum.Unknown;
                return false;
            }
        }

        result = OrderTypeEnum.Unknown;
        return false;
    }

    public string GetSolutionNameFromOrder(Order order)
    {
        return AssociatedServicesOnly
                   ? order.AssociatedServicesOnlyDetails.Solution.Name
                   : order.GetSolutionOrderItem()?.CatalogueItem.Name;
    }
}
