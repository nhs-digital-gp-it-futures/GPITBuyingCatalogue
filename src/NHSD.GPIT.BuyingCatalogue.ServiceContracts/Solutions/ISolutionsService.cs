using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Admin;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public interface ISolutionsService
    {
        Task<CatalogueItem> GetSolutionThin(CatalogueItemId solutionId);

        Task<CatalogueItem> GetSolutionWithBasicInformation(CatalogueItemId solutionId);

        Task<CatalogueItem> GetSolutionWithCapabilities(CatalogueItemId solutionId);

        Task<CatalogueItem> GetSolutionWithServiceLevelAgreements(CatalogueItemId solutionId);

        Task<CatalogueItem> GetSolutionWithSupplierDetails(CatalogueItemId solutionId);

        Task<CatalogueItem> GetSolutionWithWorkOffPlans(CatalogueItemId solutionId);

        Task<CatalogueItem> GetSolutionWithServiceAssociations(CatalogueItemId solutionId);

        Task<CatalogueItem> GetSolutionWithCataloguePrice(CatalogueItemId solutionId);

        Task<CatalogueItem> GetSolutionWithDataProcessingInformation(CatalogueItemId solutionId);

        Task<SolutionLoadingStatusesModel> GetSolutionLoadingStatuses(CatalogueItemId solutionId);

        Task<CatalogueItem> GetSolutionByName(string solutionName);

        Task<bool> CatalogueSolutionExistsWithName(string solutionName, CatalogueItemId currentCatalogueItemId = default);

        Task<CatalogueItem> GetSolutionCapability(CatalogueItemId catalogueItemId, int capabilityId);

        Task<IList<Standard>> GetSolutionStandardsForMarketing(CatalogueItemId catalogueItemId);

        Task<IList<Standard>> GetSolutionStandardsForEditing(CatalogueItemId catalogueItemId);

        Task<List<CatalogueItem>> GetPublishedAdditionalServicesForSolution(CatalogueItemId solutionId);

        Task<List<CatalogueItem>> GetPublishedAssociatedServicesForSolution(CatalogueItemId solutionId);

        Task<CatalogueItemContentStatus> GetContentStatusForCatalogueItem(CatalogueItemId solutionId);

        Task SaveSolutionDetails(CatalogueItemId id, string solutionName, int supplierId, bool isPilotSolution, SolutionCategory? solutionCategory, IList<FrameworkModel> selectedFrameworks);

        Task SaveSolutionDescription(CatalogueItemId solutionId, string summary, string description, string link);

        Task SaveSolutionFeatures(CatalogueItemId solutionId, string[] features);

        Task SaveImplementationDetail(CatalogueItemId solutionId, string detail);

        Task<ApplicationTypeDetail> GetApplicationType(CatalogueItemId solutionId);

        Task SaveApplicationType(CatalogueItemId solutionId, ApplicationTypeDetail applicationTypeDetail);

        Task DeleteApplicationType(CatalogueItemId solutionId, ApplicationType applicationType);

        Task SaveHosting(CatalogueItemId solutionId, Hosting hosting);

        Task SaveSupplierDescriptionAndLink(int supplierId, string description, string link);

        Task SaveSupplierContacts(SupplierContactsModel model);

        Task<List<CatalogueItem>> GetSupplierSolutions(int? supplierId, string selectedFrameworkId);

        Task<List<CatalogueItem>> GetSupplierSolutionsWithAssociatedServices(int? supplierId, PracticeReorganisationTypeEnum practiceReorganisationType, string selectedFrameworkId);

        Task<IList<CatalogueItem>> GetAllSolutions(PublicationStatus? publicationStatus = null);

        Task<IList<CatalogueItem>> GetAllSolutionsForSearchTerm(string searchTerm);

        Task<CatalogueItemId> AddCatalogueSolution(CreateSolutionModel model);

        Task<IList<Framework>> GetAllFrameworks();

        Task<bool> SupplierHasSolutionName(int supplierId, string solutionName);

        Task SaveContacts(CatalogueItemId solutionId, IList<SupplierContact> supplierContacts);

        Task<List<WorkOffPlan>> GetWorkOffPlans(CatalogueItemId solutionId);
    }
}
