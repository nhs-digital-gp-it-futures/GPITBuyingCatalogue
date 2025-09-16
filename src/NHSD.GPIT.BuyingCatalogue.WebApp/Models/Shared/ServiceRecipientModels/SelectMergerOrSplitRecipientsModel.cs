using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

public sealed class SelectMergerOrSplitRecipientsModel : NavBaseModel
{
    public const int SelectAtLeast = 2;

    private readonly SelectionMode? selectionMode;

    public SelectMergerOrSplitRecipientsModel()
    {
    }

    public SelectMergerOrSplitRecipientsModel(
        Organisation organisation,
        CallOffId callOffId,
        OrderType orderType,
        IEnumerable<ServiceRecipientModel> possibleServiceRecipients,
        IEnumerable<string> preSelectedRecipients,
        SelectionMode? selectionMode = null,
        bool isAmendment = false)
    {
        GetTitleAndAdviceFromOrderType(orderType);
        Caption = $"Order {callOffId}";

        this.selectionMode = selectionMode;

        OrganisationName = organisation.Name;
        OrganisationType = organisation.OrganisationType.GetValueOrDefault();

        SubLocations = possibleServiceRecipients
            .GroupBy(x => x.Location)
            .Select(x => new SublocationModel(
                x.Key,
                x.OrderBy(y => y.Name).ToList()))
            .OrderBy(x => x.Name)
            .ToArray();

        IsAmendment = isAmendment;

        SelectServiceRecipients(preSelectedRecipients);
    }

    public string OrganisationName { get; set; }

    public OrganisationType? OrganisationType { get; set; }

    public SublocationModel[] SubLocations { get; set; } = [];

    public bool? HasImportedRecipients { get; set; }

    public List<ServiceRecipientModel> PreviouslySelected { get; set; }

    public bool? ShouldExpand { get; set; }

    public bool? IsAmendment { get; set; }

    public ServiceRecipientModel[] GetSearchRecipients()
    {
        return SubLocations
            .SelectMany(x =>
                x.ServiceRecipients.Select(y => new ServiceRecipientModel { Name = y.Name, OdsCode = y.OdsCode }))
            .OrderBy(x => x.Name)
            .ToArray();
    }

    public IEnumerable<ServiceRecipientModel> GetServiceRecipients()
    {
        return SubLocations
            .Where(x => x.ServiceRecipients != null)
            .SelectMany(x => x.ServiceRecipients);
    }

    public IEnumerable<ServiceRecipientModel> GetSelectedServiceRecipients()
    {
        return GetServiceRecipients().Where(x => x.Selected);
    }

    public bool HasSelectedRecipients()
    {
        return GetSelectedServiceRecipients().Any();
    }

    private void SelectServiceRecipients(IEnumerable<string> recipients)
    {
        switch (selectionMode)
        {
            case SelectionMode.All:
                GetServiceRecipients().ToList().ForEach(x => x.Selected = true);
                break;
            case SelectionMode.None:
                GetServiceRecipients().ToList().ForEach(x => x.Selected = false);
                break;
            default:
                if (recipients == null) return;

                List<ServiceRecipientModel> matchingRecipients =
                    GetServiceRecipients().Where(x => recipients.Contains(x.OdsCode)).ToList();

                if (matchingRecipients.Count == 0)
                    return;

                matchingRecipients.ForEach(x => x.Selected = true);

                break;
        }
    }

    private void GetTitleAndAdviceFromOrderType(OrderType orderType)
    {
        switch (orderType.Value)
        {
            case OrderTypeEnum.AssociatedServiceSplit:
            {
                Title = "Service recipients splitting";
                Advice =
                    "Select all the practices that will be involved in the split you’re ordering. They must all be using the same Catalogue Solution.";
                break;
            }

            case OrderTypeEnum.AssociatedServiceMerger:
            {
                Title = "Service recipients merging";
                Advice =
                    "Select all the practices that will be involved in the merger you’re ordering. They must all be using the same Catalogue Solution.";
                break;
            }

            default: throw new ArgumentOutOfRangeException(nameof(orderType));
        }
    }
}
