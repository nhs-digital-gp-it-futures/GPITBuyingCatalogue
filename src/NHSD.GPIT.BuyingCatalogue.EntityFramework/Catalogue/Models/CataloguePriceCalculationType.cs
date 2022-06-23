using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public enum CataloguePriceCalculationType
    {
        [Display(Name = "Single fixed")]
        [Description("Buyers pay one fixed price no matter what quantity they want to buy.")]
        SingleFixed = 1,

        [Display(Name = "Cumulative")]
        [Description("Buyers pay one price for the quantity of units that fall into the first tier, another price for units that fall into the next tier and so on.")]
        Cumulative = 2,

        [Display(Name = "Volume")]
        [Description("Buyers pay the same price for all units based on how many they buy.")]
        Volume = 3,
    }
}
