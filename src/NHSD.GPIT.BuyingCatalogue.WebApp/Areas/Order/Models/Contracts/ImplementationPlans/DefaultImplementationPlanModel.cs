using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.ImplementationPlans
{
    public class DefaultImplementationPlanModel : NavBaseModel
    {
        public const string NoOptionText = "No, I've agreed bespoke milestones with the supplier";

        public CallOffId CallOffId { get; set; }

        public Solution Solution { get; set; }

        public ImplementationPlan Plan { get; set; }

        public bool? UseDefaultMilestones { get; set; }

        public IList<SelectOption<bool>> Options => new List<SelectOption<bool>>
        {
            new("Yes", true),
            new(NoOptionText, false),
        };

        public string SupplierImplementationPlan => Solution?.ImplementationDetail ?? "The supplier has not provided a standard implementation plan. You should contact them to discuss this.";
    }
}
