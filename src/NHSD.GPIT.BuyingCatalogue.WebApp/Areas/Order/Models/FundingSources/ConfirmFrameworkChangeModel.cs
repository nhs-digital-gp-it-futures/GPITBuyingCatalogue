using Microsoft.AspNetCore.Mvc.Rendering;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSources
{
    public sealed class ConfirmFrameworkChangeModel : OrderingBaseModel
    {
        public const string TitleText = "Are you sure you want to change your procurement framework?";

        public ConfirmFrameworkChangeModel()
        {
        }

        public ConfirmFrameworkChangeModel(
            EntityFramework.Ordering.Models.Order order,
            EntityFramework.Catalogue.Models.Framework selectedFramework)
        {
            Title = TitleText;
            Caption = $"Order {order.CallOffId}";

            CurrentFramework = order.SelectedFramework;
            SelectedFramework = selectedFramework;
        }

        public string Caption { get; set; }

        public EntityFramework.Catalogue.Models.Framework CurrentFramework { get; set; }

        public EntityFramework.Catalogue.Models.Framework SelectedFramework { get; set; }

        public bool? ConfirmChanges { get; set; }

        public SelectList Options => new(
        new[]
        {
            new SelectListItem($"Yes, I want to confirm changes to the procurement framework", $"{true}"),
            new SelectListItem($"No, I do not want to confirm changes to the procurement framework ", $"{false}"),
        },
        "Value",
        "Text");
    }
}
