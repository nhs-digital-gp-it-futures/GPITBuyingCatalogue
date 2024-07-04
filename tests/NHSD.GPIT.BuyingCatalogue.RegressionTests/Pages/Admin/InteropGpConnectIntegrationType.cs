using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin;

public enum InteropGpConnectIntegrationType
{
    [EnumMember(Value = "GP Connect Access Record HTML")]
    Access_Record_HTML = 0,
    [EnumMember(Value = "GP Connect Appointment Management")]
    Appointment_Management = 1,
    [EnumMember(Value = "GP Connect Access Record Structured")]
    Access_Record_Structured = 2,
    [EnumMember(Value = "GP Connect Send Document")]
    Send_Document = 3,
    [EnumMember(Value = "GP Connect Update Record")]
    Update_Record = 4,
}
