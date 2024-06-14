using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin
{
    public enum InteroperabilityGpConnectIntegrationType
    {
        [Display(Name = "GP Connect Access Record HTML")]
        Access_Record_HTML,
        [Display(Name = "GP Connect Appointment Management")]
        Appointment_Management,
        [Display(Name = "GP Connect Access Record Structured")]
        Access_Record_Structured,
        [Display(Name = "GP Connect Send Document")]
        Send_Document,
        [Display(Name = "GP Connect Update Record")]
        Update_Record,
    }
}
