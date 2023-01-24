using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients
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
            OrderItem previousItem,
            List<ServiceRecipientModel> serviceRecipients,
            SelectionMode? selectionMode,
            string[] importedRecipients = null)
        {
            this.selectionMode = selectionMode;
            ServiceRecipients = serviceRecipients;

            ItemName = previousItem?.CatalogueItem.Name ?? orderItem.CatalogueItem.Name;
            ItemType = previousItem?.CatalogueItem.CatalogueItemType ?? orderItem.CatalogueItem.CatalogueItemType;

            Title = $"Service Recipients for {ItemType.Name()}";
            Caption = ItemName;
            Advice = $"Manually select the organisations you want to receive this {ItemType.Name()} or import them using a CSV file.";

            PreviouslySelected = previousItem?.OrderItemRecipients?.Select(x => x.Recipient.Name).ToList() ?? new List<string>();
            ServiceRecipients.RemoveAll(x => PreviouslySelected.Contains(x.Name));

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
                        ServiceRecipients
                            .Where(sr => importedRecipients.Select(x => x.ToUpperInvariant()).Contains(sr.OdsCode))
                            .ToList()
                            .ForEach(x => x.Selected = true);

                        HasImportedRecipients = true;
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

        public List<string> PreviouslySelected { get; set; }

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
            ServiceRecipients.ForEach(x => x.Selected = orderItem?.OrderItemRecipients?.Any(r => r.OdsCode == x.OdsCode) ?? false);

            SelectionMode = ServiceRecipients.All(x => x.Selected)
                ? SelectionMode.None
                : SelectionMode.All;

            SelectionCaption = ServiceRecipients.All(x => x.Selected)
                ? SelectNone
                : SelectAll;
        }
    }
}
