using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public sealed class SelectSolutionServiceRecipientsDateModel : OrderingBaseModel
    {
        public SelectSolutionServiceRecipientsDateModel()
        {
        }

        public SelectSolutionServiceRecipientsDateModel(
            string odsCode,
            CallOffId callOffId,
            string solutionName,
            DateTime? commencementDate,
            DateTime? plannedDeliveryDate,
            DateTime? defaultDeliveryDate)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution/price";
            BackLinkText = "Go back";
            Title = $"Planned delivery date of {solutionName} for {callOffId}";

            CommencementDate = commencementDate;

            if (plannedDeliveryDate.HasValue)
            {
                Day = plannedDeliveryDate.Value.Day.ToString("00");
                Month = plannedDeliveryDate.Value.Month.ToString("00");
                Year = plannedDeliveryDate.Value.Year.ToString("0000");
            }
            else if (defaultDeliveryDate.HasValue)
            {
                Day = defaultDeliveryDate.Value.Day.ToString("00");
                Month = defaultDeliveryDate.Value.Month.ToString("00");
                Year = defaultDeliveryDate.Value.Year.ToString("0000");
            }
            else if (commencementDate.HasValue)
            {
                Day = commencementDate.Value.Day.ToString("00");
                Month = commencementDate.Value.Month.ToString("00");
                Year = commencementDate.Value.Year.ToString("0000");
            }
        }

        public DateTime? CommencementDate { get; set; }

        public string Day { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public (DateTime? Date, string Error) ToDateTime()
        {
            try
            {
                var date = DateTime.Parse($"{Day}/{Month}/{Year}");

                if (date.ToUniversalTime() <= DateTime.UtcNow)
                    return (null, "Planned delivery date must be in the future");

                if (CommencementDate.HasValue && date.ToUniversalTime() > CommencementDate.Value.AddMonths(42))
                    return (null, "Planned delivery date must be within 42 months from the commencement date for this Call-off Agreement");

                return (date, null);
            }
            catch (FormatException)
            {
                return (null, "Planned delivery date must be a real date");
            }
        }
    }
}
