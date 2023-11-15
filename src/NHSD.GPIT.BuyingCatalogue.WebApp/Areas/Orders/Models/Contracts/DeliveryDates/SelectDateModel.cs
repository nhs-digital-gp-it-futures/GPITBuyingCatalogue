using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates
{
    public class SelectDateModel : DateInputModel
    {
        public const string YesOption = "Yes";
        public const string NoOption = "No";

        public SelectDateModel()
        {
        }

        public SelectDateModel(string internalOrgId, CallOffId callOffId, Order order, bool? applyToAll)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            CommencementDate = order.CommencementDate;
            MaximumTerm = order.MaximumTerm;
            TriageValue = order.OrderTriageValue;

            IsAmend = order.IsAmendment;

            SetDateFields(order.DeliveryDate);

            ApplyToAll = applyToAll;
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public DateTime? CommencementDate { get; set; }

        public int? MaximumTerm { get; set; }

        public OrderTriageValue? TriageValue { get; set; }

        public bool IsAmend { get; set; }

        public bool? ApplyToAll { get; set; }

        public IEnumerable<SelectOption<bool>> ApplyToAllOptions => new List<SelectOption<bool>>
        {
            new(YesOption, true),
            new(NoOption, false),
        };

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
