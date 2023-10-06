using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices
{
    public sealed class AssociatedServicesModel : NavBaseModel
    {
        public AssociatedServicesModel()
        {
        }

        public AssociatedServicesModel(CatalogueItem catalogueItem, IReadOnlyList<CatalogueItem> associatedServices)
        {
            SolutionId = catalogueItem.Id;
            SolutionName = catalogueItem.Name;

            SelectableAssociatedServices = associatedServices.Select(s => new SelectableAssociatedService
            {
                Name = s.Name,
                Description = s.AssociatedService.Description,
                PublishedStatus = s.PublishedStatus,
                CatalogueItemId = s.AssociatedService.CatalogueItemId,
                Selected = catalogueItem.SupplierServiceAssociations.Any(ssa => ssa.AssociatedServiceId == s.Id),
                PracticeReorganisation = s.AssociatedService.PracticeReorganisationType,
            }).ToList();
        }

        public CatalogueItemId SolutionId { get; set; }

        public string SolutionName { get; set; }

        public List<SelectableAssociatedService> SelectableAssociatedServices { get; } = new();

        public SolutionMergerAndSplitTypesModel SolutionMergerAndSplits => new SolutionMergerAndSplitTypesModel(SolutionName, SelectableAssociatedServices
            .Where(s => s.Selected)
            .Select(s => s.PracticeReorganisation));
    }
}
