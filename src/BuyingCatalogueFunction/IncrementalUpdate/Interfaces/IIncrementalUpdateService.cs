using System.Threading.Tasks;

namespace BuyingCatalogueFunction.IncrementalUpdate.Interfaces
{
    public interface IIncrementalUpdateService
    {
        Task UpdateOrganisationData();
    }
}
