using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters
{
    public class SaveFilterModel : NavBaseModel
    {
        public SaveFilterModel()
        {
        }

        public SaveFilterModel(
            List<Capability> capabilities,
            List<Epic> epics,
            EntityFramework.Catalogue.Models.Framework framework,
            List<ClientApplicationType> clientApplicationTypes,
            List<HostingType> hostingTypes)
        {
            CapabilityIds = capabilities.Select(x => x.Id).ToList();
            EpicIds = epics.Select(x => x.Id).ToList();
            FrameworkId = framework.Id;
            FrameworkName = framework.ShortName; 
            ClientApplicationTypes = clientApplicationTypes;
            HostingTypes = hostingTypes;
            
            GroupedCapabilities = capabilities.ToDictionary(
                x => x.Name,
                x => epics.Where(c => c.Capability.Id == x.Id).OrderBy(c => c.Name));
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<int> CapabilityIds { get; init; }

        public List<string> EpicIds { get; init; }

        public string FrameworkId{ get; set; }

        public string FrameworkName { get; init; }

        public List<ClientApplicationType> ClientApplicationTypes { get; init; }

        public List<HostingType> HostingTypes { get; init; }

        public Dictionary<string, IOrderedEnumerable<Epic>> GroupedCapabilities { get; init; } = new ();

    }
}
