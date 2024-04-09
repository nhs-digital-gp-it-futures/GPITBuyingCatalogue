using System.IO;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv
{
    public interface ICsvService
    {
        public Task CreateFullOrderCsvAsync(int orderId, OrderType orderType, MemoryStream stream, bool showRevisions = false);
    }
}
