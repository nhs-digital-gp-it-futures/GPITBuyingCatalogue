using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public enum SlaType
    {
        [Description("Type1")]
        [Display(Name = "Type 1 Catalogue Solution")]
        Type1 = 1,

        [Description("Type2")]
        [Display(Name = "Type 2 Catalogue Solution")]
        Type2 = 2,
    }
}
