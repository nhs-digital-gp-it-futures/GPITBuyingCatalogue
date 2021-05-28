using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IFundingSourceService
    {
        Task SetFundingSource(string callOffId, bool? onlyGms);
    }
}
