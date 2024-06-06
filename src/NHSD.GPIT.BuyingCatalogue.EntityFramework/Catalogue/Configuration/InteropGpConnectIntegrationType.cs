using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

public enum InteropGpConnectIntegrationType
{
    [Display(Name = "GP Connect Access Record HTML")]
    HTML_View = 0,
    [Display(Name = "GP Connect Appointment Management")]
    Appointment_Management = 1,
    [Display(Name = "GP Connect Access Record Structured")]
    Structured_Record = 2,
    [Display(Name = "GP Connect Send Document")]
    Send_Document = 3,
    [Display(Name = "GP Connect Update Record")]
    Update_Record = 4,
}
