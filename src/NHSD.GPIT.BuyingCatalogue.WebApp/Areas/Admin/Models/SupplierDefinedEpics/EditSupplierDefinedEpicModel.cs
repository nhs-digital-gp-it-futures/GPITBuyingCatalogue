using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics
{
    public sealed class EditSupplierDefinedEpicModel : SupplierDefinedEpicBaseModel
    {
        public EditSupplierDefinedEpicModel()
        {
        }

        public EditSupplierDefinedEpicModel(Epic epic, IList<CatalogueItem> relatedItems)
        {
            Id = epic.Id;
            Name = epic.Name;
            Description = epic.Description;
            IsActive = epic.IsActive;
            SelectedCapabilityId = epic.Capabilities.First().Id;

            RelatedItems = relatedItems;

            CanDelete = !epic.IsActive && relatedItems.Count == 0;
        }

        public IList<CatalogueItem> RelatedItems { get; set; }

        public bool CanDelete { get; set; }
    }
}
