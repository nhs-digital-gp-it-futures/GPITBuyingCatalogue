using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

public enum InteropIntegrationGpConnectIntegrations
{
    [Display(Name = "GP Connect - HTML View")]
    HTMLView,
    [Display(Name = "GP Connect - Appointment Booking")]
    AppointmentBooking,
    [Display(Name = "GP Connect - Structured Record")]
    StructuredRecord,
}
