using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public enum SlaType
    {
        [Description("Type1")]
        [Display(Name = "Type 1 catalogue solution")]
        Type1 = 1,

        [Description("Type2")]
        [Display(Name = "Type 2 catalogue solution")]
        Type2 = 2,
    }
}
