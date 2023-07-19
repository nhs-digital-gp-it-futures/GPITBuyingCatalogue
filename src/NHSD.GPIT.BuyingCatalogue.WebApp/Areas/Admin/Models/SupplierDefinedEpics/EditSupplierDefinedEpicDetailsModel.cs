using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics
{
    public sealed class EditSupplierDefinedEpicDetailsModel : AddSupplierDefinedEpicDetailsModel
    {
        public EditSupplierDefinedEpicDetailsModel()
        {
        }

        public EditSupplierDefinedEpicDetailsModel(Epic epic, IList<CatalogueItem> relatedItems)
        {
            Id = epic.Id;
            Name = epic.Name;
            Description = epic.Description;
            IsActive = epic.IsActive;

            RelatedItems = relatedItems;
        }

        public IList<CatalogueItem> RelatedItems { get; set; }
    }
}
