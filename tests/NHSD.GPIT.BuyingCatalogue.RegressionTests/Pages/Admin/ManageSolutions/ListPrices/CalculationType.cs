using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.ListPrices
{
    public enum CalculationType
    {
        [EnumMember(Value = "Single fixed")]
        Single_fixed,
        [EnumMember(Value = "Volume")]
        Volume,
    }
}
