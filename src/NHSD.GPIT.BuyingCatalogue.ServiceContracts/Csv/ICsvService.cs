using System.IO;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv
{
    public interface ICsvService
    {
        public Task CreateFullOrderCsvAsync(int orderId, MemoryStream stream);

        public Task CreatePatientNumberCsvAsync(int orderId, MemoryStream stream);
    }
}
