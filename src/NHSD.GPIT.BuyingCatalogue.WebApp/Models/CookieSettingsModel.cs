using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public class CookieSettingsModel : NavBaseModel
    {
        public bool? UseAnalytics { get; init; }

        public IList<SelectListItem> CookieOptions => new List<SelectListItem>
        {
            new("Use cookies to measure my website use", true.ToString()),
            new("Do not use cookies to measure my website use", false.ToString()),
        };
    }
}
