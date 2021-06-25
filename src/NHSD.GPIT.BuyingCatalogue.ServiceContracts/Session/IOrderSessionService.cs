using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session
{
    public interface IOrderSessionService
    {
        public CreateOrderItemModel GetOrderStateFromSession();

        public void SetOrderStateToSession(CreateOrderItemModel model);

        public Task<bool> InitialiseStateForEdit(string odsCode, CallOffId callOffId, CatalogueItemId catalogueSolutionId);

        public void SetPrice(EntityFramework.Models.GPITBuyingCatalogue.CataloguePrice cataloguePrice);
    }
}
