using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public enum SolutionType
{
    [Display(Name = "GPIT")]
    GPIT,

    [Display(Name = "Community Pharmacy")]
    CommunityPharmacy,
}
