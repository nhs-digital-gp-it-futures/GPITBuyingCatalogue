using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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

    public ICollection<CompetitionCatalogueItem> Services { get; set; } = new HashSet<CompetitionCatalogueItem>();

    public ICollection<SolutionScore> Scores { get; set; } = new HashSet<SolutionScore>();

    public IEnumerable<CompetitionAdditionalService> GetAdditionalServices() =>
        Services.OfType<CompetitionAdditionalService>();

    public IEnumerable<CompetitionAssociatedService> GetAssociatedServices() =>
        Services.OfType<CompetitionAssociatedService>();

    public bool HasScoreType(ScoreType type) => Scores.Any(x => x.ScoreType == type);

    public SolutionScore GetScoreByType(ScoreType type) => Scores?.FirstOrDefault(x => x.ScoreType == type);

    public decimal? CalculateTotalPrice(int contractLength)
    {
        IPrice price = Price;

        var solutionMonthlyCost =
            price?.CalculateCostPerMonth(Quantities.Sum(x => x.Quantity.GetValueOrDefault()));
        var servicesMonthlyCost = Services?.Sum(x =>
            ((IPrice)x.Price)?.CalculateCostPerMonth(x.Quantities.Sum(y => y.Quantity.GetValueOrDefault())));
        var oneOffCost = Services?
            .Sum(x => ((IPrice)x.Price)?.CalculateOneOffCost(x.Quantities.Sum(y => y.Quantity.GetValueOrDefault())));

        return ((solutionMonthlyCost.GetValueOrDefault() + servicesMonthlyCost.GetValueOrDefault()) * contractLength)
            + oneOffCost.GetValueOrDefault();
    }
}
