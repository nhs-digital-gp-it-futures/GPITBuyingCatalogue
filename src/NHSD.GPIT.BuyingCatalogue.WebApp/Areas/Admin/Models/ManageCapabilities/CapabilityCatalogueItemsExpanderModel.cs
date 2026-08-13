using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageCapability;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageCapabilities;

public class CapabilityCatalogueItemsExpanderModel
{
    public IEnumerable<AdminManageCapabilityCatalogueItem> CatalogueItems { get; set; }
}
