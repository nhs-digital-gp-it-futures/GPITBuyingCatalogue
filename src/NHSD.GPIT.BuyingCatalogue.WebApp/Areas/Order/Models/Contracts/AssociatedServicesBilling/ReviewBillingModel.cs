using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.AssociatedServicesBilling
{
    public sealed class ReviewBillingModel : NavBaseModel
    {
        public const string NoOptionText = "No, I've agreed bespoke milestones with the supplier";

        public ReviewBillingModel()
        {
        }

        public ReviewBillingModel(CallOffId callOffId, ImplementationPlanMilestone targetMilestone, ContractFlags contractFlags, List<OrderItem> associatedServiceOrderItems)
        {
            CallOffId = callOffId;
            AssociatedServiceOrderItems = associatedServiceOrderItems;
            UseDefaultBilling = contractFlags.UseDefaultBilling;
            TargetMilestoneName = targetMilestone.Title;
        }

        public CallOffId CallOffId { get; set; }

        public string TargetMilestoneName { get; set; }

        public List<OrderItem> AssociatedServiceOrderItems { get; set; }

        public bool? UseDefaultBilling { get; set; }

        public IList<SelectListItem> Options => new List<SelectListItem>
        {
            new("Yes", $"{true}"),
            new(NoOptionText, $"{false}"),
        };
    }
}
