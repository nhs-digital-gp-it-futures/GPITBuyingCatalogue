using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices
{
    public sealed class AssociatedServicesModel : NavBaseModel
    {
        public AssociatedServicesModel()
        {
        }

        public AssociatedServicesModel(CatalogueItem catalogueItem, List<CatalogueItem> associatedServices)
        {
            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}";
            BackLinkText = "Go back";
            Solution = catalogueItem;
            SelectableAssociatedServices = associatedServices.Select(s => new SelectableAssociatedService
            {
                Name = s.Name,
                Description = s.AssociatedService.Description,
                OrderGuidance = s.AssociatedService.OrderGuidance,
                CatalogueItemId = s.AssociatedService.CatalogueItemId,
                Selected = catalogueItem.SupplierServiceAssociations.Any(ssa => ssa.AssociatedServiceId == s.Id),
            }).ToList();
            AssociatedServices = associatedServices;
        }

        public CatalogueItem Solution { get; set; }

        public List<SelectableAssociatedService> SelectableAssociatedServices { get; set; }

        public List<CatalogueItem> AssociatedServices { get; set; }

        public TaskProgress Status()
        {
            if (!AssociatedServices.Any())
                return TaskProgress.Optional;

            var statuses = AssociatedServices.Select(c => Status(c));

            if (statuses.All(s => s == TaskProgress.Completed))
                return TaskProgress.Completed;

            return statuses.Any(s => s == TaskProgress.InProgress) ? TaskProgress.InProgress : TaskProgress.Optional;
        }

        private TaskProgress Status(CatalogueItem associatedService)
        {
            // TODO - Easier to do this once Details and Price pages are done
            return TaskProgress.Completed;
        }

        public sealed class SelectableAssociatedService
        {
            public string Name { get; set; }

            public CatalogueItemId CatalogueItemId { get; set; }

            public string Description { get; set; }

            public string OrderGuidance { get; set; }

            public bool Selected { get; set; }
        }
    }
}
