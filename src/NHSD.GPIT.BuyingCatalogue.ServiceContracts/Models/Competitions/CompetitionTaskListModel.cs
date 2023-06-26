using System;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.Competitions;

public class CompetitionTaskListModel
{
    public CompetitionTaskListModel(Competition competition)
    {
        Id = competition.Id;
        Name = competition.Name;
        Description = competition.Description;

        SetSectionOneStatuses(competition);
        SetSectionTwoStatuses(competition);
    }

    public int Id { get; }

    public string Name { get; }

    public string Description { get; }

    public TaskProgress SolutionSelection => TaskProgress.Completed;

    public TaskProgress ServiceRecipients { get; private set; } = TaskProgress.CannotStart;

    public TaskProgress ContractLength { get; private set; } = TaskProgress.CannotStart;

    public TaskProgress AwardCriteria { get; private set; } = TaskProgress.CannotStart;

    public TaskProgress AwardCriteriaWeightings { get; private set; } = TaskProgress.CannotStart;

    public TaskProgress NonPriceElements { get; private set; } = TaskProgress.CannotStart;

    public TaskProgress NonPriceWeightings { get; private set; } = TaskProgress.CannotStart;

    public TaskProgress ReviewCompetitionCriteria { get; private set; } = TaskProgress.CannotStart;

    public TaskProgress CompareAndScoreSolutions { get; private set; } = TaskProgress.CannotStart;

    public TaskProgress CalculatePrice { get; private set; } = TaskProgress.CannotStart;

    public TaskProgress ViewResults { get; private set; } = TaskProgress.CannotStart;

    private static TaskProgress CompletedOrNotStarted(Competition competition, Predicate<Competition> predicate) =>
        predicate.Invoke(competition) ? TaskProgress.Completed : TaskProgress.NotStarted;

    private void SetSectionOneStatuses(Competition competition)
    {
        ServiceRecipients = CompletedOrNotStarted(competition, c => c.Recipients.Any());

        if (ServiceRecipients is TaskProgress.NotStarted) return;

        ContractLength = CompletedOrNotStarted(competition, c => c.ContractLength.HasValue);
    }

    private void SetSectionTwoStatuses(Competition competition)
    {
        if (ContractLength is TaskProgress.NotStarted) return;

        AwardCriteria = CompletedOrNotStarted(competition, c => c.IncludesNonPrice.HasValue);

        if (AwardCriteria is TaskProgress.NotStarted) return;

        if (!competition.IncludesNonPrice.GetValueOrDefault())
        {
            AwardCriteriaWeightings = NonPriceElements =
                NonPriceWeightings = ReviewCompetitionCriteria = TaskProgress.NotApplicable;

            return;
        }

        AwardCriteriaWeightings = CompletedOrNotStarted(
            competition,
            c => c.Weightings is { Price: not null, NonPrice: not null });
    }
}
