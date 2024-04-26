using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Competitions;

public static class CompetitionOrderServiceTests
{
    [Fact]
    public static void Constructor_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionOrderService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static Task CreateOrder_InvalidCompetitionId_ThrowsArgumentException(
        string internalOrgId,
        int competitionId,
        CatalogueItemId catalogueItemId,
        CompetitionOrderService service) => FluentActions
        .Awaiting(() => service.CreateOrder(internalOrgId, competitionId, catalogueItemId))
        .Should()
        .ThrowAsync<ArgumentException>("Competition either does not exist or is not yet completed");

    [Theory]
    [InMemoryDbAutoData]
    public static async Task CreateOrder_InvalidCompetitionSolutionId_ThrowsArgumentException(
        Organisation organisation,
        Competition competition,
        CatalogueItemId catalogueItemId,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CompetitionOrderService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.Organisation = organisation;

        dbContext.Organisations.Add(organisation);
        dbContext.Competitions.Add(competition);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        await FluentActions
            .Awaiting(() => service.CreateOrder(organisation.InternalIdentifier, competition.Id, catalogueItemId))
            .Should()
            .ThrowAsync<ArgumentException>("Solution either does not exist or is not a winning Solution");
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task CreateOrder_NonWinningSolution_ThrowsArgumentException(
        Organisation organisation,
        Competition competition,
        Solution solution,
        CompetitionSolution competitionSolution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CompetitionOrderService service)
    {
        competitionSolution.Solution = solution;
        competitionSolution.IsShortlisted = true;
        competitionSolution.IsWinningSolution = false;

        competition.OrganisationId = organisation.Id;
        competition.Organisation = organisation;
        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        dbContext.Organisations.Add(organisation);
        dbContext.Competitions.Add(competition);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        await FluentActions
            .Awaiting(() => service.CreateOrder(organisation.InternalIdentifier, competition.Id, solution.CatalogueItemId))
            .Should()
            .ThrowAsync<ArgumentException>("Solution either does not exist or is not a winning Solution");
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task CreateOrder_WinningSolution_Successful(
        Organisation organisation,
        Competition competition,
        Solution solution,
        CompetitionSolution competitionSolution,
        CompetitionCatalogueItemPrice price,
        CompetitionCatalogueItemPriceTier priceTier,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CompetitionOrderService service)
    {
        price.Tiers = new List<CompetitionCatalogueItemPriceTier> { priceTier };

        competitionSolution.Price = price;
        competitionSolution.Solution = solution;
        competitionSolution.IsShortlisted = true;
        competitionSolution.IsWinningSolution = true;

        competition.OrganisationId = organisation.Id;
        competition.Organisation = organisation;
        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        dbContext.Organisations.Add(organisation);
        dbContext.Competitions.Add(competition);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        await FluentActions
            .Awaiting(() => service.CreateOrder(organisation.InternalIdentifier, competition.Id, solution.CatalogueItemId))
            .Should()
            .NotThrowAsync();
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task CreateOrder_WinningSolution_CreatesOrder(
        Organisation organisation,
        Competition competition,
        Solution solution,
        CompetitionSolution competitionSolution,
        CompetitionCatalogueItemPrice price,
        CompetitionCatalogueItemPriceTier priceTier,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CompetitionOrderService service)
    {
        price.Tiers = new List<CompetitionCatalogueItemPriceTier> { priceTier };

        competitionSolution.Price = price;
        competitionSolution.Solution = solution;
        competitionSolution.IsShortlisted = true;
        competitionSolution.IsWinningSolution = true;

        competition.OrganisationId = organisation.Id;
        competition.Organisation = organisation;
        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        dbContext.Organisations.Add(organisation);
        dbContext.Competitions.Add(competition);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var callOffId = await service.CreateOrder(organisation.InternalIdentifier, competition.Id, solution.CatalogueItemId);

        callOffId.Should().NotBe(default);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task CreateOrder_WinningSolution_SetsOrderDetailsAsExpected(
        Organisation organisation,
        Competition competition,
        List<OdsOrganisation> recipients,
        Solution solution,
        CompetitionSolution competitionSolution,
        CompetitionCatalogueItemPrice price,
        CompetitionCatalogueItemPriceTier priceTier,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CompetitionOrderService service)
    {
        price.Tiers = new List<CompetitionCatalogueItemPriceTier> { priceTier };

        competitionSolution.Price = price;
        competitionSolution.Solution = solution;
        competitionSolution.IsShortlisted = true;
        competitionSolution.IsWinningSolution = true;

        competition.OrganisationId = organisation.Id;
        competition.Organisation = organisation;
        competition.Recipients = recipients;
        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        dbContext.Organisations.Add(organisation);
        dbContext.Competitions.Add(competition);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var callOffId = await service.CreateOrder(organisation.InternalIdentifier, competition.Id, solution.CatalogueItemId);

        var order = await dbContext.Order(callOffId);

        order.Revision.Should().Be(1);
        order.CompetitionId.Should().Be(competition.Id);
        order.Description.Should().Be($"Order created from competition: {competition.Id}");
        order.MaximumTerm.Should().Be(competition.ContractLength);
        order.OrderingPartyId.Should().Be(competition.OrganisationId);
        order.SupplierId.Should().Be(solution.CatalogueItem.SupplierId);
        order.OrderRecipients.Should()
            .BeEquivalentTo(
                recipients.Select(x => new OrderRecipient(x.Id)),
                opt => opt.Excluding(m => m.OrderId).Excluding(m => m.Order).Excluding(m => m.OdsOrganisation));
        order.OrderItems.Should().ContainSingle();
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task CreateOrder_WinningSolution_SetsOrderItemServices(
        Organisation organisation,
        Competition competition,
        List<OdsOrganisation> recipients,
        Solution solution,
        CompetitionSolution competitionSolution,
        CompetitionCatalogueItemPrice price,
        CompetitionCatalogueItemPriceTier priceTier,
        AdditionalService additionalService,
        SolutionService solutionService,
        CompetitionCatalogueItemPrice servicePrice,
        CompetitionCatalogueItemPriceTier servicePriceTier,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CompetitionOrderService service)
    {
        servicePrice.Tiers = new List<CompetitionCatalogueItemPriceTier> { servicePriceTier };
        price.Tiers = new List<CompetitionCatalogueItemPriceTier> { priceTier };

        solutionService.IsRequired = false;
        solutionService.Price = servicePrice;
        solutionService.Service = additionalService.CatalogueItem;

        competitionSolution.Price = price;
        competitionSolution.Solution = solution;
        competitionSolution.IsShortlisted = true;
        competitionSolution.IsWinningSolution = true;
        competitionSolution.SolutionServices = new List<SolutionService> { solutionService };

        competition.OrganisationId = organisation.Id;
        competition.Organisation = organisation;
        competition.Recipients = recipients;
        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        dbContext.Organisations.Add(organisation);
        dbContext.Competitions.Add(competition);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var callOffId = await service.CreateOrder(organisation.InternalIdentifier, competition.Id, solution.CatalogueItemId);

        var order = await dbContext.Order(callOffId);

        order.OrderItems.Should().HaveCount(2);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task CreateOrder_WinningSolution_SetsRecipientQuantities(
        Organisation organisation,
        Competition competition,
        List<OdsOrganisation> recipients,
        Solution solution,
        CompetitionSolution competitionSolution,
        CompetitionCatalogueItemPrice price,
        CompetitionCatalogueItemPriceTier priceTier,
        AdditionalService additionalService,
        SolutionService solutionService,
        CompetitionCatalogueItemPrice servicePrice,
        CompetitionCatalogueItemPriceTier servicePriceTier,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CompetitionOrderService service)
    {
        servicePrice.Tiers = new List<CompetitionCatalogueItemPriceTier> { servicePriceTier };
        price.Tiers = new List<CompetitionCatalogueItemPriceTier> { priceTier };

        solutionService.IsRequired = false;
        solutionService.Price = servicePrice;
        solutionService.Service = additionalService.CatalogueItem;
        solutionService.Quantities = recipients.Select(
                x => new ServiceQuantity
                    {
                        OdsCode = x.Id, Quantity = 5, ServiceId = additionalService.CatalogueItemId,
                    })
            .ToList();

        competitionSolution.Price = price;
        competitionSolution.Solution = solution;
        competitionSolution.IsShortlisted = true;
        competitionSolution.IsWinningSolution = true;
        competitionSolution.SolutionServices = new List<SolutionService> { solutionService };
        competitionSolution.Quantities = recipients.Select(
                x => new SolutionQuantity { OdsCode = x.Id, Quantity = 5, SolutionId = solution.CatalogueItemId })
            .ToList();

        competition.OrganisationId = organisation.Id;
        competition.Organisation = organisation;
        competition.Recipients = recipients;
        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        dbContext.Organisations.Add(organisation);
        dbContext.Competitions.Add(competition);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var callOffId = await service.CreateOrder(
            organisation.InternalIdentifier,
            competition.Id,
            solution.CatalogueItemId);

        var order = await dbContext.Order(callOffId);

        order.OrderRecipients.SelectMany(x => x.OrderItemRecipients)
            .GroupBy(x => x.CatalogueItemId)
            .Should()
            .HaveCount(2);
    }
}
