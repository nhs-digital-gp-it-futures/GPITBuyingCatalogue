using System.ComponentModel;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

public enum SolutionStandardStatus
{
    [Description("Not applicable")]
    NotApplicable = 1,
    [Description("Not yet selected")]
    NotYetSelected = 2,
    [Description("Not met")]
    NotMet = 3,
    [Description("In progress")]
    InProgress = 4,
    [Description("Fully met")]
    FullyMet = 5,
}
