using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices
{
    public sealed class CatalogueItemAssociatedServicesModel : NavBaseModel
    {
        public CatalogueItemAssociatedServicesModel()
        {
        }

        public CatalogueItemAssociatedServicesModel(
            CatalogueItem catalogueItem,
            IEnumerable<AssociatedService> associatedServices)
        {
            CatalogueItemId = catalogueItem.Id;
            CatalogueItemName = catalogueItem.Name;
            CatalogueItemType = catalogueItem.CatalogueItemType;

            SelectableAssociatedServices = associatedServices
                .Select(s => new SelectableAssociatedService
                {
                    Name = s.CatalogueItem.Name,
                    Description = s.Description,
                    OrderGuidance = s.OrderGuidance,
                    CatalogueItemId = s.CatalogueItemId,
                    Selected = catalogueItem.SupplierServiceAssociations.Any(ssa => ssa.AssociatedServiceId == s.CatalogueItemId),
                    PracticeReorganisation = s.PracticeReorganisationType,
                })
                .ToList();
        }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string CatalogueItemName { get; set; }

        public CatalogueItemType CatalogueItemType { get; set; }

        public List<SelectableAssociatedService> SelectableAssociatedServices { get; } = new();

        public string CatalogueItemTypeName => CatalogueItemType.DisplayName().ToLowerInvariant();

        public SolutionMergerAndSplitTypesModel SolutionMergerAndSplits => new(
            CatalogueItemName,
            SelectableAssociatedServices
                .Where(s => s.Selected)
                .Select(s => s.PracticeReorganisation));
    }
}
