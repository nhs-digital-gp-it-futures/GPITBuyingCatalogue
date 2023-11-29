using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels.OrderingInformationModels;

public class OrderingInformationModel : NavBaseModel
{
    [ExcludeFromCodeCoverage]
    public OrderingInformationModel()
    {
    }

    public OrderingInformationModel(
        Competition competition,
        CompetitionSolution competitionSolution)
    {
        InternalOrgId = competition.Organisation.InternalIdentifier;
        CompetitionId = competition.Id;
        CompetitionName = competition.Name;

        NumberOfRecipients = competition.Recipients.Count;
        ContractLength = competition.ContractLength.GetValueOrDefault();

        CompetitionSolution = competitionSolution;
        SolutionDisplay = new(
            competitionSolution.Solution.CatalogueItem,
            competitionSolution.Price,
            competitionSolution.Quantity ?? competitionSolution.Quantities.Sum(y => y.Quantity));

        Items = competitionSolution.SolutionServices.Select(
                x => new OrderingInformationItem(
                    x.Service,
                    x.Price,
                    x.Quantity ?? x.Quantities.Sum(y => y.Quantity)))
            .ToList();
    }

    public string InternalOrgId { get; set; }

    public int CompetitionId { get; set; }

    public string CompetitionName { get; set; }

    public int NumberOfRecipients { get; set; }

    public int ContractLength { get; set; }

    public CompetitionSolution CompetitionSolution { get; set; }

    public OrderingInformationItem SolutionDisplay { get; set; }

    public ICollection<OrderingInformationItem> Items { get; set; }

    public ICollection<OrderingInformationItem> GetAssociatedServices() =>
        GetItemByItemType(CatalogueItemType.AssociatedService);

    public ICollection<OrderingInformationItem> GetAdditionalServices() =>
        GetItemByItemType(CatalogueItemType.AdditionalService);

    public decimal CalculateTotalOneOffCost() => SolutionDisplay.Price.CalculateOneOffCost(SolutionDisplay.Quantity)
        + Items.Sum(x => x.Price.CalculateOneOffCost(x.Quantity));

    public decimal CalculateTotalMonthlyCost() => SolutionDisplay.Price.CalculateCostPerMonth(SolutionDisplay.Quantity)
        + Items.Sum(x => x.Price.CalculateCostPerMonth(x.Quantity));

    public decimal CalculateTotalYearlyCost() => SolutionDisplay.Price.CalculateCostPerYear(SolutionDisplay.Quantity)
        + Items.Sum(x => x.Price.CalculateCostPerYear(x.Quantity));

    [ExcludeFromCodeCoverage]
    public decimal CalculateTotalCost() => CompetitionSolution.CalculateTotalPrice(ContractLength).GetValueOrDefault();

    private ICollection<OrderingInformationItem> GetItemByItemType(CatalogueItemType catalogueItemType) =>
        Items.Where(x => x.CatalogueItemType == catalogueItemType).ToList();
}
