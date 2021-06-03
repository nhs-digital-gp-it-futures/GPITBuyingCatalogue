using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface ISupplierService
    {
        public Task<List<EntityFramework.Models.GPITBuyingCatalogue.Supplier>> GetListFromBuyingCatalogue(
            string searchString,
            EntityFramework.Models.GPITBuyingCatalogue.CatalogueItemType catalogueItemType,
            PublicationStatus publicationStatus);

        public Task<EntityFramework.Models.GPITBuyingCatalogue.Supplier> GetSupplierFromBuyingCatalogue(string id);

        public Task AddOrderSupplier(string callOffId, string supplierId);

        public Task AddOrUpdateOrderSupplierContact(string callOffId, Contact contact);

        public Task SetSupplierSection(Order order, EntityFramework.Models.Ordering.Supplier supplier, Contact contact);
    }
}
