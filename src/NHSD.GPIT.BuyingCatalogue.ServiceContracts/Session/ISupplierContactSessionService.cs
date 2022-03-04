using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session
{
    public interface ISupplierContactSessionService
    {
        SupplierContact GetSupplierContact(CallOffId callOffId, int supplierId);

        void SetSupplierContact(CallOffId callOffId, int supplierId, SupplierContact contact);
    }
}
