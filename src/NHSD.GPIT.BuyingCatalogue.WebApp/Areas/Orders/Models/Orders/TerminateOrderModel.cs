using System;
using System.Collections.Generic;
using System.Globalization;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders
{
    public class TerminateOrderModel : NavBaseModel
    {
        public TerminateOrderModel()
        {
        }

        public TerminateOrderModel(string internalOrgId, CallOffId callOffId)
        {
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public string Reason { get; set; }

        public string Day { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public DateTime? TerminationDate =>
            DateTime.TryParseExact($"{Day}/{Month}/{Year}", "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime parsedDate)
                ? parsedDate : null;

        public bool Confirm { get; set; }
    }
}
