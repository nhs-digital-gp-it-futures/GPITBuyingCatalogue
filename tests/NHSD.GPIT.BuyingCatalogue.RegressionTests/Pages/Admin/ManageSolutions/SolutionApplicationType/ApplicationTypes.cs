using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionApplicationType
{
    public enum ApplicationTypes
    {
        [EnumMember(Value = "Browser-based application")]
        Browser_based,
        [EnumMember(Value = "Mobile application")]
        Mobile_or_tablet,
        [EnumMember(Value = "Desktop application")]
        Desktop,
    }
}
