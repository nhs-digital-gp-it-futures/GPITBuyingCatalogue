using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices
{
    public sealed class SolutionAssociatedServicesModel : NavBaseModel
    {
        public SolutionAssociatedServicesModel()
        {
        }

        public SolutionAssociatedServicesModel(
            CatalogueItem catalogueItem,
            IEnumerable<AssociatedService> associatedServices)
        {
            SolutionId = catalogueItem.Id;
            CatalogueItemName = catalogueItem.Name;

            SelectableAssociatedServices = associatedServices
                .Select(s => new SelectableAssociatedService
                {
                    Name = s.CatalogueItem.Name,
                    Description = s.Description,
                    PublishedStatus = s.CatalogueItem.PublishedStatus,
                    CatalogueItemId = s.CatalogueItemId,
                    Selected = catalogueItem.SupplierServiceAssociations.Any(ssa => ssa.AssociatedServiceId == s.CatalogueItemId),
                    PracticeReorganisation = s.PracticeReorganisationType,
                })
                .ToList();
        }

        public CatalogueItemId SolutionId { get; set; }

        public string CatalogueItemName { get; set; }

        public List<SelectableAssociatedService> SelectableAssociatedServices { get; } = new();

        public SolutionMergerAndSplitTypesModel SolutionMergerAndSplits => new(
            CatalogueItemName,
            SelectableAssociatedServices
                .Where(s => s.Selected)
                .Select(s => s.PracticeReorganisation));
    }
}
