using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public enum StandardType
    {
        [Description("Overarching")]
        [Display(Name = "Overarching")]
        Overarching = 1,

        [Description("Supplementary Care")]
        [Display(Name = "Supplementary Care")]
        SupplementaryCare = 2,

        [Description("Other")]
        [Display(Name = "Other")]
        Other = 5,
    }
}
