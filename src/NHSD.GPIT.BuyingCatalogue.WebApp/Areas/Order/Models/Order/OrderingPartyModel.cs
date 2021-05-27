using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class OrderingPartyModel : OrderingBaseModel
    {
        public OrderingPartyModel()
        {
        }

        public OrderingPartyModel(string odsCode, EntityFramework.Models.Ordering.Order order, Organisation organisation)
        {
            BackLinkText = "Go back";
            BackLink = $"/order/organisation/{odsCode}/order/{order.CallOffId}";
            Title = $"Call-off Ordering Party information for {order.CallOffId}";
            OrganisationName = organisation.Name;
            Address = order.OrderingParty.Address;
            Contact = new PrimaryContactModel
            {
                FirstName = order.OrderingPartyContact?.FirstName,
                LastName = order.OrderingPartyContact?.LastName,
                EmailAddress = order.OrderingPartyContact?.Email,
                TelephoneNumber = order.OrderingPartyContact?.Phone,
            };
        }

        public string OrganisationName { get; set; }

        public Address Address { get; set; }

        public PrimaryContactModel Contact { get; set; }
    }
}
