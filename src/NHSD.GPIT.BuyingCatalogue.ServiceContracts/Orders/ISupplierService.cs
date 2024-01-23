using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface ISupplierService
    {
        public Task<List<Supplier>> GetAllSuppliersByOrderType(OrderType orderType);

        public Task<bool> SuppliersAvailableByOrderType(OrderType orderType);

        public Task<Supplier> GetSupplierFromBuyingCatalogue(int id);

        public Task AddOrderSupplier(CallOffId callOffId, string internalOrgId, int supplierId);

        public Task AddOrUpdateOrderSupplierContact(CallOffId callOffId, string internalOrgId, SupplierContact contact);

        public Task SetSupplierSection(Order order, Supplier supplier, Contact contact);
    }
}
