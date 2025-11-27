using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

public record SublocationModel
{
    public SublocationModel()
    {
    }

    public SublocationModel(CompetitionSublocation competitionSublocation, bool presenceDeterminesSelected)
    {
        Name = competitionSublocation.SublocationOrganisation?.Name;
        OdsCode = competitionSublocation.SublocationOdsCode;
        ServiceRecipientCount = competitionSublocation.SublocationRecipients.Count;
        ServiceRecipients = competitionSublocation.SublocationRecipients
            .Select(x => new ServiceRecipientModel(x, presenceDeterminesSelected))
            .ToArray();
    }

    public SublocationModel(OrderSublocation orderSublocation, bool presenceDeterminesSelected)
    {
        Name = orderSublocation.SublocationOrganisation?.Name;
        OdsCode = orderSublocation.SublocationOdsCode;
        ServiceRecipientCount = orderSublocation.SublocationRecipients?.Count;
        ServiceRecipients = orderSublocation.SublocationRecipients?.Select(
                x => new ServiceRecipientModel(x, presenceDeterminesSelected))
            .ToArray();
    }

    public SublocationModel(
        CompetitionSublocation competitionSublocation,
        string recipientLink,
        int serviceRecipientCount,
        TaskProgress taskProgress)
    {
        Name = competitionSublocation.SublocationOrganisation.Name;
        ServiceRecipientCount = serviceRecipientCount;
        OdsCode = competitionSublocation.SublocationOdsCode;
        RecipientLink = recipientLink;
        TaskProgress = taskProgress;
    }

    public SublocationModel(
        OrderSublocation orderSublocation,
        string recipientLink,
        int serviceRecipientCount,
        TaskProgress taskProgress)
    {
        Name = orderSublocation.SublocationOrganisation.Name;
        ServiceRecipientCount = serviceRecipientCount;
        OdsCode = orderSublocation.SublocationOdsCode;
        RecipientLink = recipientLink;
        TaskProgress = taskProgress;
    }

    public string Name { get; init; }

    public string OdsCode { get; init; }

    public IReadOnlyList<ServiceRecipientModel> ServiceRecipients { get; init; }

    public int? ServiceRecipientCount { get; init; }

    public string RecipientLink { get; init; }

    public TaskProgress TaskProgress { get; init; }

    public bool? AllRecipientsSelected => ServiceRecipients?.All(x => x.Selected);
}
