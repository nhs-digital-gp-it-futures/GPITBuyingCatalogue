using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public enum CatalogueItemType
    {
        [Display(Name = "Catalogue solution")]
        Solution = 1,

        [Display(Name = "Additional service")]
        AdditionalService = 2,

        [Display(Name = "Associated Service")]
        AssociatedService = 3,
    }
}
