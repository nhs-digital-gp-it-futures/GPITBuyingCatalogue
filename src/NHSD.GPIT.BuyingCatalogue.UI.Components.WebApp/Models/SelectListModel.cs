using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Models
{
    public class SelectListModel
    {
        public string SelectListItem { get; set; }

        public List<SelectListItem> SelectListItems { get; set; }
    }
}
