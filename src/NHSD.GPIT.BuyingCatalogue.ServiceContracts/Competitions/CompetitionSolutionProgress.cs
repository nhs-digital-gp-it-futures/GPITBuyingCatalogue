using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

public readonly struct CompetitionSolutionProgress(
    CompetitionSolution competitionSolution,
    ICollection<CompetitionSublocationRecipient> competitionRecipients)
{
    public TaskProgress Progress => (PriceProgress, QuantityProgress) switch
    {
        (TaskProgress.Completed, TaskProgress.Completed) => TaskProgress.Completed,
        (_, TaskProgress.InProgress or TaskProgress.Completed) => TaskProgress.InProgress,
        (TaskProgress.InProgress or TaskProgress.Completed, _) => TaskProgress.InProgress,
        _ => TaskProgress.NotStarted,
    };

    private TaskProgress PriceProgress
    {
        get
        {
            if (competitionSolution.Price == null && (competitionSolution.Services.Count == 0
                    || competitionSolution.Services.All(x => x.Price == null)))
                return TaskProgress.NotStarted;

            return (competitionSolution.Price != null && (competitionSolution.Services.Count == 0
                || competitionSolution.Services.All(x => x.Price != null)))
                ? TaskProgress.Completed
                : TaskProgress.InProgress;
        }
    }

    private TaskProgress QuantityProgress
    {
        get
        {
            bool HasQuantities(
                CompetitionSolution solution,
                ICollection<CompetitionSublocationRecipient> recipients)
            {
                return recipients.All(recipient =>
                        solution.Quantities.Where(quantity => quantity.RecipientOdsCode == recipient.RecipientOdsCode)
                            .All(quantity => quantity.Quantity.HasValue))
                    && (solution.Services.Count == 0 || recipients.All(recipient =>
                        solution.Services.All(service =>
                            service.Quantities
                                .Where(quantity => quantity.RecipientOdsCode == recipient.RecipientOdsCode)
                                .All(quantity => quantity.Quantity.HasValue))));
            }

            if (PriceProgress is not TaskProgress.Completed) return TaskProgress.CannotStart;

            if (!HasQuantities(competitionSolution, competitionRecipients))
                return TaskProgress.NotStarted;

            return HasQuantities(competitionSolution, competitionRecipients)
                ? TaskProgress.Completed
                : TaskProgress.InProgress;
        }
    }
}
