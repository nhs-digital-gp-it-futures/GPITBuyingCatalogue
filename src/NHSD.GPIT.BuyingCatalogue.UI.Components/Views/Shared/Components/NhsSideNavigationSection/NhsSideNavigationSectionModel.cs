using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.Components.NhsSideNavigationSection
{
    public class NhsSideNavigationSectionModel
    {
        public string Name { get; set; }

        public string Action { get; set; }

        public string Controller { get; set; }

        public Dictionary<string, string> RouteData { get; set; }

        public bool Selected { get; set; } = false;

        public bool Show { get; set; } = true;
    }
}
