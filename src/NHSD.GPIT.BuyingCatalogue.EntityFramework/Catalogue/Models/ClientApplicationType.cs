using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public enum ClientApplicationType
    {
        [Display(Name = "Browser-based")]
        [EnumMember(Value = "browser-based")]
        BrowserBased,

        [Display(Name = "Mobile or tablet")]
        [EnumMember(Value = "native-mobile")]
        MobileTablet,

        [Display(Name = "Desktop")]
        [EnumMember(Value = "native-desktop")]
        Desktop,
    }
}
