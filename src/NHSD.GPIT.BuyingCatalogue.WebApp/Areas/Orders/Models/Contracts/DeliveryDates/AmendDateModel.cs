using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates
{
    public class AmendDateModel : DateInputModel
    {
        public const string AdviceText = "Enter the date you expect this item to start being used by the Service Recipients.";
        public const string TitleText = "Planned delivery date";

        public AmendDateModel()
        {
        }

        public AmendDateModel(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            Order order,
            DateTime? date)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            CatalogueItemId = catalogueItemId;

            var orderItem = order.OrderItem(catalogueItemId);

            ItemName = orderItem?.CatalogueItem?.Name;

            CommencementDate = order.CommencementDate;
            MaximumTerm = order.MaximumTerm;
            TriageValue = order.OrderTriageValue;

            SetDateFields(date);
        }

        public override string Title => TitleText;

        public override string Caption => ItemName;

        public override string Advice => AdviceText;

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string ItemName { get; set; }

        public DateTime? CommencementDate { get; set; }

        public int? MaximumTerm { get; set; }

        public OrderTriageValue? TriageValue { get; set; }

        public RoutingSource? Source { get; set; }

        public DateTime? ContractEndDate
        {
            get
            {
                var endDate = new EndDate(CommencementDate, MaximumTerm);
                return endDate.DateTime;
            }
        }
    }
}
