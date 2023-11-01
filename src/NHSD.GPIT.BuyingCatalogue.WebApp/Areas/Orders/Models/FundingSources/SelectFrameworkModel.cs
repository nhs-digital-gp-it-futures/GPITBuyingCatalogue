using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.FundingSources
{
    public sealed class SelectFrameworkModel : OrderingBaseModel
    {
        public const string TitleText = "Select a framework";

        public SelectFrameworkModel()
        {
        }

        public SelectFrameworkModel(
            Order order,
            IList<EntityFramework.Catalogue.Models.Framework> frameworks)
        {
            SelectedFramework = order.SelectedFrameworkId;
            AssociatedServicesOnly = order.OrderType.AssociatedServicesOnly;

            SetFrameworks(frameworks);

            Title = TitleText;
            Caption = $"Order {order.CallOffId}";
        }

        public string SelectedFramework { get; set; }

        public IEnumerable<SelectOption<string>> Frameworks { get; set; }

        public bool AssociatedServicesOnly { get; set; }

        public void SetFrameworks(IList<EntityFramework.Catalogue.Models.Framework> frameworks) => Frameworks = frameworks
            .Select(f => new SelectOption<string>(f.ShortName, f.Id));
    }
}
