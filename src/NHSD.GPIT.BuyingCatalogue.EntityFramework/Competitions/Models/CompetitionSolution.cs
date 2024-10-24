﻿using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

public class CompetitionSolution : ICompetitionPriceEntity
{
    public CompetitionSolution(int competitionId, CatalogueItemId solutionId)
    {
        CompetitionId = competitionId;
        SolutionId = solutionId;
    }

    public int CompetitionId { get; set; }

    public CatalogueItemId SolutionId { get; set; }

    public int? CompetitionItemPriceId { get; set; }

    public bool IsShortlisted { get; set; }

    public bool IsWinningSolution { get; set; }

    public string Justification { get; set; }

    /// <summary>
    /// Gets or sets the global quantity when the <see cref="Price"/> is a global pricing model.
    /// </summary>
    public int? Quantity { get; set; }

    public Competition Competition { get; set; }

    public Solution Solution { get; set; }

    public CompetitionCatalogueItemPrice Price { get; set; }

    public ICollection<SolutionService> SolutionServices { get; set; } = new HashSet<SolutionService>();

    public ICollection<SolutionScore> Scores { get; set; } = new HashSet<SolutionScore>();

    /// <summary>
    /// Gets or sets the quantities for each service recipient, when the <see cref="Price"/> is based on the practice list size.
    /// </summary>
    public ICollection<SolutionQuantity> Quantities { get; set; } = new HashSet<SolutionQuantity>();

    public bool HasScoreType(ScoreType type) => Scores.Any(x => x.ScoreType == type);

    public SolutionScore GetScoreByType(ScoreType type) => Scores?.FirstOrDefault(x => x.ScoreType == type);

    public ICollection<SolutionService> GetAssociatedServices() => SolutionServices.Where(
            x => !x.IsRequired && x.Service.CatalogueItemType is CatalogueItemType.AssociatedService)
        .ToList();

    public decimal? CalculateTotalPrice(int contractLength)
    {
        var price = Price as IPrice;

        var solutionMonthlyCost =
            price?.CalculateCostPerMonth(Quantity ?? Quantities.Sum(x => x.Quantity));
        var servicesMonthlyCost = SolutionServices?.Sum(
            x => ((IPrice)x.Price)?.CalculateCostPerMonth(x.Quantity ?? x.Quantities.Sum(y => y.Quantity)));
        var oneOffCost = GetAssociatedServices()
            .Sum(x => ((IPrice)x.Price)?.CalculateOneOffCost(x.Quantity ?? x.Quantities.Sum(y => y.Quantity)));

        return oneOffCost + ((solutionMonthlyCost + servicesMonthlyCost) * contractLength);
    }
}
