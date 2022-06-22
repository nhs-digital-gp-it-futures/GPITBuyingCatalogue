using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderItemFundingService
    {
        Task<OrderItemFundingType> GetFundingType(OrderItem item);
    }
}
