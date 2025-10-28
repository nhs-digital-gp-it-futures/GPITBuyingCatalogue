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

    public ICollection<CompetitionCatalogueItem> Services { get; set; } = new HashSet<CompetitionCatalogueItem>();

    public ICollection<CompetitionAdditionalService> AdditionalServices => Services.OfType<CompetitionAdditionalService>().ToList();

    public ICollection<CompetitionAssociatedService> AssociatedServices => Services.OfType<CompetitionAssociatedService>().ToList();

    public ICollection<SolutionScore> Scores { get; set; } = new HashSet<SolutionScore>();

    public bool HasScoreType(ScoreType type) => Scores.Any(x => x.ScoreType == type);

    public SolutionScore GetScoreByType(ScoreType type) => Scores?.FirstOrDefault(x => x.ScoreType == type);

    public decimal? CalculateTotalPrice(int contractLength)
    {
        var price = Price as IPrice;

        var solutionMonthlyCost =
            price?.CalculateCostPerMonth(Quantity ?? Quantities.Sum(x => x.Quantity));
        var servicesMonthlyCost = Services?.Sum(x =>
            ((IPrice)x.Price)?.CalculateCostPerMonth(x.Quantity ?? x.Quantities.Sum(y => y.Quantity)));
        var oneOffCost = AssociatedServices
            .Sum(x => ((IPrice)x.Price)?.CalculateOneOffCost(x.Quantity ?? x.Quantities.Sum(y => y.Quantity)));

        return oneOffCost + ((solutionMonthlyCost + servicesMonthlyCost) * contractLength);
    }
}
