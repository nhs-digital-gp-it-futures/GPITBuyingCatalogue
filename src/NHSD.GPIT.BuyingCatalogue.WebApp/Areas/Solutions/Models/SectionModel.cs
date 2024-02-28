using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SectionModel
    {
        public string Action { get; set; }

        public string Controller { get; set; }

        public Dictionary<string, string> RouteData { get; set; }

        public string Name { get; set; }

        public bool Selected { get; set; } = false;

        public bool Show { get; set; } = true;
    }
}
