using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients
{
    public sealed class RecipientForPracticeReorganisationModel : NavBaseModel
    {
        public RecipientForPracticeReorganisationModel()
        {
        }

        public RecipientForPracticeReorganisationModel(
            Organisation organisation,
            Order order)
        {
            SetTitleAndAdviceForOrderType(order.OrderType);
            Caption = $"Order {order.CallOffId}";

            OrganisationName = organisation.Name;
            OrganisationType = organisation.OrganisationType.GetValueOrDefault();

            SubLocations = order.OrderSublocations
                .Select(
                    x => new SublocationModel(
                        x, false))
                .OrderBy(x => x.Name)
                .ToArray();
        }

        public string OrganisationName { get; set; }

        public OrganisationType OrganisationType { get; set; }

        public SublocationModel[] SubLocations { get; set; } = [];

        public string SelectedOdsCode { get; set; }

        private void SetTitleAndAdviceForOrderType(OrderType orderType)
        {
            Title = orderType.GetPracticeReorganisationRecipientTitle();
            Advice = orderType.Value switch
            {
                OrderTypeEnum.AssociatedServiceSplit =>
                    "Select the service recipient that will be losing patients as part of the split.",
                OrderTypeEnum.AssociatedServiceMerger =>
                    "Select the service recipient that will still exist after the merger.",
                _ => throw new ArgumentOutOfRangeException(nameof(orderType)),
            };
        }
    }
}
