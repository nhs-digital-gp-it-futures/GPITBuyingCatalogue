using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IDefaultDeliveryDateService
    {
        Task<DateTime?> GetDefaultDeliveryDate(string callOffId, string catalogueItemId);

        Task<DeliveryDateResult> SetDefaultDeliveryDate(string callOffId, string catalogueItemId, DateTime deliveryDate);
    }
}
