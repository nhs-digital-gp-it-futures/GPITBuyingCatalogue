using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

public enum InteropNhsAppIntegrationType
{
    [Display(Name = "Online Consultations")]
    OnlineConsultations,
    [Display(Name = "Personal Health Records")]
    PersonalHealthRecords,
    [Display(Name = "Primary Care Notifications")]
    PrimaryCareNotifications,
    [Display(Name = "Secondary Care Notifications")]
    SecondaryCareNotifications,
}
