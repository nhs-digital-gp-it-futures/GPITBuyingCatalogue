using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    public interface IOrganisationsService
    {
        Task<IList<Organisation>> GetAllOrganisations();

        Task<Organisation> GetOrganisation(int id);

        Task<Organisation> GetOrganisationByInternalIdentifier(string internalIdentifier);

        Task<List<Organisation>> GetOrganisationsByInternalIdentifiers(string[] internalIdentifiers);

        Task<List<Organisation>> GetOrganisationsBySearchTerm(string searchTerm);

        Task<(int OrganisationId, string Error)> AddCcgOrganisation(OdsOrganisation odsOrganisation, bool agreementSigned);

        Task UpdateCatalogueAgreementSigned(int organisationId, bool signed);

        Task<List<Organisation>> GetUnrelatedOrganisations(int organisationId);

        Task<List<Organisation>> GetRelatedOrganisations(int organisationId);

        Task AddRelatedOrganisations(int organisationId, int relatedOrganisationId);

        Task RemoveRelatedOrganisations(int organisationId, int relatedOrganisationId);
    }
}
