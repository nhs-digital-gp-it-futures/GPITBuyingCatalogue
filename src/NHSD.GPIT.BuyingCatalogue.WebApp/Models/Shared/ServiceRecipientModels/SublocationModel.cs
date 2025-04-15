using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

public record SublocationModel
{
    public SublocationModel()
    {
    }

    public SublocationModel(string name, List<ServiceRecipientModel> serviceRecipients)
    {
        Name = name;
        ServiceRecipients = serviceRecipients;
    }

    public SublocationModel(CompetitionSublocation competitionSublocation, bool presenceDeterminesSelected)
    {
        Name = competitionSublocation.SublocationOrganisation?.Name;
        OdsCode = competitionSublocation.SublocationOdsCode;
        ServiceRecipientCount = competitionSublocation.SublocationRecipients.Count;
        ServiceRecipients = competitionSublocation.SublocationRecipients?.Select(
                x => new ServiceRecipientModel(x, presenceDeterminesSelected))
            .ToArray();
    }

    public SublocationModel(
        CompetitionSublocation competitionSublocation,
        string recipientHref,
        int serviceRecipientCount)
    {
        Name = competitionSublocation.SublocationOrganisation.Name;
        ServiceRecipientCount = serviceRecipientCount;
        OdsCode = competitionSublocation.SublocationOdsCode;
        RecipientHref = recipientHref;
    }

    public string Name { get; init; }

    public string OdsCode { get; init; }

    public IReadOnlyList<ServiceRecipientModel> ServiceRecipients { get; init; }

    public int? ServiceRecipientCount { get; init; }

    public string RecipientHref { get; init; }

    public TaskProgress TaskProgress => ServiceRecipientCount == 0 ? TaskProgress.NotStarted : TaskProgress.Completed;

    public bool? AllRecipientsSelected => ServiceRecipients?.All(x => x.Selected);
}
