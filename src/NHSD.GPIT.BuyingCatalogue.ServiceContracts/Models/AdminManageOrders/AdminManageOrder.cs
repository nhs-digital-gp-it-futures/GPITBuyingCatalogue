using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageOrders
{
    public class AdminManageOrder
    {
        public CallOffId CallOffId { get; set; }

        public string OrganisationName { get; set; }

        public DateTime Created { get; set; }

        public OrderStatus Status { get; set; }
    }
}
