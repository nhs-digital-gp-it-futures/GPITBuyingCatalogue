using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

public enum InteropGpConnectIntegrations
{
    [Display(Name = "GP Connect - HTML View")]
    HTMLView,
    [Display(Name = "GP Connect - Appointment Booking")]
    AppointmentBooking,
    [Display(Name = "GP Connect - Structured Record")]
    StructuredRecord,
}
