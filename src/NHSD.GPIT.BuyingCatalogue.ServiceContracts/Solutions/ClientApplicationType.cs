using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public enum ClientApplicationType
    {
        [Display(Name = "Browser-based application")]
        [EnumMember(Value = "browser-based")]
        BrowserBased,

        [Display(Name = "Mobile application")]
        [EnumMember(Value = "native-mobile")]
        MobileTablet,

        [Display(Name = "Desktop application")]
        [EnumMember(Value = "native-desktop")]
        Desktop,
    }
}
