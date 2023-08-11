using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Shared
{
    public class RequirementTableModel
    {
        public RequirementTableModel()
        {
        }

        public RequirementTableModel(string title, IEnumerable<Requirement> requirements, bool isAction, CallOffId callOffId, string internalOrgId)
        {
            Title = title;
            Requirements = requirements ?? Enumerable.Empty<Requirement>();
            CallOffId = callOffId;
            InternalOrgId = internalOrgId;
            IsAction = isAction;
        }

        public IEnumerable<Requirement> Requirements { get; set; }

        public string Title { get; set; }

        public bool IsAction { get; set; }

        public CallOffId CallOffId { get; set; }

        public string InternalOrgId { get; set; }
    }
}
