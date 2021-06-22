using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    public interface IOrganisationsService
    {
        Task<IList<Organisation>> GetAllOrganisations();

        Task<Organisation> GetOrganisation(Guid id);

        Task<Organisation> GetOrganisationByOdsCode(string odsCode);

        Task<List<Organisation>> GetOrganisationsByOdsCodes(string[] odsCodes);

        Task<Guid> AddOdsOrganisation(OdsOrganisation odsOrganisation, bool agreementSigned);

        Task UpdateCatalogueAgreementSigned(Guid organisationId, bool signed);

        Task<List<Organisation>> GetUnrelatedOrganisations(Guid organisationId);

        Task<List<Organisation>> GetRelatedOrganisations(Guid organisationId);

        Task AddRelatedOrganisations(Guid organisationId, Guid relatedOrganisationId);

        Task RemoveRelatedOrganisations(Guid organisationId, Guid relatedOrganisationId);
    }
}
