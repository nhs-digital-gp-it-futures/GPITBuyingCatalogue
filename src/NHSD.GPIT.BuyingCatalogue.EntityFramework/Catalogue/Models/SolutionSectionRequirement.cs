using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public enum SolutionSectionRequirement
{
    [Display(Name = "Not Applicable")]
    NotApplicable,

    [Display(Name = "Optional")]
    Optional,

    [Display(Name = "Mandatory")]
    Mandatory,
}
