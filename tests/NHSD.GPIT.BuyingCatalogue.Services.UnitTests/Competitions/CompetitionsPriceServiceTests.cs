using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Competitions;

public static class CompetitionsPriceServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionsPriceService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonAutoData]
    public static Task SetSolutionPrice_NullAgreedPrices_ThrowsException(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CataloguePrice cataloguePrice,
        CompetitionsPriceService service) => FluentActions
        .Awaiting(() => service.SetSolutionPrice(internalOrgId, competitionId, solutionId, cataloguePrice, null))
        .Should()
        .ThrowAsync<ArgumentNullException>();

    [Theory]
    [CommonAutoData]
    public static Task SetServicePrice_NullAgreedPrices_ThrowsException(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId servicePrice,
        CataloguePrice cataloguePrice,
        CompetitionsPriceService service) => FluentActions
        .Awaiting(() => service.SetServicePrice(internalOrgId, competitionId, solutionId, servicePrice, cataloguePrice, null))
        .Should()
        .ThrowAsync<ArgumentNullException>();

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetSolutionPrice_NoExistingPrice_SetsPrice(
        Organisation organisation,
        Competition competition,
        Solution solution,
        CataloguePrice price,
        List<CataloguePriceTier> tiers,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsPriceService service)
    {
        tiers.ForEach(x =>
        {
            x.CataloguePrice = null;
        });

        price.CataloguePriceTiers = tiers;
        solution.CatalogueItem.CataloguePrices = new List<CataloguePrice> { price, };

        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions.Add(new CompetitionSolution(competition.Id, solution.CatalogueItemId) { IsShortlisted = true });

        context.Solutions.Add(solution);
        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        await service.SetSolutionPrice(
            organisation.InternalIdentifier,
            competition.Id,
            solution.CatalogueItemId,
            price,
            tiers.Select(
                x => new PricingTierDto { LowerRange = x.LowerRange, UpperRange = x.UpperRange, Price = x.Price, }));

        var updatedCompetition = await context.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Price)
            .ThenInclude(x => x.Tiers)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        var competitionSolution = updatedCompetition.CompetitionSolutions.First(x => x.SolutionId == solution.CatalogueItemId);

        competitionSolution.Price.Should().NotBeNull();
        competitionSolution.Price.Tiers.Should().HaveCount(tiers.Count);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetSolutionPrice_ExistingPrice_SetsPrice(
        Organisation organisation,
        Competition competition,
        Solution solution,
        CataloguePrice oldPrice,
        List<CataloguePriceTier> oldTiers,
        CataloguePrice price,
        List<CataloguePriceTier> tiers,
        CompetitionCatalogueItemPrice competitionPrice,
        List<CompetitionCatalogueItemPriceTier> competitionPriceTiers,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsPriceService service)
    {
        tiers.ForEach(x =>
        {
            x.CataloguePrice = null;
        });

        oldPrice.CataloguePriceTiers = oldTiers;
        price.CataloguePriceTiers = tiers;
        solution.CatalogueItem.CataloguePrices = new List<CataloguePrice> { oldPrice, price, };

        competitionPrice.CataloguePriceId = oldPrice.CataloguePriceId;
        competitionPrice.Tiers = competitionPriceTiers;

        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions.Add(
            new CompetitionSolution(competition.Id, solution.CatalogueItemId)
            {
                IsShortlisted = true, Price = competitionPrice,
            });

        context.Solutions.Add(solution);
        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        await service.SetSolutionPrice(
            organisation.InternalIdentifier,
            competition.Id,
            solution.CatalogueItemId,
            price,
            tiers.Select(
                x => new PricingTierDto { LowerRange = x.LowerRange, UpperRange = x.UpperRange, Price = x.Price, }));

        var updatedCompetition = await context.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Price)
            .ThenInclude(x => x.Tiers)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        var competitionSolution = updatedCompetition.CompetitionSolutions.First(x => x.SolutionId == solution.CatalogueItemId);

        competitionSolution.Price.Should().NotBeNull();
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetServicePrice_NoExistingPrice_SetsPrice(
        Organisation organisation,
        Competition competition,
        Solution solution,
        AdditionalService additionalService,
        CataloguePrice price,
        List<CataloguePriceTier> tiers,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsPriceService service)
    {
        tiers.ForEach(x =>
        {
            x.CataloguePrice = null;
        });

        price.CataloguePriceTiers = tiers;

        additionalService.SolutionId = solution.CatalogueItemId;
        additionalService.CatalogueItem.CataloguePrices = new List<CataloguePrice> { price, };

        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions.Add(
            new CompetitionSolution(competition.Id, solution.CatalogueItemId)
            {
                IsShortlisted = true,
                SolutionServices = new List<SolutionService>
                {
                    new(competition.Id, solution.CatalogueItemId, additionalService.CatalogueItemId, true),
                },
            });

        context.AdditionalServices.Add(additionalService);
        context.Solutions.Add(solution);
        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        await service.SetServicePrice(
            organisation.InternalIdentifier,
            competition.Id,
            solution.CatalogueItemId,
            additionalService.CatalogueItemId,
            price,
            tiers.Select(
                x => new PricingTierDto { LowerRange = x.LowerRange, UpperRange = x.UpperRange, Price = x.Price, }));

        var updatedCompetition = await context.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices)
            .ThenInclude(x => x.Price)
            .ThenInclude(x => x.Tiers)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        var updatedSolution = updatedCompetition.CompetitionSolutions.First(x => x.SolutionId == solution.CatalogueItemId);

        var updatedService = updatedSolution.SolutionServices.First(x => x.ServiceId == additionalService.CatalogueItemId);

        updatedService.Price.Should().NotBeNull();
        updatedService.Price.Tiers.Should().HaveCount(tiers.Count);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetServicePrice_ExistingPrice_SetsPrice(
        Organisation organisation,
        Competition competition,
        Solution solution,
        AdditionalService additionalService,
        CataloguePrice oldPrice,
        List<CataloguePriceTier> oldTiers,
        CataloguePrice price,
        List<CataloguePriceTier> tiers,
        CompetitionCatalogueItemPrice competitionPrice,
        List<CompetitionCatalogueItemPriceTier> competitionPriceTiers,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsPriceService service)
    {
        tiers.ForEach(x =>
        {
            x.CataloguePrice = null;
        });

        oldPrice.CataloguePriceTiers = oldTiers;
        price.CataloguePriceTiers = tiers;

        additionalService.SolutionId = solution.CatalogueItemId;
        additionalService.CatalogueItem.CataloguePrices = new List<CataloguePrice> { oldPrice, price, };

        competitionPrice.CataloguePriceId = oldPrice.CataloguePriceId;
        competitionPrice.Tiers = competitionPriceTiers;

        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions.Add(
            new CompetitionSolution(competition.Id, solution.CatalogueItemId)
            {
                IsShortlisted = true,
                Price = competitionPrice,
                SolutionServices = new List<SolutionService>
                {
                    new(competition.Id, solution.CatalogueItemId, additionalService.CatalogueItemId, true),
                },
            });

        context.AdditionalServices.Add(additionalService);
        context.Solutions.Add(solution);
        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        await service.SetServicePrice(
            organisation.InternalIdentifier,
            competition.Id,
            solution.CatalogueItemId,
            additionalService.CatalogueItemId,
            price,
            tiers.Select(
                x => new PricingTierDto { LowerRange = x.LowerRange, UpperRange = x.UpperRange, Price = x.Price, }));

        var updatedCompetition = await context.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices)
            .ThenInclude(x => x.Price)
            .ThenInclude(x => x.Tiers)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        var updatedSolution = updatedCompetition.CompetitionSolutions.First(x => x.SolutionId == solution.CatalogueItemId);

        var updatedService = updatedSolution.SolutionServices.First(x => x.ServiceId == additionalService.CatalogueItemId);

        updatedService.Price.Should().NotBeNull();
        updatedService.Price.Tiers.Should().HaveCount(tiers.Count);
    }
}
