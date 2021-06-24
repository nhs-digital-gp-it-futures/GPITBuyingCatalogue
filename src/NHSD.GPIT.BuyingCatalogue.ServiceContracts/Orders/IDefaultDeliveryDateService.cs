using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IDefaultDeliveryDateService
    {
        // TODO: callOffId should be of type CallOffId
        // TODO: catalogueItemId should be of type CatalogueItemId
        Task<DateTime?> GetDefaultDeliveryDate(string callOffId, string catalogueItemId);

        // TODO: callOffId should be of type CallOffId
        // TODO: catalogueItemId should be of type CatalogueItemId
        Task<DeliveryDateResult> SetDefaultDeliveryDate(string callOffId, string catalogueItemId, DateTime deliveryDate);
    }
}
