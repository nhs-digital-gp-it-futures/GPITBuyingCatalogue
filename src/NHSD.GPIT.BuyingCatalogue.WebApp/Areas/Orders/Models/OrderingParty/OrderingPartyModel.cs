using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderingParty
{
    public sealed class OrderingPartyModel : OrderingBaseModel
    {
        public OrderingPartyModel()
        {
        }

        public OrderingPartyModel(string internalOrgId, Order order)
        {
            InternalOrgId = internalOrgId;
            CallOffId = order.CallOffId.ToString();
            Contact = new PrimaryContactModel
            {
                FirstName = order.OrderingPartyContact?.FirstName,
                LastName = order.OrderingPartyContact?.LastName,
                EmailAddress = order.OrderingPartyContact?.Email,
                TelephoneNumber = order.OrderingPartyContact?.Phone,
            };
        }

        public PrimaryContactModel Contact { get; set; }

        public string CallOffId { get; set; }
    }
}
