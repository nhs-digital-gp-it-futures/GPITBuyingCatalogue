using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

public enum AccountDeactivationReason
{

    [Display(Name = "Manual")]
    Manual = 0,

    [Display(Name = "Inactivity")]
    Inactivity,
}
