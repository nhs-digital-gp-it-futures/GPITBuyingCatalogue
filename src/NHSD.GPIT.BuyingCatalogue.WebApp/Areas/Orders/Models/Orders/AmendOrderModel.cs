using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders
{
    public class AmendOrderModel : NavBaseModel
    {
        public AmendOrderModel()
        {
        }

        public AmendOrderModel(string internalOrgId, CallOffId callOffId)
        {
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public List<string> DoItems => new()
        {
            "terminate this contract",
            "update your order description and change your primary contact details and those for the supplier",
            "add new service recipients to your order",
            "add new associated services to any new service recipients that have been added to the order",
            "add new additional services",
            "assign funding sources to cover the increased cost of the order",
        };

        public List<string> DontItems => new()
        {
            "add a new catalogue solution",
            "change the supplier",
            "change the timescales for this call-off agreement",
            "change the price of any items previously included in the order",
            "change the quantity of items previously included in the order",
            "remove service recipients from the order",
            "make any changes to an associated service already ordered - to do this you'll need to create a new order",
        };
    }
}
