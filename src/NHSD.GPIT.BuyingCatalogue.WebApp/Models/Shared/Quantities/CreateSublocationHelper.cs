using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;

public static class CreateSublocationHelper
{
    public static SubLocationModel[] CreateSubLocations(IEnumerable<ServiceRecipientQuantityDto> recipients)
    {
        return recipients
            .GroupBy(x => (x.ParentSublocationOdsCode, x.Location))
            .Select(x => new SubLocationModel(
                x.Key.ParentSublocationOdsCode,
                x.Key.Location,
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
