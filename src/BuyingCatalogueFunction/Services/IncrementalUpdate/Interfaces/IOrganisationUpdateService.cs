using System;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Models.IncrementalUpdate;

namespace BuyingCatalogueFunction.Services.IncrementalUpdate.Interfaces
{
    public interface IOrganisationUpdateService
    {
        Task<DateTime> GetLastRunDate();

        Task SetLastRunDate(DateTime lastRunDate);

        Task IncrementalUpdate(IncrementalUpdateData data);
    }
}
