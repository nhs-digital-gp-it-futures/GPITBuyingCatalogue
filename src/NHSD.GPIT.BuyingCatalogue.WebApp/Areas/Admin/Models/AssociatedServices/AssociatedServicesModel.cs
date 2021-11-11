using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
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
            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}";
            Solution = catalogueItem;
            SelectableAssociatedServices = associatedServices.Select(s => new SelectableAssociatedService
            {
                Name = s.Name,
                Description = s.AssociatedService.Description,
                PublishedStatus = s.PublishedStatus,
                CatalogueItemId = s.AssociatedService.CatalogueItemId,
                Selected = catalogueItem.SupplierServiceAssociations.Any(ssa => ssa.AssociatedServiceId == s.Id),
            }).ToList();
            AssociatedServices = associatedServices;
        }

        public CatalogueItem Solution { get; }

        public List<SelectableAssociatedService> SelectableAssociatedServices { get; } = new();

        public IReadOnlyList<CatalogueItem> AssociatedServices { get; }

        public TaskProgress Status()
        {
            if (!AssociatedServices.Any())
                return TaskProgress.Optional;

            var statuses = AssociatedServices.Select(Status).ToList();

            if (statuses.All(s => s == TaskProgress.Completed))
                return TaskProgress.Completed;

            return statuses.Any(s => s == TaskProgress.InProgress) ? TaskProgress.InProgress : TaskProgress.Optional;
        }

        private TaskProgress Status(CatalogueItem associatedService)
        {
            // TODO - Easier to do this once Details and Price pages are done
            return TaskProgress.Completed;
        }
    }
}
