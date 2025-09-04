using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public enum CatalogueItemType
    {
        [Display(Name = "catalogue solution")]
        Solution = 1,

        [Display(Name = "additional service")]
        AdditionalService = 2,

        [Display(Name = "associated service")]
        AssociatedService = 3,
    }
}
