using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderingParty
{
    public sealed class OrderingPartyModel : OrderingBaseModel
    {
        public OrderingPartyModel()
        {
        }

        public OrderingPartyModel(string odsCode, EntityFramework.Ordering.Models.Order order)
        {
            InternalOrgId = odsCode;
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
