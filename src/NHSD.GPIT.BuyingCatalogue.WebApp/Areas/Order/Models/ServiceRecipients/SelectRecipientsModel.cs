using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.ServiceRecipients
{
    public class SelectRecipientsModel : NavBaseModel
    {
        public const string SelectAll = "Select all";
        public const string SelectNone = "Select none";

        private readonly SelectionMode? selectionMode;

        public SelectRecipientsModel()
        {
        }

        public SelectRecipientsModel(List<ServiceRecipientModel> serviceRecipients, SelectionMode? selectionMode)
        {
            this.selectionMode = selectionMode;
            ServiceRecipients = serviceRecipients;

            switch (selectionMode)
            {
                case SelectionMode.All:
                    serviceRecipients.ForEach(x => x.Selected = true);
                    SelectionMode = SelectionMode.None;
                    SelectionCaption = SelectNone;
                    break;

                case SelectionMode.None:
                case null:
                    serviceRecipients.ForEach(x => x.Selected = false);
                    SelectionMode = SelectionMode.All;
                    SelectionCaption = SelectAll;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(selectionMode), selectionMode, null);
            }
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public CatalogueItemId? CatalogueItemId { get; set; }

        public string ItemName { get; set; }

        public CatalogueItemType ItemType { get; set; }

        public List<ServiceRecipientModel> ServiceRecipients { get; set; }

        public string SelectionCaption { get; set; }

        public SelectionMode SelectionMode { get; set; }

        public bool PreSelected { get; set; }

        public void PreSelectRecipients(OrderItem solution)
        {
            if (selectionMode != null
                || solution?.OrderItemRecipients == null)
            {
                return;
            }

            var odsCodes = solution.OrderItemRecipients.Select(x => x.OdsCode);

            ServiceRecipients
                .Where(x => odsCodes.Contains(x.OdsCode))
                .ForEach(x => x.Selected = true);

            PreSelected = true;
        }
    }
}
