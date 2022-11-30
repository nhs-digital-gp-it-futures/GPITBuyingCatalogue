using System.Threading.Tasks;

namespace BuyingCatalogueFunction.Services.IncrementalUpdate.Interfaces
{
    public interface IIncrementalUpdateService
    {
        Task UpdateOrganisationData();
    }
}
