using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSources
{
    public sealed class SelectFrameworkModel : OrderingBaseModel
    {
        public SelectFrameworkModel()
        {
        }

        public SelectFrameworkModel(
            EntityFramework.Ordering.Models.Order order,
            IList<EntityFramework.Catalogue.Models.Framework> frameworks)
        {
            SelectedFramework = order.SelectedFrameworkId;
            AssociatedServicesOnly = order.AssociatedServicesOnly;

            SetFrameworks(frameworks);

            Title = "Select a framework";
            Caption = $"Order {order.CallOffId}";
        }

        public string SelectedFramework { get; set; }

        public SelectList Frameworks { get; set; }

        public string Caption { get; set; }

        public bool AssociatedServicesOnly { get; set; }

        public void SetFrameworks(IList<EntityFramework.Catalogue.Models.Framework> frameworks) => Frameworks = new SelectList(
                frameworks.Select(f => new SelectListItem(f.ShortName, f.Id)),
                "Value",
                "Text");
    }
}
