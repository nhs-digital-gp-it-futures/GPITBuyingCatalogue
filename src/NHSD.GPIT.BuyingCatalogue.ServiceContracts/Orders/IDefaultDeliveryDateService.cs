using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IDefaultDeliveryDateService
    {
        Task<DateTime?> GetDefaultDeliveryDate(CallOffId callOffId, string odsCode, CatalogueItemId catalogueItemId);

        Task<DeliveryDateResult> SetDefaultDeliveryDate(CallOffId callOffId, string odsCode, CatalogueItemId catalogueItemId, DateTime deliveryDate);
    }
}
