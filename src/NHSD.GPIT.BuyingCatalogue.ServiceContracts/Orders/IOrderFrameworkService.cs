using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderFrameworkService
    {
        public Task<IList<Framework>> GetFrameworksForOrder(CallOffId callOffId, string internalOrgId, bool isAssociatedServiceOrder);

        public Task SetSelectedFrameworkForOrder(CallOffId callOffId, string internalOrgId, string frameworkId);
    }
}
