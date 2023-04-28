using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients
{
    public class SelectRecipientsModel : NavBaseModel
    {
        public const string AdviceText = "Select the organisations from the sublocations that you want to receive this {0} or upload them using a CSV file.";
        public const string AdviceTextImport = "Review the organisations that will receive this {0}.";
        public const string AdviceTextNoRecipientsAvailable = "All your Service Recipients were included in the original order, so there are no more available to add.";
        public const string PreSelectedAssociatedServicesOnly = "first Associated Service";
        public const string PreSelectedCatalogueSolution = "Catalogue Solution";
        public const string SelectAll = "Select all";
        public const string SelectNone = "Deselect all";
        public const string Separator = ",";
        public const string TitleText = "Service Recipients for {0}";

        private readonly SelectionMode? selectionMode;

        public SelectRecipientsModel()
        {
        }

        public SelectRecipientsModel(
            Organisation organisation,
            OrderItem orderItem,
            OrderItem previousItem,
            List<ServiceRecipientModel> serviceRecipients,
            SelectionMode? selectionMode,
            string[] importedRecipients = null)
        {
            this.selectionMode = selectionMode;

            OrganisationName = organisation.Name;
            OrganisationType = organisation.OrganisationType;

            ItemName = previousItem?.CatalogueItem.Name ?? orderItem.CatalogueItem.Name;
            ItemType = previousItem?.CatalogueItem.CatalogueItemType ?? orderItem.CatalogueItem.CatalogueItemType;

            PreviouslySelected = previousItem?.OrderItemRecipients?.Select(x => x.Recipient?.OdsCode).ToList()
                ?? Enumerable.Empty<string>().ToList();

            SubLocations = serviceRecipients
                .GroupBy(x => x.Location)
                .Select(
                    x => new SublocationModel(
                        x.Key,
                        x.Where(x => !PreviouslySelected.Contains(x.OdsCode)).OrderBy(x => x.Name).ToList()))
                .OrderBy(x => x.Name)
                .ToList();

            SetAdvice(importedRecipients);
            ApplySelection(importedRecipients, orderItem);
        }

        public override string Title => string.Format(TitleText, ItemType.Name());

        public override string Caption => ItemName;

        public string InternalOrgId { get; set; }

        public string OrganisationName { get; set; }

        public OrganisationType? OrganisationType { get; init; }

        public CallOffId CallOffId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public bool AssociatedServicesOnly { get; set; }

        public string ItemName { get; set; }

        public CatalogueItemType ItemType { get; set; }

        public List<SublocationModel> SubLocations { get; set; }

        public string SelectionCaption { get; set; }

        public SelectionMode SelectionMode { get; set; }

        public bool PreSelected { get; set; }

        public RoutingSource? Source { get; set; }

        public bool IsAdding { get; set; } = true;

        public bool HasImportedRecipients { get; set; }

        public bool HasMissingImportedRecipients { get; set; }

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

        public List<ServiceRecipientModel> GetServiceRecipients()
        {
            return SubLocations.SelectMany(x => x.ServiceRecipients).ToList();
        }

        public List<ServiceRecipientDto> GetSelectedItems()
        {
            return GetServiceRecipients()
                .Where(x => x.Selected)
                .Select(x => x.Dto)
                .ToList();
        }

        public void SelectRecipientIds(string recipientIds)
        {
            var odsCodes = recipientIds?
                .Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                .ToList() ?? Enumerable.Empty<string>();

            odsCodes.ForEach(odsCode =>
            {
                var recipient = GetServiceRecipients().FirstOrDefault(x => x.OdsCode == odsCode);

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

            var serviceRecipients = GetServiceRecipients();

            order.GetSolution().OrderItemRecipients
                .Select(x => x.OdsCode)
                .ForEach(odsCode =>
                {
                    var recipient = serviceRecipients.FirstOrDefault(x => x.OdsCode == odsCode);

                    if (recipient != null)
                    {
                        recipient.Selected = true;
                    }
                });

            if (serviceRecipients.Any(x => x.Selected))
            {
                PreSelected = true;
            }
        }

        private void ApplySelection(string[] importedRecipients, OrderItem orderItem)
        {
            switch (selectionMode)
            {
                case SelectionMode.All:
                    GetServiceRecipients().ForEach(x => x.Selected = true);
                    SelectionMode = SelectionMode.None;
                    SelectionCaption = SelectNone;
                    break;

                case SelectionMode.None:
                    GetServiceRecipients().ForEach(x => x.Selected = false);
                    SelectionMode = SelectionMode.All;
                    SelectionCaption = SelectAll;
                    break;

                default:
                    if (importedRecipients?.Length > 0)
                    {
                        var odsCodes = importedRecipients.Select(x => x.ToUpperInvariant());

                        var serviceRecipients = GetServiceRecipients();

                        serviceRecipients
                            .Where(x => odsCodes.Contains(x.OdsCode))
                            .ForEach(x => x.Selected = true);

                        HasImportedRecipients = true;

                        var allSelected = serviceRecipients.All(x => x.Selected);

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
            if (OrganisationType == EntityFramework.Organisations.Models.OrganisationType.GP)
            {
                Advice = string.Empty;
            }
            else if (!GetServiceRecipients().Any())
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
            var serviceRecipients = GetServiceRecipients();

            serviceRecipients.ForEach(x => x.Selected = orderItem?.OrderItemRecipients?.Any(r => r.OdsCode == x.OdsCode) ?? false);

            SelectionMode = serviceRecipients.All(x => x.Selected)
                ? SelectionMode.None
                : SelectionMode.All;

            SelectionCaption = serviceRecipients.All(x => x.Selected)
                ? SelectNone
                : SelectAll;
        }
    }
}
