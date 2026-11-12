using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public enum SolutionType
{
    [Display(Name = "GP")]
    GPIT,

    [Display(Name = "community pharmacy")]
    CommunityPharmacy,
}
