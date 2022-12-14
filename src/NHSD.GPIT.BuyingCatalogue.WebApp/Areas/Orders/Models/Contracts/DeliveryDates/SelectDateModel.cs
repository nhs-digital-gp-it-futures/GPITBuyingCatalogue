using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates
{
    public class SelectDateModel : DateInputModel
    {
        public SelectDateModel()
        {
        }

        public SelectDateModel(string internalOrgId, CallOffId callOffId, DateTime commencementDate, DateTime? date)
        {
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            CommencementDate = commencementDate;

            SetDateFields(date);
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public DateTime CommencementDate { get; set; }
    }
}
