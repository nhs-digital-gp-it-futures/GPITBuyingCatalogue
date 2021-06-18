using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class CatalogueSolutionsModel : NavBaseModel
    {
        public List<CatalogueItem> CatalogueItems { get; set; }

        public string GetStatusClass(int statusId)
        {
            string colour;

            switch (statusId)
            {
                case 2: colour = "yellow"; break;
                case 3: colour = "green"; break;
                case 4: colour = "red"; break;
                default: colour = "white"; break;
            }

            return $"nhsuk-tag--{colour}";
        }
    }
}
