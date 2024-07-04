using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin
{
    public enum InteroperabilityIm1IntegrationType
    {
        [EnumMember(Value = "IM1 Bulk")]
        Bulk,
        [EnumMember(Value = "IM1 Transactional")]
        Transactional,
        [EnumMember(Value = "IM1 Patient Facing")]
        Patient_Facing,
    }
}
