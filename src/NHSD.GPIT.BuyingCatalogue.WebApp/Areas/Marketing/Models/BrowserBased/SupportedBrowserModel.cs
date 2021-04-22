using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
{
    

    public class SupportedBrowserModel
    {
        public string BrowserName { get; set; }

        public bool Checked { get; set; }
    }
}
