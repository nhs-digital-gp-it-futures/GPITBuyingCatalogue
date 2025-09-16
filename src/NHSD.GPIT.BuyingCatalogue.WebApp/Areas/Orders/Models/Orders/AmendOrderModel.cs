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
            "update your order description and change your primary contact details and those for the supplier",
            "add service recipients to your catalogue solution and any Additional Services in the order",
            "add new Additional Services",
            "assign funding sources to cover the increased cost of the order",
        };

        public List<string> DontItems => new()
        {
            "add a new catalogue solution",
            "change the supplier",
            "change the timescales for this call-off agreement",
            "remove service recipients from the order",
            "change the price of any items previously included in the order",
            "change the quantity of items previously included in the order",
            "make any changes to an Associated Service - to do this you'll need to create a new order",
        };
    }
}
