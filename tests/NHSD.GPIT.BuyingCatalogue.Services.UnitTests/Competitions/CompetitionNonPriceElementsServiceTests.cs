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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.Services.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Competitions;

public static class CompetitionNonPriceElementsServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionNonPriceElementsService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task DeleteNonPriceElement_RemovesNonPriceElement(
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CompetitionNonPriceElementsService service)
    {
        competition.NonPriceElements = new()
        {
            Implementation = new() { Requirements = "Requirements" },
            Interoperability =
                new List<InteroperabilityCriteria>
                {
                    new() { IntegrationType = InteropIntegrationType.Im1, Qualifier = "Test" },
                },
            ServiceLevel =
                new()
                {
                    ApplicableDays = "Mon-Fri",
                    TimeFrom = DateTime.UtcNow.AddHours(-1),
                    TimeUntil = DateTime.UtcNow,
                },
            NonPriceWeights = new() { Implementation = 50, ServiceLevel = 25, Interoperability = 25, },
        };

        competition.HasReviewedCriteria = true;
        competition.OrganisationId = organisation.Id;

        dbContext.Organisations.Add(organisation);
        dbContext.Competitions.Add(competition);

        await dbContext.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();

        await service.DeleteNonPriceElement(
            organisation.InternalIdentifier,
            competition.Id,
            NonPriceElement.Implementation);

        var updatedCompetition = await dbContext.Competitions
            .Include(x => x.NonPriceElements.NonPriceWeights)
            .Include(x => x.NonPriceElements.Implementation)
            .Include(x => x.NonPriceElements.Interoperability)
            .Include(x => x.NonPriceElements.ServiceLevel)
            .FirstOrDefaultAsync(x => x.OrganisationId == organisation.Id && x.Id == competition.Id);

        updatedCompetition.NonPriceElements.Implementation.Should().BeNull();
        updatedCompetition.NonPriceElements.NonPriceWeights.Should().BeNull();
        updatedCompetition.HasReviewedCriteria.Should().BeFalse();
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task DeleteNonPriceElement_SingleNonPriceElement_DeletesNonPriceElements(
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CompetitionNonPriceElementsService service)
    {
        competition.NonPriceElements = new()
        {
            Implementation = new() { Requirements = "Requirements" },
            NonPriceWeights = new() { Implementation = 100, },
        };

        competition.HasReviewedCriteria = true;
        competition.OrganisationId = organisation.Id;

        dbContext.Organisations.Add(organisation);
        dbContext.Competitions.Add(competition);

        await dbContext.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();

        await service.DeleteNonPriceElement(
            organisation.InternalIdentifier,
            competition.Id,
            NonPriceElement.Implementation);

        var updatedCompetition = await dbContext.Competitions
            .Include(x => x.NonPriceElements.NonPriceWeights)
            .FirstOrDefaultAsync(x => x.OrganisationId == organisation.Id && x.Id == competition.Id);

        updatedCompetition.NonPriceElements.Should().BeNull();
        updatedCompetition.HasReviewedCriteria.Should().BeFalse();
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task DeleteNonPriceElement_WithSolutionScores_DeletesSolutionScoresForNonPriceElement(
        Organisation organisation,
        Competition competition,
        Solution solution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CompetitionNonPriceElementsService service)
    {
        competition.NonPriceElements = new()
        {
            Implementation = new() { Requirements = "Requirements" },
            Interoperability =
                new List<InteroperabilityCriteria>
                {
                    new() { IntegrationType = InteropIntegrationType.Im1, Qualifier = "Test" },
                },
            NonPriceWeights = new() { Implementation = 50, Interoperability = 50, },
        };

        competition.CompetitionSolutions = new List<CompetitionSolution>
        {
            new(competition.Id, solution.CatalogueItemId)
            {
                IsShortlisted = true,
                Scores = new List<SolutionScore>
                {
                    new(ScoreType.Implementation, 5), new(ScoreType.Interoperability, 4),
                },
            },
        };

        competition.HasReviewedCriteria = true;
        competition.OrganisationId = organisation.Id;

        dbContext.Organisations.Add(organisation);
        dbContext.Competitions.Add(competition);
        dbContext.Solutions.Add(solution);

        await dbContext.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();

        await service.DeleteNonPriceElement(
            organisation.InternalIdentifier,
            competition.Id,
            NonPriceElement.Implementation);

        var updatedCompetition = await dbContext.Competitions
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Scores)
            .FirstOrDefaultAsync(x => x.OrganisationId == organisation.Id && x.Id == competition.Id);

        updatedCompetition.CompetitionSolutions.Should().NotContain(x => x.HasScoreType(ScoreType.Implementation));
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task AddFeatureRequirement_AddsRequirement(
        Organisation organisation,
        Competition competition,
        string requirement,
        CompliancyLevel compliance,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CompetitionNonPriceElementsService service)
    {
        competition.OrganisationId = organisation.Id;

        dbContext.Add(organisation);
        dbContext.Add(competition);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        await service.AddFeatureRequirement(organisation.InternalIdentifier, competition.Id, requirement, compliance);

        var updatedCompetition = await dbContext.Competitions.Include(x => x.NonPriceElements.Features)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.NonPriceElements.Features.Should()
            .ContainSingle(x => x.Requirements == requirement && x.Compliance == compliance);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task EditFeatureRequirement_EditsRequirement(
        Organisation organisation,
        Competition competition,
        string requirement,
        CompliancyLevel compliance,
        string newRequirement,
        CompliancyLevel newCompliance,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CompetitionNonPriceElementsService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.NonPriceElements = new() { Features = new List<FeaturesCriteria> { new(requirement, compliance) } };

        dbContext.Add(organisation);
        dbContext.Add(competition);

        await dbContext.SaveChangesAsync();

        var featureCriteriaId = competition.NonPriceElements.Features.FirstOrDefault()?.Id;

        featureCriteriaId.Should().NotBeNull();

        dbContext.ChangeTracker.Clear();

        await service.EditFeatureRequirement(organisation.InternalIdentifier, competition.Id, featureCriteriaId!.Value, newRequirement, newCompliance);

        var updatedCompetition = await dbContext.Competitions.Include(x => x.NonPriceElements.Features)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.NonPriceElements.Features.Should()
            .ContainSingle(x => x.Requirements == newRequirement && x.Compliance == newCompliance);
    }
}
