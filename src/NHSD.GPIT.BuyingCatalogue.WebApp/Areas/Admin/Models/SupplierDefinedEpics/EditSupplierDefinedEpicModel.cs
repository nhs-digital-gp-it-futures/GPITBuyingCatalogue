using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics
{
    public sealed class EditSupplierDefinedEpicModel : AddSupplierDefinedEpicDetailsModel
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
            CapabilityList = epic.Capabilities.First().Name;
            foreach (var capability in epic.Capabilities.Skip(1))
            {
                CapabilityList += ", " + capability.Name;
            }

            CanDelete = !epic.IsActive && relatedItems.Count == 0;
        }

        public string CapabilityList { get; set; }

        public bool CanDelete { get; set; }
    }
}
