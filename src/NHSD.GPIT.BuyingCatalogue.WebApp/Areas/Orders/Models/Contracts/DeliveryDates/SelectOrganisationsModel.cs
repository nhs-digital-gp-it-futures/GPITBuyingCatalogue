using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;

public class SelectOrganisationsModel : NavBaseModel
{
    public SelectOrganisationsModel()
    {
    }

    public SelectOrganisationsModel(OrderWrapper orderWrapper, string internalOrgId, int orderItemId)
    {
        InternalOrgId = internalOrgId;
        CallOffId = orderWrapper.Order.CallOffId;
        OrderItemId = orderItemId;
        Recipients = [.. orderWrapper.DetermineOrderRecipients(orderWrapper.Order.OrderItem(orderItemId))
            .Where(x => !string.Equals(
                x.RecipientOdsCode,
                orderWrapper.Order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode))
            .Select(y => new SelectRecipientToEditModel()
            {
                ParentSublocationName = y.ParentSublocation.SublocationOrganisation?.Name,
                ParentSublocationOdsCode = y.ParentSublocationOdsCode,
                RecipientOrganisationId = y.RecipientOdsOrganisation?.Id,
                RecipientName = y.RecipientOdsOrganisation?.Name,
                RecipientOdsCode = y.RecipientOdsCode,
            })];
    }

    public string InternalOrgId { get; set; }

    public CallOffId CallOffId { get; set; }

    public int? OrderItemId { get; set; }

    public List<SelectRecipientToEditModel> Recipients { get; set; }

    public Dictionary<string, SelectRecipientToEditModel[]> GetRecipientsByLocation()
    {
        return Recipients
            .GroupBy(y => y.ParentSublocationName)
            .Select(x => new KeyValuePair<string, SelectRecipientToEditModel[]>(
                x.Key,
                [.. x.OrderBy(y => y.RecipientName)]))
            .ToDictionary();
    }

    public string GetQueryStringRecipientIds()
    {
        return string.Join(",", Recipients.Select(x => Uri.EscapeDataString(x.RecipientOrganisationId)));
    }
}
