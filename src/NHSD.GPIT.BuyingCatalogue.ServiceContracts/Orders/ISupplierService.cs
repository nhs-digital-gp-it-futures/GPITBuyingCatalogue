using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface ISupplierService
    {
        public Task<IList<EntityFramework.Models.BuyingCatalogue.Supplier>> GetList(
            string searchString,
            EntityFramework.Models.BuyingCatalogue.CatalogueItemType catalogueItemType,
            PublicationStatus publicationStatus);
    }
}
