using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    public interface IOdsService
    {
        Task<(OdsOrganisation Organisation, string Error)> GetValidatedBuyerOrganisationByOdsCode(string odsCode);

        Task<IEnumerable<ServiceRecipient>> GetServiceRecipientsByParentInternalIdentifier(string internalIdentifier);

        Task<IReadOnlyList<OdsOrganisation>> GetSublocationsByParentOdsCode(string parentOdsCode);

        Task<IReadOnlyList<ServiceRecipient>> GetServiceRecipientsBySublocation(string sublocationOdsCode);

        Task<IReadOnlyList<ServiceRecipient>> GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
            string internalIdentifier,
            IEnumerable<string> odsCodes);

        Task UpdateOrganisationDetails(string odsCode);
    }
}
