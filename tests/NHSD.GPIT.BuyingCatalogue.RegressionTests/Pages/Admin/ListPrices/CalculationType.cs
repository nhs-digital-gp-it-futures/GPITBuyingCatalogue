using System.Runtime.Serialization;
using EnumsNET;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ListPrices
{
    public enum CalculationType
    {
        [EnumMember(Value = "Single fixed")]
        Single_fixed,
        [EnumMember(Value = "Volume")]
        Volume,
    }
}
