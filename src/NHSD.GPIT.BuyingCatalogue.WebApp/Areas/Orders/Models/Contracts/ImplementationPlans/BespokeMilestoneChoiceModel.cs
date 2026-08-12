using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans;

public class BespokeMilestoneChoiceModel : NavBaseModel
{
    public const string Yes = "Yes";
    public const string No = "No";

    public BespokeMilestoneChoiceModel()
    {
    }

    public CallOffId CallOffId { get; set; }

    public string InternalOrgId { get; set; }

    public bool? ShouldAddMilestone { get; set; }

    public IEnumerable<SelectOption<bool>> Options => new List<SelectOption<bool>>
    {
        new(Yes, true),
        new(No, false),
    };
}
