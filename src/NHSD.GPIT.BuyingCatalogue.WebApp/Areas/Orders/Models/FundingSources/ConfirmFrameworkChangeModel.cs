using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.FundingSources
{
    public sealed class ConfirmFrameworkChangeModel : OrderingBaseModel
    {
        public const string TitleText = "Are you sure you want to change your procurement framework?";

        public ConfirmFrameworkChangeModel()
        {
        }

        public ConfirmFrameworkChangeModel(
            Order order,
            EntityFramework.Catalogue.Models.Framework selectedFramework)
        {
            Title = TitleText;
            Caption = $"Order {order.CallOffId}";

            CurrentFramework = order.SelectedFramework;
            SelectedFramework = selectedFramework;
        }

        public static IEnumerable<SelectOption<bool>> Options => new List<SelectOption<bool>>
        {
            new($"Yes, I want to confirm changes to the procurement framework", true),
            new($"No, I do not want to confirm changes to the procurement framework ", false),
        };

        public EntityFramework.Catalogue.Models.Framework CurrentFramework { get; set; }

        public EntityFramework.Catalogue.Models.Framework SelectedFramework { get; set; }

        public bool? ConfirmChanges { get; set; }
    }
}
