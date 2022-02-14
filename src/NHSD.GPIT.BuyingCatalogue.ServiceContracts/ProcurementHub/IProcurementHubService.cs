using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.ProcurementHub
{
    public interface IProcurementHubService
    {
        Task ContactProcurementHub(ProcurementHubRequest request);
    }
}
