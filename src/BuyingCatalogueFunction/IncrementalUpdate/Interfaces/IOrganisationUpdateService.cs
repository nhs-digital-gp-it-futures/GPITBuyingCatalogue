using System;
using System.Threading.Tasks;
using BuyingCatalogueFunction.IncrementalUpdate.Models;

namespace BuyingCatalogueFunction.IncrementalUpdate.Interfaces
{
    public interface IOrganisationUpdateService
    {
        Task<DateTime> GetLastRunDate();

        Task SetLastRunDate(DateTime lastRunDate);

        Task IncrementalUpdate(IncrementalUpdateData data);
    }
}
