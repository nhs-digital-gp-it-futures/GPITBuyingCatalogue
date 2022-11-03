using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.ServiceRecipients
{
    public class SelectRecipientsModel : NavBaseModel
    {
        public const string SelectAll = "Select all";
        public const string SelectNone = "Select none";

        private readonly SelectionMode? selectionMode;

        public SelectRecipientsModel()
        {
        }

        public SelectRecipientsModel(
            OrderItem orderItem,
            List<ServiceRecipientModel> serviceRecipients,
            SelectionMode? selectionMode,
            string[] importedRecipients = null)
        {
            this.selectionMode = selectionMode;
            ServiceRecipients = serviceRecipients;

            ItemName = orderItem.CatalogueItem.Name;
            ItemType = orderItem.CatalogueItem.CatalogueItemType;

            switch (selectionMode)
            {
                case SelectionMode.All:
                    ServiceRecipients.ForEach(x => x.Selected = true);
                    SelectionMode = SelectionMode.None;
                    SelectionCaption = SelectNone;
                    break;

                case SelectionMode.None:
                    ServiceRecipients.ForEach(x => x.Selected = false);
                    SelectionMode = SelectionMode.All;
                    SelectionCaption = SelectAll;
                    break;

                case null:
                    if (importedRecipients?.Length > 0)
                    {
                        HasImportedRecipients = true;
                        ServiceRecipients.Where(
                                sr => importedRecipients.Select(x => x.ToUpperInvariant()).Contains(sr.OdsCode))
                            .ToList()
                            .ForEach(x => x.Selected = true);

                        SelectionMode = SelectionMode.None;
                        SelectionCaption = SelectNone;
                    }
                    else
                    {
                        SetSelectionsFromOrderItem(orderItem);
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(selectionMode), selectionMode, null);
            }
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public bool AssociatedServicesOnly { get; set; }

        public string ItemName { get; set; }

        public CatalogueItemType ItemType { get; set; }

        public List<ServiceRecipientModel> ServiceRecipients { get; set; }

        public string SelectionCaption { get; set; }

        public SelectionMode SelectionMode { get; set; }

        public bool PreSelected { get; set; }

        public RoutingSource? Source { get; set; }

        public bool IsAdding { get; set; } = true;

        public bool HasImportedRecipients { get; set; }

        public List<ServiceRecipientDto> GetSelectedItems()
        {
            return ServiceRecipients
                .Where(x => x.Selected)
                .Select(x => x.Dto)
                .ToList();
        }

        public void PreSelectRecipients(OrderItem orderItem)
        {
            if (selectionMode != null
                || orderItem?.OrderItemRecipients == null
                || orderItem.CatalogueItem.Name == ItemName)
            {
                return;
            }

            SetSelectionsFromOrderItem(orderItem);
            PreSelected = true;
        }

        private void SetSelectionsFromOrderItem(OrderItem orderItem)
        {
            ServiceRecipients.ForEach(x => x.Selected = orderItem.OrderItemRecipients?.Any(r => r.OdsCode == x.OdsCode) ?? false);

            SelectionMode = ServiceRecipients.All(x => x.Selected)
                ? SelectionMode.None
                : SelectionMode.All;

            SelectionCaption = ServiceRecipients.All(x => x.Selected)
                ? SelectNone
                : SelectAll;
        }
    }
}
