using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.RecipientsModels;

public class SelectRecipientsModel : NavBaseModel
{
    public const string SelectAll = "Select all";
    public const string SelectNone = "Deselect all";
    private readonly SelectionMode? selectionMode;

    public SelectRecipientsModel()
    {
    }

    public SelectRecipientsModel(
        Organisation organisation,
        IEnumerable<ServiceRecipientModel> serviceRecipients,
        IEnumerable<string> existingRecipients,
        IEnumerable<string> preSelectedRecipients,
        SelectionMode? selectionMode = null)
    {
        this.selectionMode = selectionMode;

        OrganisationName = organisation.Name;
        OrganisationType = organisation.OrganisationType.GetValueOrDefault();
        SubLocations = serviceRecipients
            .GroupBy(x => x.Location)
            .Select(
                x => new SublocationModel(
                    x.Key,
                    x.OrderBy(y => y.Name).ToList()))
            .OrderBy(x => x.Name)
            .ToArray();

        SelectServiceRecipients(existingRecipients, preSelectedRecipients);
    }

    public string ImportRecipientsLink { get; set; }

    public string OrganisationName { get; set; }

    public OrganisationType OrganisationType { get; set; }

    public SublocationModel[] SubLocations { get; set; } = Array.Empty<SublocationModel>();

    public bool HasImportedRecipients { get; set; }

    public bool ShouldExpand { get; set; }

    public string SelectionCaption { get; set; } = SelectAll;

    public SelectionMode? RecipientSelectionMode { get; set; } = SelectionMode.All;

    public IEnumerable<ServiceRecipientModel> GetServiceRecipients()
    {
        return SubLocations.SelectMany(x => x.ServiceRecipients);
    }

    public IEnumerable<ServiceRecipientModel> GetSelectedServiceRecipients()
    {
        return GetServiceRecipients().Where(x => x.Selected);
    }

    public bool HasSelectedRecipients()
    {
        return GetSelectedServiceRecipients().Any();
    }

    private void SelectServiceRecipients(
        IEnumerable<string> existingRecipients,
        IEnumerable<string> recipients)
    {
        switch (selectionMode)
        {
            case SelectionMode.All:
                GetServiceRecipients().ToList().ForEach(x => x.Selected = true);
                RecipientSelectionMode = SelectionMode.None;
                SelectionCaption = SelectNone;
                break;
            case SelectionMode.None:
                GetServiceRecipients().ToList().ForEach(x => x.Selected = false);
                RecipientSelectionMode = SelectionMode.All;
                SelectionCaption = SelectAll;
                break;
            default:
                if (recipients == null) return;

                var enumeratedRecipients = recipients.ToArray();
                var recipientsToSelect = enumeratedRecipients.Any() ? enumeratedRecipients.ToArray() : existingRecipients.ToArray();

                var matchingRecipients = GetServiceRecipients().Where(x => recipientsToSelect.Contains(x.OdsCode)).ToList();
                if (!matchingRecipients.Any())
                    return;

                matchingRecipients.ForEach(x => x.Selected = true);

                var allSelected = GetServiceRecipients().All(x => x.Selected);

                RecipientSelectionMode = allSelected ? SelectionMode.None : SelectionMode.All;
                SelectionCaption = allSelected ? SelectNone : SelectAll;
                break;
        }
    }
}
