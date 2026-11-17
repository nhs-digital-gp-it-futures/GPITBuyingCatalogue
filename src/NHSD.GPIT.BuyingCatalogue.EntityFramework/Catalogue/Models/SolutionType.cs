using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public enum SolutionType
{
    [Display(Name = "GP")]
    [Description("Catalogue solutions")]
    GPIT,

    [Display(Name = "community pharmacy")]
    [Description("Pharmacy solutions")]
    CommunityPharmacy,
}
