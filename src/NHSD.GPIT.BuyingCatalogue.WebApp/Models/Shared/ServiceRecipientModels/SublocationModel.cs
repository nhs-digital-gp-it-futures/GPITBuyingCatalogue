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
        Name = competitionSublocation.SublocationOrganisation.Name;
        OdsCode = competitionSublocation.SublocationOdsCode;
        ServiceRecipients = competitionSublocation.SublocationRecipients.Select(
                x => new ServiceRecipientModel
                {
                    OdsCode = x.RecipientOdsCode,
                    Name = x.RecipientOrganisation.Name,
                    Location = competitionSublocation.Competition.Organisation.Name,
                    Selected = presenceDeterminesSelected,
                })
            .ToList();
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

    public List<ServiceRecipientModel> ServiceRecipients { get; init; }

    public int ServiceRecipientCount { get; init; }

    public TaskProgress TaskProgress => ServiceRecipientCount == 0 ? TaskProgress.NotStarted : TaskProgress.Completed;

    public bool? AllRecipientsSelected => ServiceRecipients?.All(x => x.Selected);

    public string RecipientHref { get; init; }
}
