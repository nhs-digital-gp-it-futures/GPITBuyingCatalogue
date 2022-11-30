using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users
{
    public interface ICreateUserService
    {
        Task<AspNetUser> Create(
            int primaryOrganisationId,
            string firstName,
            string lastName,
            string emailAddress,
            string organisationFunction,
            bool isDisabled = false);
    }
}
