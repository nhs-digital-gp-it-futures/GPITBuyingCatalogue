using System;
using System.Globalization;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutionRecipientsDate
{
    public sealed class SelectSolutionServiceRecipientsDateModel : OrderingBaseModel
    {
        public SelectSolutionServiceRecipientsDateModel()
        {
        }

        public SelectSolutionServiceRecipientsDateModel(
            CreateOrderItemModel state,
            DateTime? defaultDeliveryDate)
        {
            Title = $"Planned delivery date of {state.CatalogueItemName} for {state.CallOffId}";

            CommencementDate = state.CommencementDate;

            if (state.PlannedDeliveryDate.HasValue)
            {
                Day = state.PlannedDeliveryDate.Value.Day.ToString("00");
                Month = state.PlannedDeliveryDate.Value.Month.ToString("00");
                Year = state.PlannedDeliveryDate.Value.Year.ToString("0000");
            }
            else if (defaultDeliveryDate.HasValue)
            {
                Day = defaultDeliveryDate.Value.Day.ToString("00");
                Month = defaultDeliveryDate.Value.Month.ToString("00");
                Year = defaultDeliveryDate.Value.Year.ToString("0000");
            }
            else if (state.CommencementDate.HasValue)
            {
                Day = state.CommencementDate.Value.Day.ToString("00");
                Month = state.CommencementDate.Value.Month.ToString("00");
                Year = state.CommencementDate.Value.Year.ToString("0000");
            }
        }

        public DateTime? CommencementDate { get; set; }

        public string Day { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public DateTime? DeliveryDate
        {
            get
            {
                if (!DateTime.TryParseExact($"{Day}/{Month}/{Year}", "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var deliveryDate))
                    return null;

                return deliveryDate.ToUniversalTime();
            }
        }
    }
}
