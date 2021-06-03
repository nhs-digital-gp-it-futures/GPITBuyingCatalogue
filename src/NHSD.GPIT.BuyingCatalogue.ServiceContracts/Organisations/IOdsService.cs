using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    public interface IOdsService
    {
        Task<OdsOrganisation> GetOrganisationByOdsCode(string odsCode);

        Task<IEnumerable<ServiceRecipient>> GetServiceRecipientsByParentOdsCode(string odsCode);
    }
}
