using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.ListPrices
{
    public enum ListPriceTypes
    {
        [EnumMember(Value = "Flat price")]
        Flat_price,
        [EnumMember(Value = "Tiered price")]
        Tiered_price,
    }
}
