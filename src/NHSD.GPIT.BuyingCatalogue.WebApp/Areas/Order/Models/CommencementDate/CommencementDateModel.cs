using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CommencementDate
{
    public sealed class CommencementDateModel : OrderingBaseModel
    {
        public CommencementDateModel()
        {
        }

        public CommencementDateModel(string odsCode, CallOffId callOffId, DateTime? commencementDate)
        {
            Title = $"Commencement date for {callOffId}";
            OdsCode = odsCode;

            if (commencementDate.HasValue)
            {
                Day = commencementDate.Value.Day.ToString("00");
                Month = commencementDate.Value.Month.ToString("00");
                Year = commencementDate.Value.Year.ToString("0000");
            }
        }

        [StringLength(2)]
        public string Day { get; set; }

        [StringLength(2)]
        public string Month { get; set; }

        [StringLength(4)]
        public string Year { get; set; }

        public (DateTime? Date, string Error) ToDateTime()
        {
            try
            {
                var date = DateTime.ParseExact($"{Day}/{Month}/{Year}", "d/M/yyyy", CultureInfo.InvariantCulture);

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
