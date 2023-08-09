using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Quantity
{
    public class ViewServiceRecipientQuantityModel : NavBaseModel
    {
        public const string AdviceText = "These are the quantities you agreed in the original contract. You are unable to change the originally agreed quantities.";
        public const string QuantityColumnText = "Quantity";
        public const string QuantityColumnPatientText = "Practice list size";
        public const string TitleText = "Quantity of {0}";

        private readonly ProvisioningType? provisioningType;

        public ViewServiceRecipientQuantityModel(OrderItem orderItem)
        {
            if (orderItem == null)
            {
                throw new ArgumentNullException(nameof(orderItem));
            }

            ItemName = orderItem.CatalogueItem.Name;
            ItemType = orderItem.CatalogueItem.CatalogueItemType.Name();
            provisioningType = orderItem.OrderItemPrice?.ProvisioningType;

            ServiceRecipients = orderItem.OrderItemRecipients
                .Select(x => new ServiceRecipientQuantityModel
                {
                    OdsCode = x.OdsCode,
                    Name = x.Recipient?.Name,
                    Quantity = x.Quantity ?? 0,
                })
                .ToArray();
        }

        public override string Title => string.Format(TitleText, ItemType);

        public override string Caption => ItemName;

        public override string Advice => AdviceText;

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public string ItemName { get; set; }

        public string ItemType { get; set; }

        public ServiceRecipientQuantityModel[] ServiceRecipients { get; set; }

        public string QuantityColumnTitle => provisioningType switch
        {
            ProvisioningType.Patient => QuantityColumnPatientText,
            _ => QuantityColumnText,
        };
    }
}
