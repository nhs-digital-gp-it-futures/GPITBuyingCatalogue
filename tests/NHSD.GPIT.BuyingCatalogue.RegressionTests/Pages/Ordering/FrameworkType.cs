using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering
{
    public enum FrameworkType
    {
        [EnumMember(Value = "Advanced Telephony")]
        Advanced_Telephony,
        [EnumMember(Value = "DFOCVC")]
        DFOCVC,
        [EnumMember(Value = "Tech Innovation")]
        Tech_Innovation,
    }
}
