using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Results;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.CreateBuyer
{
    public interface IAspNetUserValidator
    {
        Task<Result> ValidateAsync(AspNetUser user);
    }
}
