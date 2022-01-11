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

        public DateTime? CommencementDate
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
