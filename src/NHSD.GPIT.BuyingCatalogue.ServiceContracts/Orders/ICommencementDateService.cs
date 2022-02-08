using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface ICommencementDateService
    {
        Task SetCommencementDate(CallOffId callOffId, string odsCode, DateTime? commencementDate, int? initialPeriod, int? maximumTerm);
    }
}
