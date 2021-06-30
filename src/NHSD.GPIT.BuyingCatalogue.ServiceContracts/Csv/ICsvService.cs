using System.IO;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv
{
    public interface ICsvService
    {
        public Task CreateFullOrderCsvAsync(Order order, MemoryStream stream);

        public Task CreatePatientNumberCsvAsync(Order order, MemoryStream stream);
    }
}
