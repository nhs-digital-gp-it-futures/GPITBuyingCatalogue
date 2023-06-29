using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    public interface IOdsService
    {
        Task<(OdsOrganisation Organisation, string Error)> GetOrganisationByOdsCode(string odsCode);

        Task<IEnumerable<ServiceRecipient>> GetServiceRecipientsByParentInternalIdentifier(string internalIdentifier);

        Task<IEnumerable<ServiceRecipient>> GetServiceRecipientsById(
            string internalIdentifier,
            IEnumerable<string> odsCodes);

        Task UpdateOrganisationDetails(string odsCode);
    }
}
