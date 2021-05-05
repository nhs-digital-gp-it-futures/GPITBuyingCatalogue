using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Results;
using System;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.CreateBuyer
{
    public interface ICreateBuyerService
    {
        Task<Result<string>> Create(Guid primaryOrganisationId, string firstName, string lastName, string phoneNumber, string emailAddress);
    }
}
