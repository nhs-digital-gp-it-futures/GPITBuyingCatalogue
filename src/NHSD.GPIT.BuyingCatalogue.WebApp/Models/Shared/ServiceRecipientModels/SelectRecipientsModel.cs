using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
public class SelectRecipientsModel : NavBaseModel
{
    private readonly SelectionMode? selectionMode;

    public SelectRecipientsModel()
    {
    }

    public SelectRecipientsModel(
        Organisation organisation,
        IEnumerable<ServiceRecipientModel> possibleServiceRecipients,
        IEnumerable<string> existingRecipients,
        IEnumerable<string> excludeRecipients,
        IEnumerable<string> preSelectedRecipients,
        SelectionMode? selectionMode = null)
    {
        this.selectionMode = selectionMode;

        OrganisationName = organisation.Name;
        OrganisationType = organisation.OrganisationType.GetValueOrDefault();

        PreviouslySelected = excludeRecipients.ToList();

        SubLocations = possibleServiceRecipients
            .GroupBy(x => x.Location)
            .Select(
                x => new SublocationModel(
                    x.Key,
                    x.Where(x => !excludeRecipients.Contains(x.OdsCode)).OrderBy(y => y.Name).ToList()))
            .OrderBy(x => x.Name)
            .ToArray();

        SelectServiceRecipients(existingRecipients, preSelectedRecipients);
    }

    public string OrganisationName { get; set; }

    public OrganisationType OrganisationType { get; set; }

    public SublocationModel[] SubLocations { get; set; } = Array.Empty<SublocationModel>();

    public ServiceRecipientModel[] SearchRecipients => SubLocations.SelectMany(x => x.ServiceRecipients.Select(y => new ServiceRecipientModel { Name = y.Name, OdsCode = y.OdsCode })).ToArray();

    public bool HasImportedRecipients { get; set; }

    public List<string> PreviouslySelected { get; set; }

    public bool ShouldExpand { get; set; }

    public int? SelectAtLeast { get; set; }

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

    private void SelectServiceRecipients(
        IEnumerable<string> existingRecipients,
        IEnumerable<string> recipients)
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

                var enumeratedRecipients = recipients.ToArray();
                var recipientsToSelect = enumeratedRecipients.Any() ? enumeratedRecipients.ToArray() : existingRecipients.ToArray();

                var matchingRecipients = GetServiceRecipients().Where(x => recipientsToSelect.Contains(x.OdsCode)).ToList();
                if (!matchingRecipients.Any())
                    return;

                matchingRecipients.ForEach(x => x.Selected = true);

                var allSelected = GetServiceRecipients().All(x => x.Selected);

                break;
        }
    }
}
