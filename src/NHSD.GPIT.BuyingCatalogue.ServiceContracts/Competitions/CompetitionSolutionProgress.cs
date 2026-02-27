using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

public readonly struct CompetitionSolutionProgress(
    CompetitionSolution competitionSolution)
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
                Func<ICollection<CompetitionCatalogueItem>, bool> servicesHaveQuantitiesPredicate,
                Func<IEnumerable<CompetitionItemQuantity>, bool> solutionsHaveQuantitiesPredicate)
            {
                    return solutionsHaveQuantitiesPredicate(solution.Quantities)
                    && (solution.Services.Count == 0 || servicesHaveQuantitiesPredicate(solution.Services));
            }

            bool AnyQuantitiesPredicate(IEnumerable<CompetitionItemQuantity> quantities) => quantities.Any(quantity => quantity.Quantity.HasValue);
            bool AllQuantitiesPredicate(IEnumerable<CompetitionItemQuantity> quantities) => quantities.All(quantity => quantity.Quantity.HasValue);

            if (PriceProgress is not TaskProgress.Completed) return TaskProgress.CannotStart;

            if (!HasQuantities(
                    competitionSolution,
                    services =>
                        services.Any(service => AnyQuantitiesPredicate(service.Quantities)),
                    AnyQuantitiesPredicate))
                return TaskProgress.NotStarted;

            return HasQuantities(
                    competitionSolution,
                    services =>
                        services.All(service => AllQuantitiesPredicate(service.Quantities)),
                    AllQuantitiesPredicate) ? TaskProgress.Completed : TaskProgress.InProgress;
        }
    }
}
