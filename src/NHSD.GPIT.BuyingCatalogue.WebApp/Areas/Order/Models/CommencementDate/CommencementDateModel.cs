using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CommencementDate
{
    public sealed class CommencementDateModel : OrderingBaseModel
    {
        public CommencementDateModel()
        {
        }

        public CommencementDateModel(string internalOrgId, CallOffId callOffId, DateTime? commencementDate, int? initialPeriod, int? maximumTerm)
        {
            Title = "Timescales for Call-off Agreement";
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;

            if (commencementDate.HasValue)
            {
                Day = commencementDate.Value.Day.ToString("00");
                Month = commencementDate.Value.Month.ToString("00");
                Year = commencementDate.Value.Year.ToString("0000");
            }

            InitialPeriod = $"{initialPeriod}";
            MaximumTerm = $"{maximumTerm}";
        }

        public CallOffId CallOffId { get; set; }

        [StringLength(2)]
        public string Day { get; set; }

        [StringLength(2)]
        public string Month { get; set; }

        [StringLength(4)]
        public string Year { get; set; }

        public string InitialPeriod { get; set; }

        public int? InitialPeriodValue => InitialPeriod.AsNullableInt();

        public string MaximumTerm { get; set; }

        public int? MaximumTermValue => MaximumTerm.AsNullableInt();

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
