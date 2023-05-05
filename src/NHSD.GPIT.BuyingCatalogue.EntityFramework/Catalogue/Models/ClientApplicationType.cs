using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public enum ClientApplicationType
    {
        [Display(Name = "Browser-based")]
        [EnumMember(Value = "browser-based")]
        BrowserBased = 1,

        [Display(Name = "Mobile or tablet")]
        [EnumMember(Value = "native-mobile")]
        MobileTablet = 2,

        [Display(Name = "Desktop")]
        [EnumMember(Value = "native-desktop")]
        Desktop = 3,
    }
}
