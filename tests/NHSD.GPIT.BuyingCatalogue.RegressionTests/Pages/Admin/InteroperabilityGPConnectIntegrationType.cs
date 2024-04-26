using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin
{
    public enum InteroperabilityGPConnectIntegrationType
    {
        [EnumMember(Value = "GP Connect - HTML View")]
        HTML_View,
        [EnumMember(Value = "GP Connect - Appointment Booking")]
        Appointment_Booking,
        [EnumMember(Value = "GP Connect - Structured Record")]
        Structured_Record,
    }
}
