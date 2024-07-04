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
            Dictionary<string, IOrderedEnumerable<Epic>> groupedCapabilities,
            EntityFramework.Catalogue.Models.Framework framework,
            List<ApplicationType> applicationTypes,
            List<HostingType> hostingTypes,
            Dictionary<string, IOrderedEnumerable<string>> integrations,
            int organisationId)
        {
            if (framework != null)
            {
                FrameworkId = framework.Id;
                FrameworkName = framework.ShortName;
            }

            ApplicationTypes = applicationTypes;
            HostingTypes = hostingTypes;
            OrganisationId = organisationId;
            GroupedCapabilities = groupedCapabilities;
            Integrations = integrations;
        }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public string FrameworkId { get; init; }

        public string FrameworkName { get; init; }

        public int OrganisationId { get; init; }

        public List<ApplicationType> ApplicationTypes { get; init; } = [];

        public List<HostingType> HostingTypes { get; init; } = [];

        public Dictionary<string, IOrderedEnumerable<string>> Integrations { get; set; } = new();

        public Dictionary<string, IOrderedEnumerable<Epic>> GroupedCapabilities { get; set; } = new();
    }
}
