using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class CompetitionSolution : CompetitionCatalogueItem
{
    public CompetitionSolution()
    {
    }

    public CompetitionSolution(
        int competitionId,
        CatalogueItemId catalogueItemId)
        : base(competitionId, catalogueItemId)
    {
    }

    public bool IsShortlisted { get; set; }

    [MaxLength(1000)]
    public string Justification { get; set; }

    public bool IsWinningSolution { get; set; }

    public bool AssociatedServicesAvailable { get; set; }

    public bool AssociatedServicesRemaining { get; set; }

    public IEnumerable<CompetitionAdditionalService> CompetitionAdditionalServices =>
        Services?.OfType<CompetitionAdditionalService>() ?? [];

    public IEnumerable<CompetitionAssociatedService> CompetitionAssociatedServices =>
        Services?.OfType<CompetitionAssociatedService>() ?? [];

    public ICollection<SolutionScore> Scores { get; set; } = new HashSet<SolutionScore>();

    public IEnumerable<CompetitionAdditionalService> GetAdditionalServices() => CompetitionAdditionalServices;

    public IEnumerable<CompetitionAssociatedService> GetAssociatedServices() => CompetitionAssociatedServices;

    public IEnumerable<CompetitionCatalogueItem> GetAllServices()
    {
        foreach (var additionalService in CompetitionAdditionalServices)
        {
            yield return additionalService;
        }

        foreach (var associatedService in CompetitionAssociatedServices)
        {
            yield return associatedService;
        }

        foreach (var additionalService in CompetitionAdditionalServices)
        {
            foreach (var associatedService in additionalService.CompetitionAssociatedServices)
            {
                yield return associatedService;
            }
        }
    }

    public bool HasScoreType(ScoreType type) => Scores.Any(x => x.ScoreType == type);

    public SolutionScore GetScoreByType(ScoreType type) => Scores?.FirstOrDefault(x => x.ScoreType == type);

    public decimal? CalculateTotalPrice(int contractLength)
    {
        var solutionMonthlyCost = ((IPrice)Price)?.CalculateCostPerMonth(Quantities.Sum(x => x.Quantity.GetValueOrDefault()));
        var servicesMonthlyCost = GetAllServices().Sum(CalculateCostPerMonth);
        var servicesOneOffCost = GetAllServices().Sum(CalculateOneOffCost);

        return ((solutionMonthlyCost.GetValueOrDefault() + servicesMonthlyCost.GetValueOrDefault()) * contractLength)
            + servicesOneOffCost.GetValueOrDefault();
    }

    private static decimal? CalculateCostPerMonth(CompetitionCatalogueItem service)
    {
        var quantity = service.Quantities.Sum(x => x.Quantity.GetValueOrDefault());

        return ((IPrice)service.Price)?.CalculateCostPerMonth(quantity);
    }

    private static decimal? CalculateOneOffCost(CompetitionCatalogueItem service)
    {
        var quantity = service.Quantities.Sum(x => x.Quantity.GetValueOrDefault());

        return ((IPrice)service.Price)?.CalculateOneOffCost(quantity);
    }
}
