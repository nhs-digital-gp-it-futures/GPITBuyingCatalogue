using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class InteroperabilityModel : NavBaseModel
    {
        public InteroperabilityModel()
        {
        }

        public InteroperabilityModel(CatalogueItem catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            Link = catalogueItem.Solution?.AboutUrl;
            SolutionName = catalogueItem.Name;
        }

        public string SolutionName { get; set; }

        [StringLength(1000)]
        [Url]
        public string Link { get; set; }
    }
}
