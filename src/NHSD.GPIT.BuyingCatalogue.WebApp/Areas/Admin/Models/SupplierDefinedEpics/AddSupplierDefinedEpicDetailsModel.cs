using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics
{
    public class AddSupplierDefinedEpicDetailsModel : NavBaseModel
    {
        public string Id { get; set; }

        public string SelectedCapabilityIds { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        [StringLength(1500)]
        public string Description { get; set; }

        public bool? IsActive { get; set; }

        public IList<SelectOption<string>> ActiveOptions => new List<SelectOption<string>>
        {
            new("Active", true.ToString()),
            new("Inactive", false.ToString()),
        };
    }
}
