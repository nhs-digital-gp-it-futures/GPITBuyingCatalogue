using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.CommencementDate
{
    public sealed class CommencementDateModel : DateInputModel
    {
        public CommencementDateModel()
        {
        }

        public CommencementDateModel(string internalOrgId, Order order, int maximumTermUpperLimit)
        {
            InternalOrgId = internalOrgId;
            CallOffId = order.CallOffId;
            IsAmendment = order.IsAmendment;
            InitialPeriod = $"{order.InitialPeriod}";
            MaximumTerm = $"{order.MaximumTerm}";
            MaxumimTermUpperLimit = maximumTermUpperLimit;

            SetDateFields(order.CommencementDate);
        }

        public int MaxumimTermUpperLimit { get; set; }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public bool IsAmendment { get; set; }

        public string InitialPeriod { get; set; }

        public int? InitialPeriodValue => InitialPeriod.AsNullableInt();

        public string MaximumTerm { get; set; }

        public int? MaximumTermValue => MaximumTerm.AsNullableInt();
    }
}
