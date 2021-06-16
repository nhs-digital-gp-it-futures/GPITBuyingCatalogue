using System;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class CommencementDateModel : OrderingBaseModel
    {
        public CommencementDateModel()
        {
        }

        public CommencementDateModel(string odsCode, string callOffId, DateTime? commencementDate)
        {
            BackLinkText = "Go back";
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}";
            Title = $"Commencement date for {callOffId}";
            OdsCode = odsCode;

            if (commencementDate.HasValue)
            {
                Day = commencementDate.Value.Day.ToString("00");
                Month = commencementDate.Value.Month.ToString("00");
                Year = commencementDate.Value.Year.ToString("0000");
            }
        }

        public string Day { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public (DateTime? Date, string Error) ToDateTime()
        {
            try
            {
                var date = DateTime.Parse($"{Day}/{Month}/{Year}");

                if (date.ToUniversalTime() <= DateTime.UtcNow.AddDays(-60))
                    return (null, "Commencement date must be in the future or within the last 60 days");

                return (date, null);
            }
            catch (FormatException)
            {
                return (null, "Commencement date must be a real date");
            }
        }
    }
}
