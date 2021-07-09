using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public sealed class OrderingPartyModel : OrderingBaseModel
    {
        public OrderingPartyModel()
        {
        }

        public OrderingPartyModel(string odsCode, EntityFramework.Ordering.Models.Order order, Organisation organisation)
        {
            BackLinkText = "Go back";
            BackLink = $"/order/organisation/{odsCode}/order/{order.CallOffId}";
            Title = $"Call-off Ordering Party information for {order.CallOffId}";
            OdsCode = odsCode;
            OrganisationName = organisation.Name;
            Address = organisation.Address;
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
