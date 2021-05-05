using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    public interface IOrganisationsService
    {
        Task<List<Organisation>> GetAllOrganisations();

        Task<Organisation> GetOrganisation(Guid id);

        Task<Guid> AddOdsOrganisation(OdsOrganisation odsOrganisation, bool agreementSigned);

        Task UpdateCatalogueAgreementSigned(Guid organisationId, bool signed);
    }
}
