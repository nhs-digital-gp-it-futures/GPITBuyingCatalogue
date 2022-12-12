using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Models
{
    public class SelectListModel
    {
        public string SelectListItem { get; set; }

        public List<SelectOption<string>> SelectListItems { get; set; }
    }
}
