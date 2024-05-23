using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionHostingType
{
    public enum HostingTypes
    {
        [EnumMember(Value = "Public cloud")]
        Public_cloud,
        [EnumMember(Value = "Private cloud")]
        Private_cloud,
        [EnumMember(Value = "Hybrid")]
        Hybrid,
        [EnumMember(Value = "On premise")]
        On_premise,
    }
}
