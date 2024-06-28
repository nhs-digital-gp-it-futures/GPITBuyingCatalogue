using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.ListPrices
{
    public enum ProvisioningType
    {
        [EnumMember(Value = "Per patient per year")]
        Per_patient_per_year,
        [EnumMember(Value = "Declarative")]
        Declarative,
        [EnumMember(Value = "On demand")]
        On_demand,
    }
}
