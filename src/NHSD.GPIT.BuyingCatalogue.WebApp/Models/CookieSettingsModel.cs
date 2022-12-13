using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public class CookieSettingsModel : NavBaseModel
    {
        public bool? UseAnalytics { get; init; }

        public IList<SelectOption<string>> CookieOptions => new List<SelectOption<string>>
        {
            new("Use cookies to measure my website use", true.ToString()),
            new("Do not use cookies to measure my website use", false.ToString()),
        };
    }
}
