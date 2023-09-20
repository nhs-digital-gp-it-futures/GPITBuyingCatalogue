using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface ISupplierTemporalService
    {
        public Task<Supplier> GetSupplierByDate(int id, DateTime dateTime);
    }
}
