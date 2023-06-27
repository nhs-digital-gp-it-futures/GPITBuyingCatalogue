using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics
{
    public class SupplierDefinedEpicSelectCapabilities : NavBaseModel
    {
        public string Id { get; set; }

        public int? SelectedCapabilityId { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        [StringLength(1500)]
        public string Description { get; set; }

        public bool? IsActive { get; set; }

        public IList<SelectOption<string>> Capabilities { get; set; }

        public IList<SelectOption<string>> ActiveOptions => new List<SelectOption<string>>
        {
            new("Active", true.ToString()),
            new("Inactive", false.ToString()),
        };

        public SupplierDefinedEpicSelectCapabilities WithSelectListCapabilities(List<Capability> capabilities)
        {
            Capabilities = capabilities
                .OrderBy(c => c.Name)
                .Select(c => new SelectOption<string>(c.Name, c.Id.ToString()))
                .ToList();

            return this;
        }
    }
}
