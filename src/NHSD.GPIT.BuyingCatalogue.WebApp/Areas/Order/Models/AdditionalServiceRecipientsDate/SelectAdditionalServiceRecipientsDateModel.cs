using System;
using System.Globalization;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Constants;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServiceRecipientsDate
{
    public sealed class SelectAdditionalServiceRecipientsDateModel : OrderingBaseModel
    {
        public SelectAdditionalServiceRecipientsDateModel()
        {
        }

        public SelectAdditionalServiceRecipientsDateModel(
            string odsCode,
            CreateOrderItemModel state,
            DateTime? defaultDeliveryDate)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{state.CallOffId}/additional-services/select/additional-service/price/recipients";
            BackLinkText = "Go back";
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

        public (DateTime? Date, string Error) ValidateAndGetDeliveryDate()
        {
            try
            {
                var date = DateTime.ParseExact($"{Day}/{Month}/{Year}", "d/M/yyyy", CultureInfo.InvariantCulture);

                if (date.ToUniversalTime() <= DateTime.UtcNow)
                    return (null, "Planned delivery date must be in the future");

                if (CommencementDate.HasValue && date.ToUniversalTime() > CommencementDate.Value.AddMonths(ValidationConstants.MaxDeliveryMonthsFromCommencement))
                    return (null, $"Planned delivery date must be within {ValidationConstants.MaxDeliveryMonthsFromCommencement} months from the commencement date for this Call-off Agreement");

                return (date, null);
            }
            catch (FormatException)
            {
                return (null, "Planned delivery date must be a real date");
            }
        }
    }
}
