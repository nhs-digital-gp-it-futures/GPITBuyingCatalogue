using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IDefaultDeliveryDateService
    {
        Task<DateTime?> GetDefaultDeliveryDate(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId);

        Task<DeliveryDateResult> SetDefaultDeliveryDate(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId, DateTime deliveryDate);
    }
}
