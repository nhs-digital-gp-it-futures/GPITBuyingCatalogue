using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

public enum InteropNhsAppIntegrationType
{
    [Display(Name = "Online Consultations")]
    Online_Consultations,
    [Display(Name = "Personal Health Records")]
    Personal_Health_Records,
    [Display(Name = "Primary Care Notifications")]
    Primary_Care_Notifications,
    [Display(Name = "Secondary Care Notifications")]
    Secondary_Care_Notifications,
}
