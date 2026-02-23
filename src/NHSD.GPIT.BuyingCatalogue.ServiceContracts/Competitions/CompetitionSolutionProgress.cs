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

            bool ServiceHasQuantities(CompetitionCatalogueItem service, Func<IEnumerable<CompetitionItemQuantity>, bool> predicate)
            {
                var quantities = service.Quantities;
                return predicate(quantities);
            }

            if (PriceProgress is not TaskProgress.Completed) return TaskProgress.CannotStart;

            Func<IEnumerable<CompetitionItemQuantity>, bool> anyQuantitiesPredicate = quantities => quantities.Any(quantity => quantity.Quantity.HasValue);
            Func<IEnumerable<CompetitionItemQuantity>, bool> allQuantitiesPredicate = quantities => quantities.All(quantity => quantity.Quantity.HasValue);

            return !HasQuantities(
                    competitionSolution,
                    services =>
                        services.Any(service => ServiceHasQuantities(service, anyQuantitiesPredicate)),
                    anyQuantitiesPredicate) ? TaskProgress.NotStarted
                : HasQuantities(
                    competitionSolution,
                    services =>
                        services.All(service => ServiceHasQuantities(service, allQuantitiesPredicate)),
                    allQuantitiesPredicate) ? TaskProgress.Completed
                : TaskProgress.InProgress;
        }
    }
}
