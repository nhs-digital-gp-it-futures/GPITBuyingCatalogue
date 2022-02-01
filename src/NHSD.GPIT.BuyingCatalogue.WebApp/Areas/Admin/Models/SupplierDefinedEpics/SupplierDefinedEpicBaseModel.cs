using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics
{
    public class SupplierDefinedEpicBaseModel : NavBaseModel
    {
        public string Id { get; set; }

        public int? SelectedCapabilityId { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public bool? IsActive { get; set; }

        public SelectList Capabilities { get; set; }

        public IList<SelectListItem> ActiveOptions => new List<SelectListItem>
        {
            new("Active", true.ToString()),
            new("Inactive", false.ToString()),
        };

        public SupplierDefinedEpicBaseModel WithSelectListCapabilities(List<Capability> capabilities)
        {
            Capabilities = new SelectList(
                capabilities
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem(c.Name, c.Id.ToString())),
                "Value",
                "Text");

            return this;
        }
    }
}
