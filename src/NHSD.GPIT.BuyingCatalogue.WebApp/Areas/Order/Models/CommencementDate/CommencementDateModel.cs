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

        public CommencementDateModel(string internalOrgId, EntityFramework.Ordering.Models.Order order)
        {
            Title = "Timescales for Call-off Agreement";
            InternalOrgId = internalOrgId;
            CallOffId = order.CallOffId;
            OrderTriageValue = order.OrderTriageValue;

            if (order.CommencementDate.HasValue)
            {
                Day = order.CommencementDate.Value.Day.ToString("00");
                Month = order.CommencementDate.Value.Month.ToString("00");
                Year = order.CommencementDate.Value.Year.ToString("0000");
            }

            InitialPeriod = $"{order.InitialPeriod}";
            MaximumTerm = $"{order.MaximumTerm}";
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

        public OrderTriageValue? OrderTriageValue { get; set; }

        public DateTime? CommencementDate
        {
            get
            {
                if (!DateTime.TryParseExact($"{Day}/{Month}/{Year}", "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var deliveryDate))
                    return null;

                return deliveryDate.ToUniversalTime();
            }
        }
    }
}
