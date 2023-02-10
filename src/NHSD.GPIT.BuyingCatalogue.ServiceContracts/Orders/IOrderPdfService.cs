using System.IO;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

public interface IOrderPdfService
{
    Task<MemoryStream> CreateOrderSummaryPdf(Order order);
}
