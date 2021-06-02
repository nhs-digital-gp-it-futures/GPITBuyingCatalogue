using System;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface ICommencementDateService
    {
        Task SetCommencementDate(string callOffId, DateTime? commencementDate);
    }
}
