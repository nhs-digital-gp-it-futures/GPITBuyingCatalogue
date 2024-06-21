using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

public enum InteropNhsAppIntegrationType
{
    [Display(Name = "Online Consultations")]
    Online_Consultations = 0,
    [Display(Name = "Personal Health Records")]
    Personal_Health_Records = 1,
    [Display(Name = "Primary Care Notifications")]
    Primary_Care_Notifications = 2,
    [Display(Name = "Secondary Care Notifications")]
    Secondary_Care_Notifications = 3,
}
