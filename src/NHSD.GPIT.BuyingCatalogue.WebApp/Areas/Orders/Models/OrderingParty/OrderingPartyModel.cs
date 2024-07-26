using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

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
            FirstName = order.OrderingPartyContact?.FirstName;
            LastName = order.OrderingPartyContact?.LastName;
            EmailAddress = order.OrderingPartyContact?.Email;
            TelephoneNumber = order.OrderingPartyContact?.Phone;
        }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(35)]
        public string TelephoneNumber { get; set; }

        [StringLength(256)]
        public string EmailAddress { get; set; }

        public string CallOffId { get; set; }
    }
}
