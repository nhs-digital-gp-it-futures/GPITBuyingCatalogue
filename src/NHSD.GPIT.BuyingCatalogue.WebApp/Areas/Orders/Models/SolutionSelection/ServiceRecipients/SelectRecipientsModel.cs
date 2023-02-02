﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients
{
    public class SelectRecipientsModel : NavBaseModel
    {
        public const string AdviceText = "Manually select the organisations you want to receive this {0} or import them using a CSV file.";
        public const string AdviceTextImport = "Review the organisations that will receive this {0}.";
        public const string AdviceTextNoRecipientsAvailable = "All your Service Recipients were included in the original order, so there are no more available to add.";
        public const string TitleText = "Service Recipients for {0}";

        public const string SelectAll = "Select all";
        public const string SelectNone = "Deselect all";
        public const string Separator = ",";
        public const string PreSelectedAssociatedServicesOnly = "first Associated Service";
        public const string PreSelectedCatalogueSolution = "Catalogue Solution";

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

            Title = string.Format(TitleText, ItemType.Name());
            Caption = ItemName;

            PreviouslySelected = previousItem?.OrderItemRecipients?.Select(x => x.Recipient?.Name).ToList() ?? new List<string>();
            ServiceRecipients.RemoveAll(x => PreviouslySelected.Contains(x.Name));

            SetAdvice(importedRecipients);
            ApplySelection(importedRecipients, orderItem);
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

        public string PreSelectedItemType => AssociatedServicesOnly
            ? PreSelectedAssociatedServicesOnly
            : PreSelectedCatalogueSolution;

        public string ActionName => IsAdding
            ? nameof(ServiceRecipientsController.AddServiceRecipients)
            : nameof(ServiceRecipientsController.EditServiceRecipients);

        public ServiceRecipientImportMode ImportMode => IsAdding
            ? ServiceRecipientImportMode.Add
            : ServiceRecipientImportMode.Edit;

        public List<ServiceRecipientDto> GetSelectedItems()
        {
            return ServiceRecipients
                .Where(x => x.Selected)
                .Select(x => x.Dto)
                .ToList();
        }

        public void SelectRecipientIds(string recipientIds)
        {
            var odsCodes = recipientIds?
                .Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                .ToList() ?? new List<string>();

            odsCodes.ForEach(odsCode =>
            {
                var recipient = ServiceRecipients.FirstOrDefault(x => x.OdsCode == odsCode);

                if (recipient != null)
                {
                    recipient.Selected = true;
                }
            });
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

        public void PreSelectSolutionServiceRecipients(Order order, CatalogueItemId catalogueItemId)
        {
            var catalogueItem = order?.OrderItem(catalogueItemId)?.CatalogueItem;

            if (catalogueItem == null
                || catalogueItem.CatalogueItemType == CatalogueItemType.Solution)
            {
                return;
            }

            order.GetSolution().OrderItemRecipients
                .Select(x => x.OdsCode)
                .ForEach(odsCode =>
                {
                    var recipient = ServiceRecipients.FirstOrDefault(x => x.OdsCode == odsCode);

                    if (recipient != null)
                    {
                        recipient.Selected = true;
                    }
                });

            if (ServiceRecipients.Any(x => x.Selected))
            {
                PreSelected = true;
            }
        }

        private void ApplySelection(string[] importedRecipients, OrderItem orderItem)
        {
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

                default:
                    if (importedRecipients?.Length > 0)
                    {
                        var odsCodes = importedRecipients.Select(x => x.ToUpperInvariant());

                        ServiceRecipients
                            .Where(x => odsCodes.Contains(x.OdsCode))
                            .ForEach(x => x.Selected = true);

                        HasImportedRecipients = true;

                        var allSelected = ServiceRecipients.All(x => x.Selected);

                        SelectionMode = allSelected ? SelectionMode.None : SelectionMode.All;
                        SelectionCaption = allSelected ? SelectNone : SelectAll;
                    }
                    else
                    {
                        SetSelectionsFromOrderItem(orderItem);
                    }

                    break;
            }
        }

        private void SetAdvice(IEnumerable importedRecipients)
        {
            if (!ServiceRecipients.Any())
            {
                Advice = AdviceTextNoRecipientsAvailable;
            }
            else
            {
                Advice = importedRecipients == null
                    ? string.Format(AdviceText, ItemType.Name())
                    : string.Format(AdviceTextImport, ItemType.Name());
            }
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
