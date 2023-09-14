using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

public enum InteropGpConnectIntegrationType
{
    [Display(Name = "GP Connect - HTML View")]
    HTML_View,
    [Display(Name = "GP Connect - Appointment Booking")]
    Appointment_Booking,
    [Display(Name = "GP Connect - Structured Record")]
    Structured_Record,
}
