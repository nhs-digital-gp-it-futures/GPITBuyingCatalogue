﻿using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CommencementDate
{
    public sealed class CommencementDateModel : DateInputModel
    {
        public CommencementDateModel()
        {
        }

        public CommencementDateModel(string internalOrgId, EntityFramework.Ordering.Models.Order order)
        {
            InternalOrgId = internalOrgId;
            CallOffId = order.CallOffId;
            IsAmendment = order.IsAmendment;
            OrderTriageValue = order.OrderTriageValue;
            InitialPeriod = $"{order.InitialPeriod}";
            MaximumTerm = $"{order.MaximumTerm}";

            SetDateFields(order.CommencementDate);
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public bool IsAmendment { get; set; }

        public string InitialPeriod { get; set; }

        public int? InitialPeriodValue => InitialPeriod.AsNullableInt();

        public string MaximumTerm { get; set; }

        public int? MaximumTermValue => MaximumTerm.AsNullableInt();

        public OrderTriageValue? OrderTriageValue { get; set; }
    }
}
