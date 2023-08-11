using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Shared
{
    public class MilestoneTableModel
    {
        public MilestoneTableModel()
        {
        }

        public MilestoneTableModel(string title, IEnumerable<ImplementationPlanMilestone> milestones, bool isAction, CallOffId callOffId, string internalOrgId)
        {
            Title = title;
            Milestones = milestones ?? Enumerable.Empty<ImplementationPlanMilestone>();
            IsAction = isAction;
            CallOffId = callOffId;
            InternalOrgId = internalOrgId;
        }

        public IEnumerable<ImplementationPlanMilestone> Milestones { get; set; }

        public string Title { get; set; }

        public bool IsAction { get; set; }

        public CallOffId CallOffId { get; set; }

        public string InternalOrgId { get; set; }
    }
}
