using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Competitions;

public static class CompetitionOrderServiceTests
{
    [Fact]
    public static void Constructor_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionOrderService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static Task CreateDirectAwardOrder_InvalidCompetitionId_ThrowsArgumentException(
        string internalOrgId,
        int competitionId,
        CatalogueItemId catalogueItemId,
        CompetitionOrderService service) => FluentActions
        .Awaiting(() => service.CreateDirectAwardOrder(internalOrgId, competitionId, catalogueItemId))
        .Should()
        .ThrowAsync<ArgumentException>("Competition either does not exist or is not yet completed");

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task CreateDirectAwardOrder_InvalidCompetitionSolutionId_ThrowsArgumentException(
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
            .Awaiting(() => service.CreateDirectAwardOrder(organisation.InternalIdentifier, competition.Id, catalogueItemId))
            .Should()
            .ThrowAsync<ArgumentException>("Solution does not exist on competition");
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task CreateDirectAwardOrder_Solution_CreatesOrder(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        Solution solution,
        CompetitionSolution competitionSolution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CompetitionOrderService service)
    {
        competitionSolution.CatalogueItem = solution.CatalogueItem;

        competition.OrganisationId = organisation.Id;
        competition.Organisation = organisation;
        competition.CompetitionSolutions = [competitionSolution];
        competition.CompetitionSublocations = competitionSublocations;

        dbContext.Organisations.Add(organisation);
        dbContext.Competitions.Add(competition);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var callOffId = await service.CreateDirectAwardOrder(organisation.InternalIdentifier, competition.Id, solution.CatalogueItemId);

        callOffId.Should().NotBe(default);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task CreateDirectAwardOrder_Solution_SetsOrderDetailsAsExpected(
        Organisation organisation,
        Competition competition,
        Solution solution,
        CompetitionSolution competitionSolution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CompetitionOrderService service)
    {
        competitionSolution.CatalogueItem = solution.CatalogueItem;

        competition.OrganisationId = organisation.Id;
        competition.Organisation = organisation;
        competition.CompetitionSolutions = [competitionSolution];

        dbContext.Organisations.Add(organisation);
        dbContext.Competitions.Add(competition);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var callOffId = await service.CreateDirectAwardOrder(organisation.InternalIdentifier, competition.Id, solution.CatalogueItemId);

        var order = await dbContext.Order(callOffId);

        order.Revision.Should().Be(1);
        order.CompetitionId.Should().Be(competition.Id);
        order.Description.Should().Be($"Order created from competition: {competition.Id}");
        order.OrderingPartyId.Should().Be(competition.OrganisationId);
        order.SupplierId.Should().Be(solution.CatalogueItem.SupplierId);
        order.OrderItems.Select(o => o.CatalogueItemId).Should().BeEquivalentTo([solution.CatalogueItemId]);
        order.SelectedFrameworkId.Should().Be(competition.FrameworkId);
        order.OrderType.Value.Should().Be(OrderTypeEnum.Solution);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task CreateDirectAwardOrder_Solution_SetsOrderItemServices(
        Organisation organisation,
        Competition competition,
        Solution solution,
        CompetitionSolution competitionSolution,
        AdditionalService additionalService,
        CompetitionAdditionalService solutionService,
        CompetitionCatalogueItemPrice price,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CompetitionOrderService service)
    {
        solutionService.CatalogueItem = additionalService.CatalogueItem;

        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.Services = [solutionService];
        solutionService.Price = price;

        competition.OrganisationId = organisation.Id;
        competition.Organisation = organisation;
        competition.CompetitionSolutions = [competitionSolution];

        dbContext.Organisations.Add(organisation);
        dbContext.Competitions.Add(competition);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var callOffId = await service.CreateDirectAwardOrder(organisation.InternalIdentifier, competition.Id, solution.CatalogueItemId);

        var order = await dbContext.Order(callOffId);

        order.OrderItems.Should().HaveCount(2);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static Task CreateOrder_InvalidCompetitionId_ThrowsArgumentException(
        string internalOrgId,
        int competitionId,
        CatalogueItemId catalogueItemId,
        CompetitionOrderService service) => FluentActions
        .Awaiting(() => service.CreateOrder(internalOrgId, competitionId, catalogueItemId))
        .Should()
        .ThrowAsync<ArgumentException>("Competition either does not exist or is not yet completed");

    [Theory]
    [MockInMemoryDbAutoData]
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
    [MockInMemoryDbAutoData]
    public static async Task CreateOrder_NonWinningSolution_ThrowsArgumentException(
        Organisation organisation,
        Competition competition,
        Solution solution,
        CompetitionSolution competitionSolution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CompetitionOrderService service)
    {
        competitionSolution.CatalogueItem = solution.CatalogueItem;
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
    [MockInMemoryDbAutoData]
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
        competitionSolution.CatalogueItem = solution.CatalogueItem;
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
    [MockInMemoryDbAutoData]
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
        competitionSolution.CatalogueItem = solution.CatalogueItem;
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
    [MockInMemoryDbAutoData]
    public static async Task CreateOrder_WinningSolution_SetsOrderDetailsAsExpected(
        Organisation organisation,
        Competition competition,
        Solution solution,
        CompetitionSolution competitionSolution,
        List<CompetitionSublocation> competitionSublocations,
        CompetitionCatalogueItemPrice price,
        CompetitionCatalogueItemPriceTier priceTier,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CompetitionOrderService service)
    {
        price.Tiers = new List<CompetitionCatalogueItemPriceTier> { priceTier };

        competitionSolution.Price = price;
        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.IsShortlisted = true;
        competitionSolution.IsWinningSolution = true;

        competition.OrganisationId = organisation.Id;
        competition.Organisation = organisation;
        competition.CompetitionSublocations = competitionSublocations;
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
        order.FlattenedRecipients.Should()
            .BeEquivalentTo(
                competition.FlattenedRecipients.Select(x => new OrderSublocationRecipient(x)),
                opt => opt.Excluding(m => m.OrderId)
                    .Excluding(m => m.Order)
                    .Excluding(m => m.RecipientOdsOrganisation)
                    .Excluding(m => m.ParentSublocation));
        order.OrderItems.Select(o => o.CatalogueItemId).Should().BeEquivalentTo([solution.CatalogueItemId]);
        order.SelectedFrameworkId.Should().Be(competition.FrameworkId);
        order.OrderType.Value.Should().Be(OrderTypeEnum.Solution);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task CreateOrder_WinningSolution_SetsOrderItemServices(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        Solution solution,
        CompetitionSolution competitionSolution,
        CompetitionCatalogueItemPrice price,
        CompetitionCatalogueItemPriceTier priceTier,
        AdditionalService additionalService,
        CompetitionAdditionalService solutionService,
        CompetitionCatalogueItemPrice servicePrice,
        CompetitionCatalogueItemPriceTier servicePriceTier,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CompetitionOrderService service)
    {
        servicePrice.Tiers = new List<CompetitionCatalogueItemPriceTier> { servicePriceTier };
        price.Tiers = new List<CompetitionCatalogueItemPriceTier> { priceTier };

        solutionService.IsRequired = false;
        solutionService.Price = servicePrice;
        solutionService.CatalogueItem = additionalService.CatalogueItem;

        competitionSolution.Price = price;
        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.IsShortlisted = true;
        competitionSolution.IsWinningSolution = true;
        competitionSolution.Services = [solutionService];

        competition.OrganisationId = organisation.Id;
        competition.Organisation = organisation;
        competition.CompetitionSublocations = competitionSublocations;
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
    [MockInMemoryDbAutoData]
    public static async Task CreateOrder_WinningSolution_SetsRecipientQuantities(
        Organisation organisation,
        Competition competition,
        CompetitionSublocationRecipient competitionSublocationRecipient,
        CompetitionSublocation competitionSublocation,
        Solution solution,
        CompetitionSolution competitionSolution,
        CompetitionCatalogueItemPrice solutionPrice,
        CompetitionCatalogueItemPriceTier solutionPriceTier,
        AdditionalService additionalService,
        CompetitionAdditionalService competitionAdditionalService,
        CompetitionCatalogueItemPrice additionalServicePrice,
        CompetitionCatalogueItemPriceTier additionalServicePriceTier,
        AssociatedService associatedService,
        CompetitionAssociatedService competitionAssociatedService,
        CompetitionCatalogueItemPrice associatedServicePrice,
        CompetitionCatalogueItemPriceTier associatedServicePriceTier,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CompetitionOrderService service)
    {
        var competitionSolutionId = 1001;
        var competitionAdditionalServiceId = 1002;
        var competitionAssociatedServiceId = 1003;

        competition.OrganisationId = organisation.Id;
        competition.Organisation = organisation;

        competitionSublocationRecipient.CompetitionId = competition.Id;
        competitionSublocationRecipient.Competition = competition;

        competitionSublocation.CompetitionId = competition.Id;
        competitionSublocation.Competition = competition;
        competitionSublocation.SublocationRecipients = [competitionSublocationRecipient];

        competition.CompetitionSublocations = [competitionSublocation];

        var additionalServiceQuantity = 15;
        additionalServicePrice.Tiers = [additionalServicePriceTier];
        competitionAdditionalService.Id = competitionAdditionalServiceId;
        competitionAdditionalService.ParentItemId = competitionSolutionId;
        competitionAdditionalService.Price = additionalServicePrice;
        competitionAdditionalService.CatalogueItemId = additionalService.CatalogueItem.Id;
        competitionAdditionalService.CatalogueItem = additionalService.CatalogueItem;
        competitionAdditionalService.CompetitionId = competition.Id;
        competitionAdditionalService.Quantities = [.. competition.FlattenedRecipients.Select(x =>
            new CompetitionItemQuantity
            {
                CompetitionId = competition.Id,
                CompetitionItemId = competitionAdditionalServiceId,
                ParentSublocationOdsCode = x.ParentSublocationOdsCode,
                RecipientOdsCode = x.RecipientOdsCode,
                Quantity = additionalServiceQuantity,
            })];

        var associatedServiceQuantity = 10;
        associatedServicePrice.Tiers = [associatedServicePriceTier];
        competitionAssociatedService.Id = competitionAssociatedServiceId;
        competitionAssociatedService.ParentItemId = competitionSolutionId;
        competitionAssociatedService.Price = associatedServicePrice;
        competitionAssociatedService.CatalogueItemId = associatedService.CatalogueItem.Id;
        competitionAssociatedService.CatalogueItem = associatedService.CatalogueItem;
        competitionAssociatedService.Services = [];
        competitionAssociatedService.CompetitionId = competition.Id;
        competitionAssociatedService.Quantities = [.. competition.FlattenedRecipients.Select(x =>
            new CompetitionItemQuantity
            {
                CompetitionId = competition.Id,
                CompetitionItemId = competitionAssociatedServiceId,
                ParentSublocationOdsCode = x.ParentSublocationOdsCode,
                RecipientOdsCode = x.RecipientOdsCode,
                Quantity = associatedServiceQuantity,
            })];

        var competitionSolutionQuantity = 5;
        solutionPrice.Tiers = [solutionPriceTier];
        competitionSolution.Id = competitionSolutionId;
        competitionSolution.ParentItemId = null;
        competitionSolution.Price = solutionPrice;
        competitionSolution.CatalogueItemId = solution.CatalogueItem.Id;
        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.IsShortlisted = true;
        competitionSolution.IsWinningSolution = true;
        competitionSolution.Services = [competitionAdditionalService, competitionAssociatedService];
        competitionSolution.CompetitionId = competition.Id;
        competitionSolution.Quantities = [.. competition.FlattenedRecipients
            .Select(x => new CompetitionItemQuantity()
            {
                CompetitionId = competition.Id,
                CompetitionItemId = competitionSolutionId,
                ParentSublocationOdsCode = x.ParentSublocationOdsCode,
                RecipientOdsCode = x.RecipientOdsCode,
                Quantity = competitionSolutionQuantity,
            })];

        competition.CompetitionSolutions = [competitionSolution];

        dbContext.Organisations.Add(organisation);
        dbContext.Competitions.Add(competition);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var callOffId = await service.CreateOrder(
            organisation.InternalIdentifier,
            competition.Id,
            solution.CatalogueItemId);

        var order = await dbContext.Order(callOffId);
        order.FlattenedRecipients.Count().Should().Be(1);
        order.FlattenedRecipients.First().OrderItemSublocationRecipients.Count.Should().Be(3);

        var additionalAssociatedServiceRecipients = order.FlattenedRecipients.First().OrderItemSublocationRecipients
            .FirstOrDefault(x => x.OrderItem.CatalogueItemId == competitionSolution.CatalogueItemId);
        additionalAssociatedServiceRecipients.Should().NotBeNull();
        additionalAssociatedServiceRecipients.Quantity.Should().Be(competitionSolutionQuantity);

        var additionalServiceRecipients = order.FlattenedRecipients.First().OrderItemSublocationRecipients
            .FirstOrDefault(x => x.OrderItem.CatalogueItemId == additionalService.CatalogueItemId);
        additionalServiceRecipients.Should().NotBeNull();
        additionalServiceRecipients.Quantity.Should().Be(additionalServiceQuantity);

        var associatedServiceRecipients = order.FlattenedRecipients.First().OrderItemSublocationRecipients
            .FirstOrDefault(x => x.OrderItem.CatalogueItemId == associatedService.CatalogueItemId);
        associatedServiceRecipients.Should().NotBeNull();
        associatedServiceRecipients.Quantity.Should().Be(associatedServiceQuantity);
    }
}
