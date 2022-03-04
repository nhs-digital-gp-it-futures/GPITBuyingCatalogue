using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    public interface IGpPracticeCacheService
    {
        Task<int?> GetNumberOfPatients(string odsCode);
    }
}
