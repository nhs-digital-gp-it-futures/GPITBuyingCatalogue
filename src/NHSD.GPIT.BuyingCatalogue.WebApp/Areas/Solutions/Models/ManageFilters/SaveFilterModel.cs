using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.ManageFilters
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
            List<ApplicationType> clientApplicationTypes,
            List<HostingType> hostingTypes,
            int organisationId)
        {
            CapabilityIds = capabilities.Select(x => x.Id).ToList();
            EpicIds = epics.Select(x => x.Id).ToList();

            if (framework != null)
            {
                FrameworkId = framework.Id;
                FrameworkName = framework.ShortName;
            }

            ClientApplicationTypes = clientApplicationTypes;
            HostingTypes = hostingTypes;
            OrganisationId = organisationId;

            SetGroupedCapabilities(capabilities, epics);
        }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public List<int> CapabilityIds { get; init; }

        public List<string> EpicIds { get; init; }

        public string FrameworkId { get; init; }

        public string FrameworkName { get; init; }

        public int OrganisationId { get; init; }

        public List<ApplicationType> ClientApplicationTypes { get; init; }

        public List<HostingType> HostingTypes { get; init; }

        public Dictionary<string, IOrderedEnumerable<Epic>> GroupedCapabilities { get; set; } = new();

        public void SetGroupedCapabilities(List<Capability> capabilities, List<Epic> epics)
        {
            GroupedCapabilities = capabilities.ToDictionary(
                x => x.Name,
                x => epics.Where(c => c.Capabilities.Any(y => y.Id == x.Id)).OrderBy(c => c.Name));
        }
    }
}
