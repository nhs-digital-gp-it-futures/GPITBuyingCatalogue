using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public enum CatalogueItemType
    {
        [Display(Name = "Catalogue Solution")]
        Solution = 1,

        [Display(Name = "Additional Service")]
        AdditionalService = 2,

        [Display(Name = "Associated Service")]
        AssociatedService = 3,
    }
}
