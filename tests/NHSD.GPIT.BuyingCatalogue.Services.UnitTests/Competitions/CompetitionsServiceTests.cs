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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Services.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;
using EntityOdsOrganisation = NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models.OdsOrganisation;
using ServiceContractOdsOrganisation = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations.OdsOrganisation;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Competitions;

public static class CompetitionsServiceTests
{
    private const int CommonCompetitionId = 34;
    private const int CommonOrganisationId = 21;
    private const string CommonOrganisationInternalIdentifier = "BB-FFGG";
    private const string CommonOrganisationExternalIdentifier = "FFGG";

    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionsService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetCompetitionCriteriaReview_ReturnsCompetition(
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.FrameworkId = competition.Framework.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetCompetitionCriteriaReview(organisation.InternalIdentifier, competition.Id);

        result.Should().BeEquivalentTo(
            competition,
            opt => opt.Excluding(x => x.Organisation).Excluding(x => x.Framework));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetCompetitions_ReturnsCompetitions(
        Organisation organisation,
        List<Competition> competitions,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competitions.ForEach(
            x =>
            {
                x.OrganisationId = organisation.Id;
                x.FrameworkId = x.Framework.Id;
                x.Organisation = organisation;
                x.IsDeleted = false;
            });

        context.Organisations.Add(organisation);
        context.Competitions.AddRange(competitions);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var result = await service.GetCompetitions(organisation.InternalIdentifier);

        result.Should()
            .BeEquivalentTo(
                competitions,
                opt => opt
                    .Excluding(x => x.Organisation)
                    .Excluding(x => x.Framework)
                    .Excluding(x => x.LastUpdatedByUser)
                    .Excluding(x => x.Filter));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetCompetitionWithSublocations_ReturnsCompetition(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.Organisation = organisation;

        competitionSublocations.ForEach(
            x =>
            {
                x.OwnerOdsCode = organisation.ExternalIdentifier;
                x.CompetitionId = competition.Id;
                x.SublocationOrganisation.Id = x.SublocationOdsCode;
            });

        competition.CompetitionSublocations = competitionSublocations;

        context.Add(competition);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        Competition result = await service.GetCompetitionWithSublocations(
            organisation.InternalIdentifier,
            competition.Id);

        result.Should()
            .BeEquivalentTo(
                competition,
                opt => opt.Excluding(m => m.Framework)
                    .Excluding(m => m.CompetitionSublocations)
                    .Excluding(m => m.FlattenedRecipients));
        result.CompetitionSublocations.Should()
            .BeEquivalentTo(
                competition.CompetitionSublocations,
                opt => opt.Excluding(m => m.Competition).Excluding(m => m.SublocationRecipients));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetCompetitionsWithSublocationsAndSublocationRecipients_ReturnsCompetition(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.Organisation = organisation;

        competitionSublocations.ForEach(
            x =>
            {
                x.OwnerOdsCode = organisation.ExternalIdentifier;
                x.CompetitionId = competition.Id;
                x.SublocationOrganisation.Id = x.SublocationOdsCode;
                foreach (CompetitionSublocationRecipient competitionSublocationRecipient in x.SublocationRecipients)
                {
                    competitionSublocationRecipient.RecipientOrganisation.Id =
                        competitionSublocationRecipient.RecipientOdsCode;
                }
            });

        competition.CompetitionSublocations = competitionSublocations;

        context.Add(competition);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        Competition result = await service.GetCompetitionWithSublocationsAndSublocationRecipients(
            organisation.InternalIdentifier,
            competition.Id);

        result.Should()
            .BeEquivalentTo(
                competition,
                opt => opt.Excluding(m => m.Framework)
                    .Excluding(m => m.CompetitionSublocations)
                    .Excluding(m => m.FlattenedRecipients));
        result.CompetitionSublocations.Should()
            .BeEquivalentTo(
                competition.CompetitionSublocations,
                opt => opt.Excluding(m => m.Competition)
                    .Excluding(m => m.SublocationOrganisation)
                    .Excluding(m => m.SublocationRecipients));

        foreach (CompetitionSublocation expectedCompetitionSublocation in competition.CompetitionSublocations)
        {
            ICollection<CompetitionSublocationRecipient> actualSublocationRecipients = result.CompetitionSublocations
                .First(
                    x => x.SublocationOdsCode == expectedCompetitionSublocation.SublocationOdsCode)
                .SublocationRecipients;

            expectedCompetitionSublocation.SublocationRecipients.Should()
                .BeEquivalentTo(
                    actualSublocationRecipients,
                    opt => opt.Excluding(m => m.Competition).Excluding(m => m.ParentSublocation));
        }
    }

    public static IEnumerable<object[]> CompetitionSublocationFlattenedRecipients()
    {
        return
        [
            [
                new List<CompetitionSublocation>
                {
                    CommonCompetitionSublocationFactory(
                        "XXXX",
                        [
                            CommonCompetitionSublocationRecipientFactory("AAAA", "XXXX", true),
                            CommonCompetitionSublocationRecipientFactory("AAAB", "XXXX", true),
                        ],
                        true),
                    CommonCompetitionSublocationFactory(
                        "XXXA",
                        [CommonCompetitionSublocationRecipientFactory("BAAA", "XXXA", true)],
                        true),
                },
                new List<CompetitionSublocationRecipient>
                {
                    CommonCompetitionSublocationRecipientFactory("AAAA", "XXXX", true),
                    CommonCompetitionSublocationRecipientFactory("AAAB", "XXXX", true),
                    CommonCompetitionSublocationRecipientFactory("BAAA", "XXXA", true),
                },
            ],
        ];
    }

    [Theory]
    [MockInMemoryDbMemberAutoData(nameof(CompetitionSublocationFlattenedRecipients))]
    public static async Task
        GetCompetitionWithSublocationsAndSublocationRecipients_FlattenedRecipientsPropertyPopulated(
            List<CompetitionSublocation> competitionSublocations,
            List<CompetitionSublocationRecipient> expectedCompetitionSublocationRecipients,
            Competition competition,
            Organisation organisation,
            [Frozen] BuyingCatalogueDbContext context,
            CompetitionsService service)
    {
        competition.Organisation = organisation;

        competitionSublocations.ForEach(x =>
        {
            x.OwnerOdsCode = organisation.ExternalIdentifier;
            x.CompetitionId = competition.Id;
        });

        competition.CompetitionSublocations = competitionSublocations;

        context.Add(competition);

        await context.SaveChangesAsync();

        expectedCompetitionSublocationRecipients.ForEach(x => x.CompetitionId = competition.Id);

        context.ChangeTracker.Clear();

        Competition result = await service.GetCompetitionWithSublocationsAndSublocationRecipients(
            organisation.InternalIdentifier,
            competition.Id);

        result.FlattenedRecipients.Should()
            .BeEquivalentTo(expectedCompetitionSublocationRecipients, opt => opt.Excluding(m => m.ParentSublocation));
    }

    [Theory]
    [MockInMemoryDbInlineAutoData(false, false)]
    [MockInMemoryDbInlineAutoData(true, true)]
    public static async Task GetCompetitionHasAnySublocations_ReturnsBool(
        bool addSublocation,
        bool expectedResult,
        Competition competition,
        Organisation organisation,
        CompetitionSublocation competitionSublocation,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.Organisation = organisation;

        if (addSublocation)
        {
            competition.CompetitionSublocations.Add(competitionSublocation);
        }

        context.Add(competition);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var result = await service.GetCompetitionHasAnySublocations(organisation.InternalIdentifier, competition.Id);

        Assert.Equal(expectedResult, result);
    }

    public static IEnumerable<object[]> CompetitionCountData()
    {
        return
        [
            [
                new List<CompetitionSublocation>
                {
                    CommonCompetitionSublocationFactory(
                        "XXXX",
                        [
                            CommonCompetitionSublocationRecipientFactory("AAAA", "XXXX"),
                            CommonCompetitionSublocationRecipientFactory("AAAB", "XXXX"),
                            CommonCompetitionSublocationRecipientFactory("AAAC", "XXXX"),
                        ]),
                    CommonCompetitionSublocationFactory(
                        "XXXY",
                        [
                            CommonCompetitionSublocationRecipientFactory("BAAA", "XXXY"),
                            CommonCompetitionSublocationRecipientFactory("BAAB", "XXXY"),
                            CommonCompetitionSublocationRecipientFactory("BAAC", "XXXY"),
                        ]),
                    CommonCompetitionSublocationFactory(
                        "XXXZ",
                        [
                            CommonCompetitionSublocationRecipientFactory("CAAA", "XXXZ"),
                            CommonCompetitionSublocationRecipientFactory("CAAB", "XXXZ"),
                        ]),
                },
                8,
            ],
        ];
    }

    [Theory]
    [MockInMemoryDbMemberAutoData(nameof(CompetitionCountData))]
    public static async Task GetCompetitionTotalRecipientCount_ReturnsInt(
        List<CompetitionSublocation> competitionSublocations,
        int expectedRecipientCount,
        Competition competition,
        Organisation organisation,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.Organisation = organisation;
        competition.CompetitionSublocations = competitionSublocations;

        context.Add(competition);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var result = await service.GetCompetitionTotalRecipientCount(organisation.InternalIdentifier, competition.Id);

        Assert.Equal(expectedRecipientCount, result);
    }

    [Theory]
    [MockInMemoryDbInlineAutoData(0)]
    [MockInMemoryDbInlineAutoData(1)]
    public static async Task GetPagedCompetitions_ReturnsExpectedPageSize(
        int page,
        Organisation organisation,
        List<Competition> competitions,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competitions.ForEach(
            x =>
            {
                x.OrganisationId = organisation.Id;
                x.Organisation = organisation;
                x.IsDeleted = false;
            });

        context.Organisations.Add(organisation);
        context.Competitions.AddRange(competitions);

        await context.SaveChangesAsync();

        var pagedCompetitions = await service.GetPagedCompetitions(organisation.InternalIdentifier, new PageOptions(page.ToString(), 2));

        pagedCompetitions.Items.Count.Should().Be(2);
        pagedCompetitions.Options.TotalNumberOfItems.Should().Be(competitions.Count);

        var expected = (int)Math.Ceiling((double)competitions.Count / pagedCompetitions.Options.PageSize);

        pagedCompetitions.Options.NumberOfPages.Should().Be(expected);
    }

    [Theory]
    [MockInMemoryDbInlineAutoData(null)]
    [MockInMemoryDbInlineAutoData("")]
    [MockInMemoryDbInlineAutoData("  ")]
    public static async Task AddCompetition_Requires_FrameworkId(
        string frameworkId,
        Organisation organisation,
        Filter filter,
        string name,
        string description,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        filter.Organisation = null;
        filter.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Filters.Add(filter);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.Awaiting(s => s.AddCompetition(organisation.Id, filter.Id, frameworkId, name, description))
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task AddCompetition_Requires_Existing_FrameworkId(
        string frameworkId,
        Organisation organisation,
        Filter filter,
        string name,
        string description,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        filter.Organisation = null;
        filter.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Filters.Add(filter);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.Awaiting(s => s.AddCompetition(organisation.Id, filter.Id, frameworkId, name, description))
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task AddCompetition_Requires_Active_Framework(
        Organisation organisation,
        Filter filter,
        EntityFramework.Catalogue.Models.Framework framework,
        string name,
        string description,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        framework.IsExpired = true;

        filter.Organisation = null;
        filter.OrganisationId = organisation.Id;

        context.Frameworks.Add(framework);
        context.Organisations.Add(organisation);
        context.Filters.Add(filter);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.Awaiting(s => s.AddCompetition(organisation.Id, filter.Id, framework.Id, name, description))
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task AddCompetition_AddsCompetition(
        Organisation organisation,
        Filter filter,
        EntityFramework.Catalogue.Models.Framework framework,
        string name,
        string description,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        framework.IsExpired = false;

        filter.Organisation = null;
        filter.OrganisationId = organisation.Id;

        context.Frameworks.Add(framework);
        context.Organisations.Add(organisation);
        context.Filters.Add(filter);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.AddCompetition(organisation.Id, filter.Id, framework.Id, name, description);

        context.Competitions.Should()
            .Contain(
                x => x.Name == name && x.Description == description && x.OrganisationId == organisation.Id
                    && x.FilterId == filter.Id && x.FrameworkId == framework.Id);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetCompetitionWithServices_ReturnsCompetition(
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.FrameworkId = competition.Framework.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetCompetitionWithServices(organisation.InternalIdentifier, competition.Id);

        result.Should().BeEquivalentTo(competition, opt => opt.Excluding(x => x.Organisation).Excluding(x => x.Framework));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetCompetitionWithServicesAndFramework_ReturnsCompetition(
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.FrameworkId = competition.Framework.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetCompetitionWithServicesAndFramework(organisation.InternalIdentifier, competition.Id);

        result.Should().BeEquivalentTo(competition, opt => opt.Excluding(x => x.Organisation).Excluding(x => x.Framework));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetCompetitionForResults_ReturnsCompetition(
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.FrameworkId = competition.Framework.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetCompetitionForResults(organisation.InternalIdentifier, competition.Id);

        result.Should().BeEquivalentTo(
            competition,
            opt => opt.Excluding(x => x.Organisation).Excluding(x => x.Framework));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetCompetition_ReturnsExpected(
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.FrameworkId = competition.Framework.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetCompetition(organisation.InternalIdentifier, competition.Id);

        result.Should().BeEquivalentTo(
            competition,
            opt => opt.Excluding(x => x.Organisation).Excluding(x => x.Framework));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetCompetitionWithFramework_ReturnsExpected(
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.FrameworkId = competition.Framework.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetCompetitionWithFramework(organisation.InternalIdentifier, competition.Id);

        result.Should().BeEquivalentTo(
            competition,
            opt => opt.Excluding(x => x.Organisation));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task AddCompetitionSolutions_Adds(
        Organisation organisation,
        Competition competition,
        List<Solution> solutions,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.Solutions.AddRange(solutions);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var competitionSolutions = solutions.Select(x => new CompetitionSolution(competition.Id, x.CatalogueItemId)).ToList();

        await service.AddCompetitionSolutions(organisation.InternalIdentifier, competition.Id, competitionSolutions);

        var updatedCompetition = await context.Competitions.AsNoTracking()
            .IgnoreQueryFilters()
            .Include(x => x.CompetitionSolutions)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.CompetitionSolutions.Select(x => x.CatalogueItemId)
            .Should()
            .BeEquivalentTo(competitionSolutions.Select(x => x.CatalogueItemId));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetShortlistedSolutions_ShortlistsSolutions(
        Organisation organisation,
        Competition competition,
        List<Solution> solutions,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions =
            solutions.Select(x => new CompetitionSolution(competition.Id, x.CatalogueItemId)).ToList();

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.Solutions.AddRange(solutions);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var shortlisted = solutions.Take(1).Select(x => x.CatalogueItemId).ToList();
        var nonShortlisted = solutions.Skip(1).Select(x => x.CatalogueItemId).ToList();

        await service.SetShortlistedSolutions(organisation.InternalIdentifier, competition.Id, shortlisted);

        var updatedCompetition = await context.Competitions.AsNoTracking()
            .IgnoreQueryFilters()
            .Include(x => x.CompetitionSolutions)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        var actualShortlistedSolutions = updatedCompetition.CompetitionSolutions.Where(x => x.IsShortlisted)
            .Select(x => x.CatalogueItemId);
        var actualNonShortlistedSolutions = updatedCompetition.CompetitionSolutions.Where(x => !x.IsShortlisted)
            .Select(x => x.CatalogueItemId);

        actualShortlistedSolutions.Should().BeEquivalentTo(shortlisted);
        actualNonShortlistedSolutions.Should().BeEquivalentTo(nonShortlisted);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetShortlistedSolutions_WithPreviousSelection_ErasesPreviousSelection(
        Organisation organisation,
        Competition competition,
        List<Solution> solutions,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        var previouslyShortlistedSolution = solutions.First().CatalogueItemId;
        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions =
            solutions.Skip(1).Select(x => new CompetitionSolution(competition.Id, x.CatalogueItemId)).ToList();

        competition.CompetitionSolutions.Add(
            new CompetitionSolution(competition.Id, previouslyShortlistedSolution) { IsShortlisted = true });

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.Solutions.AddRange(solutions);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var shortlisted = solutions.Skip(1).Select(x => x.CatalogueItemId).ToList();

        await service.SetShortlistedSolutions(organisation.InternalIdentifier, competition.Id, shortlisted);

        var updatedCompetition = await context.Competitions.AsNoTracking()
            .IgnoreQueryFilters()
            .Include(x => x.CompetitionSolutions)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        var actualShortlistedSolutions = updatedCompetition.CompetitionSolutions.Where(x => x.IsShortlisted)
            .Select(x => x.CatalogueItemId);
        var actualNonShortlistedSolutions = updatedCompetition.CompetitionSolutions.Where(x => !x.IsShortlisted)
            .Select(x => x.CatalogueItemId);

        actualShortlistedSolutions.Should().BeEquivalentTo(shortlisted);
        actualNonShortlistedSolutions.Should().Contain(x => x == previouslyShortlistedSolution);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetSolutionJustifications_SetsJustifications(
        Organisation organisation,
        Competition competition,
        Dictionary<Solution, string> justifications,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        var solutions = justifications.Keys.ToList();
        var solutionIdsJustifications = justifications.ToDictionary(x => x.Key.CatalogueItemId, x => x.Value);

        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions =
            solutions.Select(x => new CompetitionSolution(competition.Id, x.CatalogueItemId)).ToList();

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.Solutions.AddRange(solutions);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.SetSolutionJustifications(
            organisation.InternalIdentifier,
            competition.Id,
            solutionIdsJustifications);

        var updatedCompetition = await context.Competitions.AsNoTracking()
            .IgnoreQueryFilters()
            .Include(x => x.CompetitionSolutions)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.CompetitionSolutions.Should()
            .Contain(x => x.Justification == solutionIdsJustifications[x.CatalogueItemId]);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetSolutionJustifications_PreviousJustifications_ErasesPreviousJustifications(
        string previousJustification,
        Organisation organisation,
        Competition competition,
        Dictionary<Solution, string> justifications,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        var previouslyJustified = justifications.Keys.First();
        var solutions = justifications.Keys.Skip(1).ToList();
        var solutionIdsJustifications = justifications.Skip(1).ToDictionary(x => x.Key.CatalogueItemId, x => x.Value);

        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions =
            solutions.Select(x => new CompetitionSolution(competition.Id, x.CatalogueItemId)).ToList();

        competition.CompetitionSolutions.Add(new CompetitionSolution(competition.Id, previouslyJustified.CatalogueItemId)
        {
            Justification = previousJustification,
        });

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.Solutions.AddRange(solutions);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.SetSolutionJustifications(
            organisation.InternalIdentifier,
            competition.Id,
            solutionIdsJustifications);

        var updatedCompetition = await context.Competitions.AsNoTracking()
            .IgnoreQueryFilters()
            .Include(x => x.CompetitionSolutions)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.CompetitionSolutions.Should()
            .Contain(x => x.CatalogueItemId == previouslyJustified.CatalogueItemId && string.IsNullOrEmpty(x.Justification));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task AcceptShortlist_SetsAcceptanceDate(
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.ShortlistAccepted = null;
        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.AcceptShortlist(organisation.InternalIdentifier, competition.Id);

        var updatedCompetition =
            await context.Competitions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.ShortlistAccepted.Should().NotBeNull();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task AcceptShortlist_PreviouslyAccepted_DoesNothing(
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        var acceptedDate = DateTime.UtcNow;

        competition.ShortlistAccepted = acceptedDate;
        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.AcceptShortlist(organisation.InternalIdentifier, competition.Id);

        var refreshedCompetition =
            await context.Competitions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == competition.Id);

        refreshedCompetition.ShortlistAccepted.Should().Be(acceptedDate);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task CompleteCompetition_SetsCompletionDate(
        Organisation organisation,
        Competition competition,
        CompetitionCatalogueItemPrice price,
        CompetitionCatalogueItemPriceTier tier,
        Solution solution,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        price.Tiers = new List<CompetitionCatalogueItemPriceTier> { tier };

        competition.Completed = null;
        competition.OrganisationId = organisation.Id;
        competition.Weightings = new() { Price = 70, NonPrice = 30 };
        competition.CompetitionSolutions = new List<CompetitionSolution>
        {
            new(competition.Id, solution.CatalogueItemId) { Quantity = 500, Price = price, IsShortlisted = true },
        };

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.Solutions.Add(solution);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.CompleteCompetition(organisation.InternalIdentifier, competition.Id);

        var updatedCompetition =
            await context.Competitions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.Completed.Should().NotBeNull();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task CompleteCompetition_DirectAward_SetsCompletionDate(
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.Completed = null;
        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.CompleteCompetition(organisation.InternalIdentifier, competition.Id, true);

        var updatedCompetition =
            await context.Competitions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.Completed.Should().NotBeNull();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task CompleteCompetition_PreviouslyCompleted_DoesNothing(
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        var completedDate = DateTime.UtcNow;

        competition.Completed = completedDate;
        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.CompleteCompetition(organisation.InternalIdentifier, competition.Id);

        var refreshedCompetition =
            await context.Competitions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == competition.Id);

        refreshedCompetition.Completed.Should().Be(completedDate);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task DeleteCompetition_SetsDeleted(
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.IsDeleted = false;
        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.DeleteCompetition(organisation.InternalIdentifier, competition.Id);

        var updatedCompetition =
            await context.Competitions.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.IsDeleted.Should().BeTrue();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task DeleteCompetition_PreviouslyDeleted_DoesNothing(
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.IsDeleted = true;
        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.DeleteCompetition(organisation.InternalIdentifier, competition.Id);

        var refreshedCompetition =
            await context.Competitions.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(x => x.Id == competition.Id);

        refreshedCompetition.IsDeleted.Should().BeTrue();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task Exists_ReturnsExpected(
        Organisation organisation,
        Filter filter,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.IsDeleted = false;
        competition.OrganisationId = organisation.Id;
        competition.FilterId = filter.Id;

        filter.Organisation = null;
        filter.OrganisationId = organisation.Id;

        context.Competitions.Add(competition);
        context.Organisations.Add(organisation);
        context.Filters.Add(filter);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.Exists(organisation.InternalIdentifier, competition.Name);

        result.Should().BeTrue();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetCompetitionTaskList_ReturnsExpected(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.CompetitionSublocations = competitionSublocations;

        context.Competitions.Add(competition);
        context.Organisations.Add(organisation);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var expectedModel = new CompetitionTaskListModel(competition);

        CompetitionTaskListModel result = await service.GetCompetitionTaskList(
            organisation.InternalIdentifier,
            competition.Id);

        result.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetCompetitionName_ReturnsExpected(
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;

        context.Competitions.Add(competition);
        context.Organisations.Add(organisation);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var result = await service.GetCompetitionName(organisation.InternalIdentifier, competition.Id);

        result.Should().Be(competition.Name);
    }

    [Theory]
    [MockInMemoryDbInlineAutoData("", 5, true, typeof(ArgumentException))]
    [MockInMemoryDbInlineAutoData(null, 5, true, typeof(ArgumentNullException))]
    [MockInMemoryDbInlineAutoData("MY-ORG-ID", 5, false, typeof(ArgumentException))]
    public static async Task SetSublocations_RejectsNullParams(
        string internalOrgId,
        int competitionId,
        bool populateSublocations,
        Type expectedExceptionType,
        HashSet<string> competitionSublocations,
        CompetitionsService service)
    {
        Exception exception = await Record.ExceptionAsync(
            async () =>
            {
                if (!populateSublocations)
                {
                    await service.SetSublocations(
                        internalOrgId,
                        competitionId,
                        []);
                }
                else
                {
                    await service.SetSublocations(
                        internalOrgId,
                        competitionId,
                        competitionSublocations);
                }
            });

        exception.Should().NotBeNull();
        exception!.GetType().Should().Be(expectedExceptionType);
    }

    public static IEnumerable<object[]> SetSublocationsNotValidData()
    {
        Competition completedCompetition = CommonCompetitionFactory(67, 21);
        completedCompetition.Completed = new DateTime(2024, 05, 03);

        var addHashSet = new HashSet<string> { "XXXX", "XXXY", "XXXZ" };

        return
        [
            [
                CommonOrganisationFactory(21), completedCompetition,
                addHashSet,
                "Cannot set sublocations on a completed competition.",
            ],
            [
                CommonOrganisationFactory(78), CommonCompetitionFactory(33, 78),
                new HashSet<string> { "FFGH" },
                "One or more requested Ids not found or not valid for this organisation.",
            ],
        ];
    }

    [Theory]
    [MockInMemoryDbMemberAutoData(nameof(SetSublocationsNotValidData))]
    public static async Task SetSublocations_RejectsInvalidOperations(
        Organisation organisation,
        Competition competition,
        HashSet<string> sublocationOdsCodes,
        string expectedMessage,
        [Frozen] BuyingCatalogueDbContext context,
        [Frozen] IOdsService odsService,
        CompetitionsService service)
    {
        competition.Organisation = organisation;

        context.Add(competition);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        odsService.GetSublocationsByParentOdsCode(organisation.ExternalIdentifier).Returns([]);

        Exception exception = await Record.ExceptionAsync(
            async () =>
            {
                await service.SetSublocations(
                    organisation.InternalIdentifier,
                    competition.Id,
                    sublocationOdsCodes);
            });

        exception.Should().NotBeNull();
        exception!.GetType().Should().Be(typeof(InvalidOperationException));
        exception!.Message.Should().Be(expectedMessage);
    }

    public static IEnumerable<object[]> SetSublocationsData()
    {
        return
        [
            // Adds
            [
                CommonOrganisationFactory(78), CommonCompetitionFactory(33, 78),
                new List<EntityOdsOrganisation>
                {
                    CommonEntityOdsOrganisationFactory("XXXX"),
                    CommonEntityOdsOrganisationFactory("XXXY"),
                    CommonEntityOdsOrganisationFactory("XXXZ"),
                },
                new List<ServiceContractOdsOrganisation>
                {
                    CommonServiceContractOdsOrganisationFactory("XXXX"),
                    CommonServiceContractOdsOrganisationFactory("XXXY"),
                    CommonServiceContractOdsOrganisationFactory("XXXZ"),
                },
                new HashSet<string> { "XXXX", "XXXY" },
                new List<CompetitionSublocation>
                {
                    CommonCompetitionSublocationFactory("XXXX", null, false, 33),
                    CommonCompetitionSublocationFactory("XXXY", null, false, 33),
                },
            ],

            // Removes
            [
                CommonOrganisationFactory(31),
                CommonCompetitionFactory(
                    64,
                    31,
                    [
                        CommonCompetitionSublocationFactory("YXXA"), CommonCompetitionSublocationFactory("YXXB"),
                        CommonCompetitionSublocationFactory("YXXC"),
                    ]),
                new List<EntityOdsOrganisation>
                {
                    CommonEntityOdsOrganisationFactory("YXXA"),
                    CommonEntityOdsOrganisationFactory("YXXB"),
                    CommonEntityOdsOrganisationFactory("YXXC"),
                },
                new List<ServiceContractOdsOrganisation>
                {
                    CommonServiceContractOdsOrganisationFactory("YXXA"),
                    CommonServiceContractOdsOrganisationFactory("YXXB"),
                    CommonServiceContractOdsOrganisationFactory("YXXC"),
                },
                new HashSet<string> { "YXXB", "YXXC" },
                new List<CompetitionSublocation>
                {
                    CommonCompetitionSublocationFactory("YXXB", null, false, 64),
                    CommonCompetitionSublocationFactory("YXXC", null, false, 64),
                },
            ],

            // Adds and removes
            [
                CommonOrganisationFactory(87),
                CommonCompetitionFactory(
                    55,
                    87,
                    [
                        CommonCompetitionSublocationFactory("ZXXA"), CommonCompetitionSublocationFactory("ZXXB"),
                        CommonCompetitionSublocationFactory("ZXXC"),
                    ]),
                new List<EntityOdsOrganisation>
                {
                    CommonEntityOdsOrganisationFactory("ZXXA"),
                    CommonEntityOdsOrganisationFactory("ZXXB"),
                    CommonEntityOdsOrganisationFactory("ZXXC"),
                    CommonEntityOdsOrganisationFactory("ZXXD"),
                    CommonEntityOdsOrganisationFactory("ZXXE"),
                    CommonEntityOdsOrganisationFactory("ZXXF"),
                },
                new List<ServiceContractOdsOrganisation>
                {
                    CommonServiceContractOdsOrganisationFactory("ZXXA"),
                    CommonServiceContractOdsOrganisationFactory("ZXXB"),
                    CommonServiceContractOdsOrganisationFactory("ZXXC"),
                    CommonServiceContractOdsOrganisationFactory("ZXXD"),
                    CommonServiceContractOdsOrganisationFactory("ZXXE"),
                    CommonServiceContractOdsOrganisationFactory("ZXXF"),
                },
                new HashSet<string> { "ZXXB", "ZXXC", "ZXXD", "ZXXE" },
                new List<CompetitionSublocation>
                {
                    CommonCompetitionSublocationFactory("ZXXB", null, false, 55),
                    CommonCompetitionSublocationFactory("ZXXC", null, false, 55),
                    CommonCompetitionSublocationFactory("ZXXD", null, false, 55),
                    CommonCompetitionSublocationFactory("ZXXE", null, false, 55),
                },
            ],
        ];
    }

    [Theory]
    [MockInMemoryDbMemberAutoData(nameof(SetSublocationsData))]
    public static async Task SetSublocations_SetsSublocations(
        Organisation organisation,
        Competition competition,
        List<EntityOdsOrganisation> validSublocationsAsEntityModels,
        List<ServiceContractOdsOrganisation> validSublocationsAsServiceModels,
        HashSet<string> setSublocationOdsCodes,
        List<CompetitionSublocation> expectedCompetitionSublocations,
        [Frozen] BuyingCatalogueDbContext context,
        [Frozen] IOdsService odsService,
        CompetitionsService service)
    {
        competition.Organisation = organisation;

        context.AddRange(validSublocationsAsEntityModels);
        context.Add(organisation);
        context.Add(competition);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        odsService.GetSublocationsByParentOdsCode(organisation.ExternalIdentifier)
            .Returns(validSublocationsAsServiceModels);

        await service.SetSublocations(
            organisation.InternalIdentifier,
            competition.Id,
            setSublocationOdsCodes);

        Competition actualCompetition = await service.GetCompetitionWithSublocations(
            organisation.InternalIdentifier,
            competition.Id);

        actualCompetition.CompetitionSublocations.Should()
            .BeEquivalentTo(
                expectedCompetitionSublocations,
                opt => opt.WithoutStrictOrdering()
                    .Excluding(m => m.Competition)
                    .Excluding(m => m.SublocationOrganisation)
                    .Excluding(m => m.SublocationRecipients));
    }

    [Theory]
    [MockInMemoryDbInlineAutoData("", 5, true, typeof(ArgumentException))]
    [MockInMemoryDbInlineAutoData(null, 5, true, typeof(ArgumentNullException))]
    [MockInMemoryDbInlineAutoData("MY-ORG-ID", 5, false, typeof(ArgumentException))]
    public static async Task SetCompetitionSublocationsAndRecipients_RejectsNullParams(
        string internalOrgId,
        int competitionId,
        bool populateSublocations,
        Type expectedExceptionType,
        ICollection<CompetitionSublocation> competitionSublocations,
        CompetitionsService service)
    {
        Exception exception = await Record.ExceptionAsync(
            async () =>
            {
                if (!populateSublocations)
                {
                    await service.SetCompetitionSublocationsAndRecipients(
                        internalOrgId,
                        competitionId,
                        new List<CompetitionSublocation>());
                }
                else
                {
                    await service.SetCompetitionSublocationsAndRecipients(
                        internalOrgId,
                        competitionId,
                        competitionSublocations);
                }
            });

        exception.Should().NotBeNull();
        exception!.GetType().Should().Be(expectedExceptionType);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetCompetitionSublocationsAndRecipients_RejectsInvalidOperations_CompetitionComplete(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.Organisation = organisation;
        competition.CompetitionSublocations = competitionSublocations;

        competition.Completed = new DateTime(2023, 02, 01);

        context.Add(competition);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        Exception exception = await Record.ExceptionAsync(
            async () =>
            {
                await service.SetCompetitionSublocationsAndRecipients(
                    organisation.InternalIdentifier,
                    competition.Id,
                    competitionSublocations);
            });

        exception.Should().NotBeNull();
        exception!.GetType().Should().Be(typeof(InvalidOperationException));
        exception!.Message.Should().Be("Cannot set sublocations / recipients on a completed competition.");
    }

    public static IEnumerable<object[]> CompetitionSublocationNotValidData()
    {
        Organisation organisation = CommonOrganisationFactory();
        Competition competition = CommonCompetitionFactory();

        competition.CompetitionSublocations =
            [CommonCompetitionSublocationFactory("XXXX"), CommonCompetitionSublocationFactory("XXXA")];

        return
        [
            [
                organisation, competition,
                CommonEntityOdsOrganisationFactory(CommonOrganisationExternalIdentifier),
                new List<EntityOdsOrganisation>
                {
                    CommonEntityOdsOrganisationFactory("XXXX"), CommonEntityOdsOrganisationFactory("XXXA"),
                },
                new List<EntityOdsOrganisation>
                {
                    CommonEntityOdsOrganisationFactory("AAAA"),
                    CommonEntityOdsOrganisationFactory("AAAB"),
                    CommonEntityOdsOrganisationFactory("BAAA"),
                    CommonEntityOdsOrganisationFactory("BAAB"),
                },
                new List<ServiceContractOdsOrganisation>
                {
                    CommonServiceContractOdsOrganisationFactory("ZZZZ"),
                    CommonServiceContractOdsOrganisationFactory("XXXA"),
                },
                new List<CompetitionSublocation>
                {
                    CommonCompetitionSublocationFactory(
                        "XXXX",
                        [
                            CommonCompetitionSublocationRecipientFactory("AAAA", "XXXX"),
                            CommonCompetitionSublocationRecipientFactory("AAAB", "XXXX"),
                        ]),
                    CommonCompetitionSublocationFactory(
                        "XXXA",
                        [
                            CommonCompetitionSublocationRecipientFactory("BAAA", "XXXA"),
                            CommonCompetitionSublocationRecipientFactory("BAAB", "XXXA"),
                        ]),
                },
            ],
        ];
    }

    [Theory]
    [MockInMemoryDbMemberAutoData(nameof(CompetitionSublocationNotValidData))]
    public static async Task SetCompetitionSublocationsAndRecipients_RejectsInvalidOperations_SublocationsNotValid(
        Organisation organisation,
        Competition competition,
        EntityOdsOrganisation ownerOdsOrganisation,
        List<EntityOdsOrganisation> sublocationOdsOrganisationsForDb,
        List<EntityOdsOrganisation> recipientOdsOrganisationsForDb,
        List<ServiceContractOdsOrganisation> validSublocations,
        List<CompetitionSublocation> competitionSublocationsToSubmit,
        [Frozen] BuyingCatalogueDbContext context,
        [Frozen] IOdsService odsService,
        CompetitionsService service)
    {
        competition.Organisation = organisation;

        context.Add(ownerOdsOrganisation);
        context.AddRange(sublocationOdsOrganisationsForDb);
        context.AddRange(recipientOdsOrganisationsForDb);
        context.Add(organisation);
        context.Add(competition);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        odsService.GetSublocationsByParentOdsCode(organisation.ExternalIdentifier)
            .Returns(validSublocations);

        Exception exception = await Record.ExceptionAsync(
            async () =>
            {
                await service.SetCompetitionSublocationsAndRecipients(
                    organisation.InternalIdentifier,
                    competition.Id,
                    competitionSublocationsToSubmit);
            });

        exception.Should().NotBeNull();
        exception!.GetType().Should().Be(typeof(InvalidOperationException));
        exception!.Message.Should().Be("Provided sublocations not valid for this organisation.");
    }

    public static IEnumerable<object[]> CompetitionSublocationAndInvalidRecipientsData()
    {
        Organisation organisation = CommonOrganisationFactory();
        Competition competition = CommonCompetitionFactory();

        competition.CompetitionSublocations =
            [CommonCompetitionSublocationFactory("XXXX"), CommonCompetitionSublocationFactory("XXXA")];

        return
        [
            [
                organisation, competition,
                CommonEntityOdsOrganisationFactory(CommonOrganisationExternalIdentifier),
                new List<EntityOdsOrganisation>
                {
                    CommonEntityOdsOrganisationFactory("XXXX"), CommonEntityOdsOrganisationFactory("XXXA"),
                },
                new List<EntityOdsOrganisation>
                {
                    CommonEntityOdsOrganisationFactory("AAAA"),
                    CommonEntityOdsOrganisationFactory("AAAB"),
                    CommonEntityOdsOrganisationFactory("BAAA"),
                    CommonEntityOdsOrganisationFactory("BAAB"),
                },
                new List<ServiceContractOdsOrganisation>
                {
                    CommonServiceContractOdsOrganisationFactory("XXXX"),
                    CommonServiceContractOdsOrganisationFactory("XXXA"),
                },
                new Dictionary<string, List<ServiceRecipient>>
                {
                    {
                        "XXXX",
                        [CommonServiceRecipientFactory("AXXA", "XXXX"), CommonServiceRecipientFactory("AAAB", "XXXX")]
                    },
                    {
                        "XXXA",
                        [CommonServiceRecipientFactory("BAAA", "XXXA"), CommonServiceRecipientFactory("BAAB", "XXXB")]
                    },
                },
                new List<CompetitionSublocation>
                {
                    CommonCompetitionSublocationFactory(
                        "XXXX",
                        [
                            CommonCompetitionSublocationRecipientFactory("AAAA", "XXXX"),
                            CommonCompetitionSublocationRecipientFactory("AAAB", "XXXX"),
                        ]),
                    CommonCompetitionSublocationFactory(
                        "XXXA",
                        [
                            CommonCompetitionSublocationRecipientFactory("BAAA", "XXXA"),
                            CommonCompetitionSublocationRecipientFactory("BAAB", "XXXA"),
                        ]),
                },
            ],
        ];
    }

    [Theory]
    [MockInMemoryDbMemberAutoData(nameof(CompetitionSublocationAndInvalidRecipientsData))]
    public static async Task SetCompetitionSublocationAndRecipients_RejectsInvalidOperations_RecipientsNotValid(
        Organisation organisation,
        Competition competition,
        EntityOdsOrganisation ownerOdsOrganisation,
        List<EntityOdsOrganisation> sublocationOdsOrganisationsForDb,
        List<EntityOdsOrganisation> recipientOdsOrganisationsForDb,
        List<ServiceContractOdsOrganisation> validSublocations,
        Dictionary<string, List<ServiceRecipient>> validRecipientsPerSublocation,
        List<CompetitionSublocation> competitionSublocationsToSubmit,
        [Frozen] BuyingCatalogueDbContext context,
        [Frozen] IOdsService odsService,
        CompetitionsService service)
    {
        competition.Organisation = organisation;

        context.Add(ownerOdsOrganisation);
        context.AddRange(sublocationOdsOrganisationsForDb);
        context.AddRange(recipientOdsOrganisationsForDb);
        context.Add(organisation);
        context.Add(competition);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        odsService.GetSublocationsByParentOdsCode(organisation.ExternalIdentifier)
            .Returns(validSublocations);

        foreach (KeyValuePair<string, List<ServiceRecipient>> kvp in validRecipientsPerSublocation)
        {
            odsService.GetServiceRecipientsBySublocation(kvp.Key).Returns(kvp.Value);
        }

        Exception exception = await Record.ExceptionAsync(
            async () =>
            {
                await service.SetCompetitionSublocationsAndRecipients(
                    organisation.InternalIdentifier,
                    competition.Id,
                    competitionSublocationsToSubmit);
            });

        exception.Should().NotBeNull();
        exception!.GetType().Should().Be(typeof(InvalidOperationException));
        exception!.Message.Should().Be("Provided recipients not valid for this organisation or its sublocations.");
    }

    public static IEnumerable<object[]> CompetitionSublocationAndRecipientsData()
    {
        Organisation organisation = CommonOrganisationFactory();
        Competition competition = CommonCompetitionFactory();

        competition.CompetitionSublocations =
            [CommonCompetitionSublocationFactory("XXXX"), CommonCompetitionSublocationFactory("XXXA")];

        return
        [
            [
                organisation, competition,
                CommonEntityOdsOrganisationFactory(CommonOrganisationExternalIdentifier),
                new List<EntityOdsOrganisation>
                {
                    CommonEntityOdsOrganisationFactory("XXXX"), CommonEntityOdsOrganisationFactory("XXXA"),
                },
                new List<EntityOdsOrganisation>
                {
                    CommonEntityOdsOrganisationFactory("AAAA"),
                    CommonEntityOdsOrganisationFactory("AAAB"),
                    CommonEntityOdsOrganisationFactory("BAAA"),
                    CommonEntityOdsOrganisationFactory("BAAB"),
                },
                new List<ServiceContractOdsOrganisation>
                {
                    CommonServiceContractOdsOrganisationFactory("XXXX"),
                    CommonServiceContractOdsOrganisationFactory("XXXA"),
                },
                new Dictionary<string, List<ServiceRecipient>>
                {
                    {
                        "XXXX",
                        [CommonServiceRecipientFactory("AAAA", "XXXX"), CommonServiceRecipientFactory("AAAB", "XXXX")]
                    },
                    {
                        "XXXA",
                        [CommonServiceRecipientFactory("BAAA", "XXXA"), CommonServiceRecipientFactory("BAAB", "XXXB")]
                    },
                },
                new List<CompetitionSublocation>
                {
                    CommonCompetitionSublocationFactory(
                        "XXXX",
                        [
                            CommonCompetitionSublocationRecipientFactory("AAAA", "XXXX"),
                            CommonCompetitionSublocationRecipientFactory("AAAB", "XXXX"),
                        ]),
                    CommonCompetitionSublocationFactory(
                        "XXXA",
                        [
                            CommonCompetitionSublocationRecipientFactory("BAAA", "XXXA"),
                            CommonCompetitionSublocationRecipientFactory("BAAB", "XXXA"),
                        ]),
                },
            ],
        ];
    }

    [Theory]
    [MockInMemoryDbMemberAutoData(nameof(CompetitionSublocationAndRecipientsData))]
    public static async Task SetCompetitionSublocationAndRecipients_SetsCompetitionSublocationAndRecipients(
        Organisation organisation,
        Competition competition,
        EntityOdsOrganisation ownerOdsOrganisation,
        List<EntityOdsOrganisation> sublocationOdsOrganisationsForDb,
        List<EntityOdsOrganisation> recipientOdsOrganisationsForDb,
        List<ServiceContractOdsOrganisation> validSublocations,
        Dictionary<string, List<ServiceRecipient>> validRecipientsPerSublocation,
        List<CompetitionSublocation> competitionSublocationsToSubmit,
        [Frozen] BuyingCatalogueDbContext context,
        [Frozen] IOdsService odsService,
        CompetitionsService service)
    {
        competition.Organisation = organisation;

        context.Add(ownerOdsOrganisation);
        context.AddRange(sublocationOdsOrganisationsForDb);
        context.AddRange(recipientOdsOrganisationsForDb);
        context.Add(organisation);
        context.Add(competition);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        odsService.GetSublocationsByParentOdsCode(organisation.ExternalIdentifier)
            .Returns(validSublocations);

        foreach (KeyValuePair<string, List<ServiceRecipient>> kvp in validRecipientsPerSublocation)
        {
            odsService.GetServiceRecipientsBySublocation(kvp.Key).Returns(kvp.Value);
        }

        await service.SetCompetitionSublocationsAndRecipients(
            organisation.InternalIdentifier,
            competition.Id,
            competitionSublocationsToSubmit);

        Competition actualCompetition = await service.GetCompetitionWithSublocationsAndSublocationRecipients(
            organisation.InternalIdentifier,
            competition.Id);

        actualCompetition.CompetitionSublocations.Should()
            .BeEquivalentTo(
                competitionSublocationsToSubmit,
                opt => opt.Excluding(m => m.Competition)
                    .Excluding(m => m.SublocationOrganisation)
                    .Excluding(m => m.SublocationRecipients));
        foreach (CompetitionSublocation actualSublocation in actualCompetition.CompetitionSublocations)
        {
            actualSublocation.SublocationRecipients.Should()
                .BeEquivalentTo(
                    competitionSublocationsToSubmit.First(
                            x => x.SublocationOdsCode == actualSublocation.SublocationOdsCode)
                        .SublocationRecipients,
                    opt => opt.Excluding(x => x.Competition)
                        .Excluding(x => x.ParentSublocation)
                        .Excluding(x => x.RecipientOrganisation));
        }
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetContractLength_SetsLength(
        int contractLength,
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.ContractLength = null;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.SetContractLength(organisation.InternalIdentifier, competition.Id, contractLength);

        var updatedCompetition = await context.Competitions.FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.ContractLength.Should().Be(contractLength);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetCompetitionCriteria(
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.IncludesNonPrice = false;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.SetCompetitionCriteria(organisation.InternalIdentifier, competition.Id, true);

        var updatedCompetition = await context.Competitions
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.IncludesNonPrice.Should().BeTrue();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetCompetitionWeightings_SetsWeightings(
        int priceWeighting,
        int nonPriceWeighting,
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.SetCompetitionWeightings(organisation.InternalIdentifier, competition.Id, priceWeighting, nonPriceWeighting);

        var updatedCompetition = await context.Competitions.Include(x => x.Weightings)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.Weightings.Should().NotBeNull();
        updatedCompetition.Weightings.Price.Should().Be(priceWeighting);
        updatedCompetition.Weightings.NonPrice.Should().Be(nonPriceWeighting);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetCompetitionWithWeightings_ReturnsExpected(
        Weightings weightings,
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.Weightings = weightings;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var updatedCompetition = await service.GetCompetitionWithWeightings(organisation.InternalIdentifier, competition.Id);

        updatedCompetition.Weightings.Should().BeEquivalentTo(weightings, opt => opt.Excluding(m => m.CompetitionId));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetImplementationCriteria_NullNonPriceElements_CreatesNonPriceElements(
        Organisation organisation,
        Competition competition,
        string requirements,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        competition.NonPriceElements.Should().BeNull();

        await service.SetImplementationCriteria(organisation.InternalIdentifier, competition.Id, requirements);

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        updatedCompetition.NonPriceElements.Should().NotBeNull();
        updatedCompetition.NonPriceElements.Implementation.Should().NotBeNull();
        updatedCompetition.NonPriceElements.Implementation.Requirements.Should().Be(requirements);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetImplementationCriteria_NullImplementationCriteria_CreatesImplementationCriteria(
        Organisation organisation,
        Competition competition,
        string requirements,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.NonPriceElements = new();
        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        competition.NonPriceElements.Should().NotBeNull();
        competition.NonPriceElements.Implementation.Should().BeNull();

        await service.SetImplementationCriteria(organisation.InternalIdentifier, competition.Id, requirements);

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        updatedCompetition.NonPriceElements.Should().NotBeNull();
        updatedCompetition.NonPriceElements.Implementation.Should().NotBeNull();
        updatedCompetition.NonPriceElements.Implementation.Requirements.Should().Be(requirements);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetImplementationCriteria_ExistingRequirements_UpdatesRequirements(
        string oldRequirements,
        Organisation organisation,
        Competition competition,
        string requirements,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.NonPriceElements = new()
        {
            Implementation = new() { Requirements = oldRequirements },
        };

        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        competition.NonPriceElements.Should().NotBeNull();
        competition.NonPriceElements.Implementation.Should().NotBeNull();

        await service.SetImplementationCriteria(organisation.InternalIdentifier, competition.Id, requirements);

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        updatedCompetition.NonPriceElements.Should().NotBeNull();
        updatedCompetition.NonPriceElements.Implementation.Should().NotBeNull();
        updatedCompetition.NonPriceElements.Implementation.Requirements.Should().Be(requirements);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetImplementationCriteria_WithImplementationScore_RemovesScore(
        string oldRequirements,
        Organisation organisation,
        Competition competition,
        Solution solution,
        string requirements,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.NonPriceElements = new()
        {
            Implementation = new() { Requirements = oldRequirements },
        };

        competition.CompetitionSolutions = new List<CompetitionSolution>
        {
            new(competition.Id, solution.CatalogueItemId)
            {
                Scores = new List<SolutionScore> { new(ScoreType.Implementation, 3), },
            },
        };

        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        competition.NonPriceElements.Should().NotBeNull();
        competition.NonPriceElements.Implementation.Should().NotBeNull();

        await service.SetImplementationCriteria(organisation.InternalIdentifier, competition.Id, requirements);

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        updatedCompetition.CompetitionSolutions.Should().NotContain(x => x.HasScoreType(ScoreType.Implementation));
        updatedCompetition.NonPriceElements.Should().NotBeNull();
        updatedCompetition.NonPriceElements.Implementation.Should().NotBeNull();
        updatedCompetition.NonPriceElements.Implementation.Requirements.Should().Be(requirements);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetServiceLevelCriteria_NullNonPriceElements_CreatesNonPriceElements(
        List<Iso8601DayOfWeek> applicableDays,
        DateTime timeFrom,
        DateTime timeUntil,
        bool includesBankHolidays,
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        competition.NonPriceElements.Should().BeNull();

        await service.SetServiceLevelCriteria(
            organisation.InternalIdentifier,
            competition.Id,
            timeFrom,
            timeUntil,
            applicableDays,
            includesBankHolidays);

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        updatedCompetition.NonPriceElements.Should().NotBeNull();
        updatedCompetition.NonPriceElements.ServiceLevel.Should().NotBeNull();
        updatedCompetition.NonPriceElements.ServiceLevel.ApplicableDays.Should().BeEquivalentTo(applicableDays);
        updatedCompetition.NonPriceElements.ServiceLevel.TimeFrom.Should().Be(timeFrom);
        updatedCompetition.NonPriceElements.ServiceLevel.TimeUntil.Should().Be(timeUntil);
        updatedCompetition.NonPriceElements.ServiceLevel.IncludesBankHolidays.Should().Be(includesBankHolidays);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetServiceLevelCriteria_NullServiceLevelCriteria_CreatesServiceLevelCriteria(
        List<Iso8601DayOfWeek> applicableDays,
        DateTime timeFrom,
        DateTime timeUntil,
        bool includesBankHolidays,
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.NonPriceElements = new();
        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        competition.NonPriceElements.Should().NotBeNull();
        competition.NonPriceElements.ServiceLevel.Should().BeNull();

        await service.SetServiceLevelCriteria(
            organisation.InternalIdentifier,
            competition.Id,
            timeFrom,
            timeUntil,
            applicableDays,
            includesBankHolidays);

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        updatedCompetition.NonPriceElements.Should().NotBeNull();
        updatedCompetition.NonPriceElements.ServiceLevel.Should().NotBeNull();
        updatedCompetition.NonPriceElements.ServiceLevel.ApplicableDays.Should().BeEquivalentTo(applicableDays);
        updatedCompetition.NonPriceElements.ServiceLevel.TimeFrom.Should().Be(timeFrom);
        updatedCompetition.NonPriceElements.ServiceLevel.TimeUntil.Should().Be(timeUntil);
        updatedCompetition.NonPriceElements.ServiceLevel.IncludesBankHolidays.Should().Be(includesBankHolidays);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetServiceLevelCriteria_ExistingServiceLevelCriteria_UpdatesServiceLevelCriteria(
        ServiceLevelCriteria oldServiceLevelCriteria,
        ServiceLevelCriteria newServiceLevelCriteria,
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.NonPriceElements = new() { ServiceLevel = oldServiceLevelCriteria };
        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        competition.NonPriceElements.Should().NotBeNull();
        competition.NonPriceElements.ServiceLevel.Should().NotBeNull();

        newServiceLevelCriteria.NonPriceElementsId = competition.NonPriceElements.Id;

        await service.SetServiceLevelCriteria(
            organisation.InternalIdentifier,
            competition.Id,
            newServiceLevelCriteria.TimeFrom,
            newServiceLevelCriteria.TimeUntil,
            newServiceLevelCriteria.ApplicableDays,
            newServiceLevelCriteria.IncludesBankHolidays);

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        updatedCompetition.NonPriceElements.Should().NotBeNull();
        updatedCompetition.NonPriceElements.ServiceLevel.Should().NotBeNull();
        updatedCompetition.NonPriceElements.ServiceLevel.Should()
            .BeEquivalentTo(newServiceLevelCriteria, opt => opt.Excluding(m => m.Id));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetServiceLevelCriteria_WithServiceLevelScore_RemovesScore(
        ServiceLevelCriteria oldServiceLevelCriteria,
        ServiceLevelCriteria newServiceLevelCriteria,
        Organisation organisation,
        Solution solution,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.NonPriceElements = new() { ServiceLevel = oldServiceLevelCriteria };
        competition.OrganisationId = organisation.Id;

        competition.CompetitionSolutions = new List<CompetitionSolution>
        {
            new(competition.Id, solution.CatalogueItemId)
            {
                Scores = new List<SolutionScore> { new(ScoreType.ServiceLevel, 3), },
            },
        };

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        competition.NonPriceElements.Should().NotBeNull();
        competition.NonPriceElements.ServiceLevel.Should().NotBeNull();

        newServiceLevelCriteria.NonPriceElementsId = competition.NonPriceElements.Id;

        await service.SetServiceLevelCriteria(
            organisation.InternalIdentifier,
            competition.Id,
            newServiceLevelCriteria.TimeFrom,
            newServiceLevelCriteria.TimeUntil,
            newServiceLevelCriteria.ApplicableDays,
            newServiceLevelCriteria.IncludesBankHolidays);

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        updatedCompetition.CompetitionSolutions.Should().NotContain(x => x.HasScoreType(ScoreType.ServiceLevel));
        updatedCompetition.NonPriceElements.Should().NotBeNull();
        updatedCompetition.NonPriceElements.ServiceLevel.Should().NotBeNull();
        updatedCompetition.NonPriceElements.ServiceLevel.Should()
            .BeEquivalentTo(newServiceLevelCriteria, opt => opt.Excluding(m => m.Id));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetInteroperabilityCriteria_AddsIntegrations(
        Organisation organisation,
        Competition competition,
        Integration integration,
        List<IntegrationType> integrationTypes,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        integration.IntegrationTypes = integrationTypes;
        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.Integrations.Add(integration);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.SetInteroperabilityCriteria(organisation.InternalIdentifier, competition.Id, integrationTypes.Select(x => x.Id));

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        integrationTypes.ForEach(
            x => updatedCompetition.NonPriceElements.IntegrationTypes.Should().Contain(y => x.Id == y.Id));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetInteroperabilityCriteria_StaleIntegrations_RemovesStaleIntegrations(
        Organisation organisation,
        Competition competition,
        List<IntegrationType> integrationTypes,
        IntegrationType staleIntegration,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.NonPriceElements = new() { IntegrationTypes = new List<IntegrationType>() { staleIntegration, }, };

        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.IntegrationTypes.Add(staleIntegration);
        context.IntegrationTypes.AddRange(integrationTypes);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.SetInteroperabilityCriteria(organisation.InternalIdentifier, competition.Id, integrationTypes.Select(x => x.Id));

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        updatedCompetition.NonPriceElements.IntegrationTypes.Should().NotContain(x => x.Id == staleIntegration.Id);
        integrationTypes.ForEach(
            x => updatedCompetition.NonPriceElements.IntegrationTypes.Should().Contain(y => x.Id == y.Id));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetInteroperabilityCriteria_WithInteroperabilityScore_RemovesScores(
        Organisation organisation,
        Competition competition,
        Solution solution,
        List<IntegrationType> integrationTypes,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.CompetitionSolutions = new List<CompetitionSolution>
        {
            new(competition.Id, solution.CatalogueItemId)
            {
                Scores = new List<SolutionScore> { new(ScoreType.Interoperability, 3), },
            },
        };

        competition.NonPriceElements = new() { IntegrationTypes = integrationTypes, };

        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.IntegrationTypes.AddRange(integrationTypes);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var integrations = integrationTypes.Skip(1).ToList();

        await service.SetInteroperabilityCriteria(
            organisation.InternalIdentifier,
            competition.Id,
            integrations.Select(x => x.Id));

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        updatedCompetition.CompetitionSolutions.Should().NotContain(x => x.HasScoreType(ScoreType.Interoperability));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetCriteriaReviewed_SetsCriteriaReviewed(
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.HasReviewedCriteria = false;
        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.SetCriteriaReviewed(organisation.InternalIdentifier, competition.Id);

        var updatedCompetition = await service.GetCompetition(organisation.InternalIdentifier, competition.Id);

        updatedCompetition.HasReviewedCriteria.Should().BeTrue();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetNonPriceWeights_NullNonPriceWeights_CreatesAndSetsWeights(
        int implementationWeight,
        int interoperabilityWeight,
        int serviceLevelWeight,
        int featuresWeight,
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.NonPriceElements = new();
        competition.HasReviewedCriteria = false;
        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.SetNonPriceWeights(
            organisation.InternalIdentifier,
            competition.Id,
            implementationWeight,
            interoperabilityWeight,
            serviceLevelWeight,
            featuresWeight);

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        var nonPriceElementWeightings = updatedCompetition.NonPriceElements.NonPriceWeights;

        nonPriceElementWeightings.Should().NotBeNull();
        nonPriceElementWeightings.Implementation.Should().Be(implementationWeight);
        nonPriceElementWeightings.Interoperability.Should().Be(interoperabilityWeight);
        nonPriceElementWeightings.ServiceLevel.Should().Be(serviceLevelWeight);
        nonPriceElementWeightings.Features.Should().Be(featuresWeight);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetNonPriceWeights_ExistingNonPriceWeights_SetsWeights(
        int implementationWeight,
        int interoperabilityWeight,
        int serviceLevelWeight,
        int featuresWeight,
        NonPriceWeights nonPriceWeights,
        Organisation organisation,
        Competition competition,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.NonPriceElements = new() { NonPriceWeights = nonPriceWeights };
        competition.HasReviewedCriteria = false;
        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.SetNonPriceWeights(
            organisation.InternalIdentifier,
            competition.Id,
            implementationWeight,
            interoperabilityWeight,
            serviceLevelWeight,
            featuresWeight);

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        var nonPriceElementWeightings = updatedCompetition.NonPriceElements.NonPriceWeights;

        nonPriceElementWeightings.Should().NotBeNull();
        nonPriceElementWeightings.Implementation.Should().Be(implementationWeight);
        nonPriceElementWeightings.Interoperability.Should().Be(interoperabilityWeight);
        nonPriceElementWeightings.ServiceLevel.Should().Be(serviceLevelWeight);
        nonPriceElementWeightings.Features.Should().Be(featuresWeight);
    }

    [Theory]
    [MockAutoData]
    public static Task SetSolutionsImplementationScores_NullSolutionsScores_ThrowsArgumentNullException(
        string internalOrgId,
        int competitionId,
        CompetitionsService service)
        => FluentActions
            .Awaiting(() => service.SetSolutionsImplementationScores(internalOrgId, competitionId, null))
            .Should()
            .ThrowAsync<ArgumentNullException>();

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetSolutionsImplementationScores_ExistingScore_UpdatesScore(
        Organisation organisation,
        Competition competition,
        Solution solution,
        int score,
        string justification,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        const ScoreType scoreType = ScoreType.Implementation;

        var competitionSolution = new CompetitionSolution(competition.Id, solution.CatalogueItemId)
        {
            Scores = new List<SolutionScore> { new(scoreType, score + 2) },
            IsShortlisted = true,
        };

        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions.Add(competitionSolution);

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.Solutions.Add(solution);

        competitionSolution.HasScoreType(scoreType).Should().BeTrue();

        await context.SaveChangesAsync();

        await service.SetSolutionsImplementationScores(
            organisation.InternalIdentifier,
            competition.Id,
            new Dictionary<CatalogueItemId, (int, string)> { { competitionSolution.CatalogueItemId, (score, justification) } });

        var updatedCompetition =
            await service.GetCompetitionWithSolutions(organisation.InternalIdentifier, competition.Id);
        var updatedSolution =
            updatedCompetition.CompetitionSolutions.First(x => x.CatalogueItemId == competitionSolution.CatalogueItemId);

        updatedSolution.HasScoreType(scoreType).Should().BeTrue();
        updatedSolution.GetScoreByType(scoreType).Score.Should().Be(score);
        updatedSolution.GetScoreByType(scoreType).Justification.Should().Be(justification);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetSolutionsImplementationScores_NewScore_AddsScore(
        Organisation organisation,
        Competition competition,
        Solution solution,
        int score,
        string justification,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        const ScoreType scoreType = ScoreType.Implementation;

        var competitionSolution = new CompetitionSolution(competition.Id, solution.CatalogueItemId)
        {
            IsShortlisted = true,
        };

        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions.Add(competitionSolution);

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.Solutions.Add(solution);

        competitionSolution.HasScoreType(scoreType).Should().BeFalse();

        await context.SaveChangesAsync();

        await service.SetSolutionsImplementationScores(
            organisation.InternalIdentifier,
            competition.Id,
            new Dictionary<CatalogueItemId, (int, string)> { { competitionSolution.CatalogueItemId, (score, justification) } });

        var updatedCompetition =
            await service.GetCompetitionWithSolutions(organisation.InternalIdentifier, competition.Id);
        var updatedSolution =
            updatedCompetition.CompetitionSolutions.First(x => x.CatalogueItemId == competitionSolution.CatalogueItemId);

        updatedSolution.HasScoreType(scoreType).Should().BeTrue();
        updatedSolution.GetScoreByType(scoreType).Score.Should().Be(score);
        updatedSolution.GetScoreByType(scoreType).Justification.Should().Be(justification);
    }

    [Theory]
    [MockAutoData]
    public static Task SetSolutionsInteroperabilityScores_NullSolutionsScores_ThrowsArgumentNullException(
        string internalOrgId,
        int competitionId,
        CompetitionsService service)
        => FluentActions
            .Awaiting(() => service.SetSolutionsInteroperabilityScores(internalOrgId, competitionId, null))
            .Should()
            .ThrowAsync<ArgumentNullException>();

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetSolutionsInteroperabilityScores_ExistingScore_UpdatesScore(
        Organisation organisation,
        Competition competition,
        Solution solution,
        int score,
        string justification,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        const ScoreType scoreType = ScoreType.Interoperability;

        var competitionSolution = new CompetitionSolution(competition.Id, solution.CatalogueItemId)
        {
            Scores = new List<SolutionScore> { new(scoreType, score + 2) },
            IsShortlisted = true,
        };

        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions.Add(competitionSolution);

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.Solutions.Add(solution);

        competitionSolution.HasScoreType(scoreType).Should().BeTrue();

        await context.SaveChangesAsync();

        await service.SetSolutionsInteroperabilityScores(
            organisation.InternalIdentifier,
            competition.Id,
            new Dictionary<CatalogueItemId, (int, string)> { { competitionSolution.CatalogueItemId, (score, justification) } });

        var updatedCompetition =
            await service.GetCompetitionWithSolutions(organisation.InternalIdentifier, competition.Id);
        var updatedSolution =
            updatedCompetition.CompetitionSolutions.First(x => x.CatalogueItemId == competitionSolution.CatalogueItemId);

        updatedSolution.HasScoreType(scoreType).Should().BeTrue();
        updatedSolution.GetScoreByType(scoreType).Score.Should().Be(score);
        updatedSolution.GetScoreByType(scoreType).Justification.Should().Be(justification);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetSolutionsInteroperabilityScores_NewScore_AddsScore(
        Organisation organisation,
        Competition competition,
        Solution solution,
        int score,
        string justification,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        const ScoreType scoreType = ScoreType.Interoperability;

        var competitionSolution = new CompetitionSolution(competition.Id, solution.CatalogueItemId)
        {
            IsShortlisted = true,
        };

        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions.Add(competitionSolution);

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.Solutions.Add(solution);

        competitionSolution.HasScoreType(scoreType).Should().BeFalse();

        await context.SaveChangesAsync();

        await service.SetSolutionsInteroperabilityScores(
            organisation.InternalIdentifier,
            competition.Id,
            new Dictionary<CatalogueItemId, (int, string)> { { competitionSolution.CatalogueItemId, (score, justification) } });

        var updatedCompetition =
            await service.GetCompetitionWithSolutions(organisation.InternalIdentifier, competition.Id);
        var updatedSolution =
            updatedCompetition.CompetitionSolutions.First(x => x.CatalogueItemId == competitionSolution.CatalogueItemId);

        updatedSolution.HasScoreType(scoreType).Should().BeTrue();
        updatedSolution.GetScoreByType(scoreType).Score.Should().Be(score);
        updatedSolution.GetScoreByType(scoreType).Justification.Should().Be(justification);
    }

    [Theory]
    [MockAutoData]
    public static Task SetSolutionsServiceLevelScores_NullSolutionsScores_ThrowsArgumentNullException(
        string internalOrgId,
        int competitionId,
        CompetitionsService service)
        => FluentActions
            .Awaiting(() => service.SetSolutionsServiceLevelScores(internalOrgId, competitionId, null))
            .Should()
            .ThrowAsync<ArgumentNullException>();

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetSolutionsServiceLevelScores_ExistingScore_UpdatesScore(
        Organisation organisation,
        Competition competition,
        Solution solution,
        int score,
        string justification,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        const ScoreType scoreType = ScoreType.ServiceLevel;

        var competitionSolution = new CompetitionSolution(competition.Id, solution.CatalogueItemId)
        {
            Scores = new List<SolutionScore> { new(scoreType, score + 2) },
            IsShortlisted = true,
        };

        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions.Add(competitionSolution);

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.Solutions.Add(solution);

        competitionSolution.HasScoreType(scoreType).Should().BeTrue();

        await context.SaveChangesAsync();

        await service.SetSolutionsServiceLevelScores(
            organisation.InternalIdentifier,
            competition.Id,
            new Dictionary<CatalogueItemId, (int, string)> { { competitionSolution.CatalogueItemId, (score, justification) } });

        var updatedCompetition =
            await service.GetCompetitionWithSolutions(organisation.InternalIdentifier, competition.Id);
        var updatedSolution =
            updatedCompetition.CompetitionSolutions.First(x => x.CatalogueItemId == competitionSolution.CatalogueItemId);

        updatedSolution.HasScoreType(scoreType).Should().BeTrue();
        updatedSolution.GetScoreByType(scoreType).Score.Should().Be(score);
        updatedSolution.GetScoreByType(scoreType).Justification.Should().Be(justification);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetSolutionsServiceLevelScores_NewScore_AddsScore(
        Organisation organisation,
        Competition competition,
        Solution solution,
        int score,
        string justification,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        const ScoreType scoreType = ScoreType.ServiceLevel;

        var competitionSolution = new CompetitionSolution(competition.Id, solution.CatalogueItemId)
        {
            IsShortlisted = true,
        };

        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions.Add(competitionSolution);

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.Solutions.Add(solution);

        competitionSolution.HasScoreType(scoreType).Should().BeFalse();

        await context.SaveChangesAsync();

        await service.SetSolutionsServiceLevelScores(
            organisation.InternalIdentifier,
            competition.Id,
            new Dictionary<CatalogueItemId, (int, string)> { { competitionSolution.CatalogueItemId, (score, justification) } });

        var updatedCompetition =
            await service.GetCompetitionWithSolutions(organisation.InternalIdentifier, competition.Id);
        var updatedSolution =
            updatedCompetition.CompetitionSolutions.First(x => x.CatalogueItemId == competitionSolution.CatalogueItemId);

        updatedSolution.HasScoreType(scoreType).Should().BeTrue();
        updatedSolution.GetScoreByType(scoreType).Score.Should().Be(score);
        updatedSolution.GetScoreByType(scoreType).Justification.Should().Be(justification);
    }

    [Theory]
    [MockAutoData]
    public static Task SetSolutionsFeaturesScores_NullSolutionsScores_ThrowsArgumentNullException(
        string internalOrgId,
        int competitionId,
        CompetitionsService service)
        => FluentActions
            .Awaiting(() => service.SetSolutionsFeaturesScores(internalOrgId, competitionId, null))
            .Should()
            .ThrowAsync<ArgumentNullException>();

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetSolutionsFeaturesScores_ExistingScore_UpdatesScore(
        Organisation organisation,
        Competition competition,
        Solution solution,
        int score,
        string justification,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        const ScoreType scoreType = ScoreType.Features;

        var competitionSolution = new CompetitionSolution(competition.Id, solution.CatalogueItemId)
        {
            Scores = new List<SolutionScore> { new(scoreType, score + 2) },
            IsShortlisted = true,
        };

        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions.Add(competitionSolution);

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.Solutions.Add(solution);

        competitionSolution.HasScoreType(scoreType).Should().BeTrue();

        await context.SaveChangesAsync();

        await service.SetSolutionsFeaturesScores(
            organisation.InternalIdentifier,
            competition.Id,
            new Dictionary<CatalogueItemId, (int, string)> { { competitionSolution.CatalogueItemId, (score, justification) } });

        var updatedCompetition =
            await service.GetCompetitionWithSolutions(organisation.InternalIdentifier, competition.Id);
        var updatedSolution =
            updatedCompetition.CompetitionSolutions.First(x => x.CatalogueItemId == competitionSolution.CatalogueItemId);

        updatedSolution.HasScoreType(scoreType).Should().BeTrue();
        updatedSolution.GetScoreByType(scoreType).Score.Should().Be(score);
        updatedSolution.GetScoreByType(scoreType).Justification.Should().Be(justification);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetSolutionsFeaturesScores_NewScore_AddsScore(
        Organisation organisation,
        Competition competition,
        Solution solution,
        int score,
        string justification,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        const ScoreType scoreType = ScoreType.Features;

        var competitionSolution = new CompetitionSolution(competition.Id, solution.CatalogueItemId)
        {
            IsShortlisted = true,
        };

        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions.Add(competitionSolution);

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.Solutions.Add(solution);

        competitionSolution.HasScoreType(scoreType).Should().BeFalse();

        await context.SaveChangesAsync();

        await service.SetSolutionsFeaturesScores(
            organisation.InternalIdentifier,
            competition.Id,
            new Dictionary<CatalogueItemId, (int, string)> { { competitionSolution.CatalogueItemId, (score, justification) } });

        var updatedCompetition =
            await service.GetCompetitionWithSolutions(organisation.InternalIdentifier, competition.Id);
        var updatedSolution =
            updatedCompetition.CompetitionSolutions.First(x => x.CatalogueItemId == competitionSolution.CatalogueItemId);

        updatedSolution.HasScoreType(scoreType).Should().BeTrue();
        updatedSolution.GetScoreByType(scoreType).Score.Should().Be(score);
        updatedSolution.GetScoreByType(scoreType).Justification.Should().Be(justification);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task RemoveNonPriceElements_RemovesNonPriceElements(
        Organisation organisation,
        Competition competition,
        List<IntegrationType> integrationTypes,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.NonPriceElements = new()
        {
            ServiceLevel =
                new()
                {
                    ApplicableDays = Enum.GetValues<Iso8601DayOfWeek>(),
                    TimeFrom = DateTime.UtcNow,
                    TimeUntil = DateTime.UtcNow,
                },
            Implementation = new() { Requirements = "Test" },
            IntegrationTypes = integrationTypes,
        };

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.IntegrationTypes.AddRange(integrationTypes);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        competition.NonPriceElements.Should().NotBeNull();

        await service.RemoveNonPriceElements(organisation.InternalIdentifier, competition.Id);

        var updatedCompetition = await context.Competitions.Include(x => x.NonPriceElements)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.NonPriceElements.Should().BeNull();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task AddAssociatedServices_NoExistingServices_AddsAssociatedServices(
        Organisation organisation,
        Supplier supplier,
        Competition competition,
        CompetitionSolution competitionSolution,
        Solution solution,
        List<AssociatedService> associatedServices,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        supplier.CatalogueItems = null;

        solution.CatalogueItem.Supplier = null;
        solution.CatalogueItem.SupplierId = supplier.Id;

        associatedServices.ForEach(
            x =>
            {
                x.CatalogueItem.Supplier = null;
                x.CatalogueItem.SupplierId = supplier.Id;
            });

        var supplierServiceAssociations = associatedServices.Select(
            x => new SupplierServiceAssociation(solution.CatalogueItemId, x.CatalogueItemId));

        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionSolution.IsShortlisted = true;
        competitionSolution.CompetitionId = competition.Id;
        competitionSolution.CatalogueItemId = solution.CatalogueItemId;
        competitionSolution.Services = new List<CompetitionCatalogueItem>();

        var serviceIds = associatedServices.Select(x => x.CatalogueItemId).ToList();

        context.Organisations.Add(organisation);
        context.Suppliers.Add(supplier);
        context.Solutions.Add(solution);
        context.AssociatedServices.AddRange(associatedServices);
        context.SupplierServiceAssociations.AddRange(supplierServiceAssociations);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        await service.AddAssociatedServices(
            organisation.InternalIdentifier,
            competition.Id,
            solution.CatalogueItemId,
            serviceIds);

        var updatedCompetition = await context.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Services)
            .ThenInclude(x => x.CatalogueItem)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == organisation.InternalIdentifier && x.Id == competition.Id);

        var updatedCompetitionSolution =
            updatedCompetition.CompetitionSolutions.First(x => x.CatalogueItemId == solution.CatalogueItemId);

        var solutionServices = updatedCompetitionSolution.Services.ToList();

        solutionServices.Should().HaveCount(associatedServices.Count);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task AddAssociatedServices_ExistingService_DoesNotRemoveExistingService(
        Organisation organisation,
        Supplier supplier,
        Competition competition,
        CompetitionSolution competitionSolution,
        Solution solution,
        List<AssociatedService> associatedServices,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        supplier.CatalogueItems = null;

        solution.CatalogueItem.Supplier = null;
        solution.CatalogueItem.SupplierId = supplier.Id;

        associatedServices.ForEach(
            x =>
            {
                x.CatalogueItem.Supplier = null;
                x.CatalogueItem.SupplierId = supplier.Id;
            });

        var supplierServiceAssociations = associatedServices.Select(
            x => new SupplierServiceAssociation(solution.CatalogueItemId, x.CatalogueItemId));

        var existingService = associatedServices.First();

        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionSolution.IsShortlisted = true;
        competitionSolution.CompetitionId = competition.Id;
        competitionSolution.CatalogueItemId = solution.CatalogueItemId;
        competitionSolution.Services = new List<CompetitionCatalogueItem>
        {
            new CompetitionAdditionalService(competition.Id, existingService.CatalogueItemId),
        };

        var serviceIds = associatedServices.Skip(1).Select(x => x.CatalogueItemId).ToList();

        context.Organisations.Add(organisation);
        context.Suppliers.Add(supplier);
        context.Solutions.Add(solution);
        context.AssociatedServices.AddRange(associatedServices);
        context.SupplierServiceAssociations.AddRange(supplierServiceAssociations);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        await service.AddAssociatedServices(
            organisation.InternalIdentifier,
            competition.Id,
            solution.CatalogueItemId,
            serviceIds);

        var updatedCompetition = await context.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Services)
            .ThenInclude(x => x.CatalogueItem)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == organisation.InternalIdentifier && x.Id == competition.Id);

        var updatedCompetitionSolution =
            updatedCompetition.CompetitionSolutions.First(x => x.CatalogueItemId == solution.CatalogueItemId);

        var solutionServices = updatedCompetitionSolution.Services.ToList();

        solutionServices.Should().HaveCount(associatedServices.Count);
        solutionServices.Should().Contain(x => x.CatalogueItemId == existingService.CatalogueItemId);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task RemoveAssociatedService_ValidService_RemovesService(
        Organisation organisation,
        Supplier supplier,
        Competition competition,
        CompetitionSolution competitionSolution,
        Solution solution,
        List<AssociatedService> associatedServices,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        supplier.CatalogueItems = null;

        solution.CatalogueItem.Supplier = null;
        solution.CatalogueItem.SupplierId = supplier.Id;

        associatedServices.ForEach(
            x =>
            {
                x.CatalogueItem.Supplier = null;
                x.CatalogueItem.SupplierId = supplier.Id;
            });

        var supplierServiceAssociations = associatedServices.Select(
            x => new SupplierServiceAssociation(solution.CatalogueItemId, x.CatalogueItemId));

        var existingService = associatedServices.First();

        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionSolution.IsShortlisted = true;
        competitionSolution.CompetitionId = competition.Id;
        competitionSolution.CatalogueItemId = solution.CatalogueItemId;
        competitionSolution.Services = new List<CompetitionCatalogueItem>
        {
            new CompetitionAssociatedService(competition.Id, existingService.CatalogueItemId),
        };

        context.Organisations.Add(organisation);
        context.Suppliers.Add(supplier);
        context.Solutions.Add(solution);
        context.AssociatedServices.AddRange(associatedServices);
        context.SupplierServiceAssociations.AddRange(supplierServiceAssociations);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        await service.RemoveAssociatedService(
            organisation.InternalIdentifier,
            competition.Id,
            solution.CatalogueItemId,
            existingService.CatalogueItemId);

        var updatedCompetition = await context.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Services)
            .ThenInclude(x => x.CatalogueItem)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == organisation.InternalIdentifier && x.Id == competition.Id);

        var updatedCompetitionSolution =
            updatedCompetition.CompetitionSolutions.First(x => x.CatalogueItemId == solution.CatalogueItemId);

        var solutionServices = updatedCompetitionSolution.Services.ToList();

        solutionServices.Should().BeEmpty();
        solutionServices.Should().NotContain(x => x.CatalogueItemId == existingService.CatalogueItemId);
    }

    [Theory]
    [MockAutoData]
    public static void ScoreSolutionPrices_IncludesNonPriceElements_ScoresAsExpected(
        Competition competition,
        CompetitionSolution winningSolution,
        CompetitionSolution nonWinningSolution)
    {
        const int priceWeighting = 70;
        const int nonPriceWeighting = 100 - priceWeighting;
        var quantity = new CompetitionItemQuantity() { Quantity = 1000 };

        competition.IncludesNonPrice = true;
        competition.Weightings = new() { Price = priceWeighting, NonPrice = nonPriceWeighting, };

        winningSolution.Quantities = [quantity];
        winningSolution.Price = new()
        {
            BillingPeriod = TimeUnit.PerMonth,
            ProvisioningType = ProvisioningType.Patient,
            CataloguePriceType = CataloguePriceType.Flat,
            CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
            Tiers = new List<CompetitionCatalogueItemPriceTier>
            {
                new() { LowerRange = 0, ListPrice = 1.01M, Price = 1.01M, },
            },
        };

        nonWinningSolution.Quantities = [quantity];
        nonWinningSolution.Price = new()
        {
            BillingPeriod = TimeUnit.PerMonth,
            ProvisioningType = ProvisioningType.Patient,
            CataloguePriceType = CataloguePriceType.Flat,
            CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
            Tiers = new List<CompetitionCatalogueItemPriceTier>
            {
                new() { LowerRange = 0, ListPrice = 5M, Price = 5M, },
            },
        };

        competition.CompetitionSolutions = new List<CompetitionSolution> { winningSolution, nonWinningSolution };

        CompetitionsService.ScoreSolutionPrices(competition);

        winningSolution.Scores.Should().ContainSingle(x => x.Score == 5 && x.WeightedScore == 3.5M);
        nonWinningSolution.Scores.Should().ContainSingle(x => x.Score == 1 && x.WeightedScore == 0.7M);
    }

    [Theory]
    [MockAutoData]
    public static void ScoreSolutionPrices_PriceOnly_ScoresAsExpected(
        Competition competition,
        CompetitionSolution winningSolution,
        CompetitionSolution nonWinningSolution)
    {
        competition.IncludesNonPrice = false;

        var quantity = new CompetitionItemQuantity { Quantity = 1000 };

        winningSolution.Quantities = [quantity];
        winningSolution.Price = new()
        {
            BillingPeriod = TimeUnit.PerMonth,
            ProvisioningType = ProvisioningType.Patient,
            CataloguePriceType = CataloguePriceType.Flat,
            CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed,
            Tiers = new List<CompetitionCatalogueItemPriceTier>
            {
                new() { LowerRange = 0, ListPrice = 1.01M, Price = 1.01M, },
            },
        };

        nonWinningSolution.Quantities = [quantity];
        nonWinningSolution.Price = new()
        {
            BillingPeriod = TimeUnit.PerMonth,
            ProvisioningType = ProvisioningType.Patient,
            CataloguePriceType = CataloguePriceType.Flat,
            CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
            Tiers = new List<CompetitionCatalogueItemPriceTier>
            {
                new() { LowerRange = 0, ListPrice = 5M, Price = 5M, },
            },
        };

        competition.CompetitionSolutions = new List<CompetitionSolution> { winningSolution, nonWinningSolution };

        CompetitionsService.ScoreSolutionPrices(competition);

        winningSolution.Scores.Should().ContainSingle(x => x.Score == 5 && x.WeightedScore == 5M);
        nonWinningSolution.Scores.Should().ContainSingle(x => x.Score == 1 && x.WeightedScore == 1M);
    }

    [Theory]
    [MockAutoData]
    public static void SetNonPriceScoreWeightings(
        Competition competition,
        CompetitionSolution competitionSolution)
    {
        competition.IncludesNonPrice = true;
        competition.NonPriceElements = new()
        {
            NonPriceWeights = new() { Implementation = 50, Interoperability = 25, ServiceLevel = 25, },
            Implementation = new(),
            IntegrationTypes = new List<IntegrationType> { new() },
            ServiceLevel = new(),
        };

        competitionSolution.Scores = new List<SolutionScore>
        {
            new(ScoreType.Implementation, 5), new(ScoreType.Interoperability, 3), new(ScoreType.ServiceLevel, 2),
        };

        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        CompetitionsService.SetNonPriceScoreWeightings(competition);

        competitionSolution.Scores.Should()
            .ContainSingle(x => x.ScoreType == ScoreType.Implementation && x.Score == 5 && x.WeightedScore == 2.5M);

        competitionSolution.Scores.Should()
            .ContainSingle(x => x.ScoreType == ScoreType.Interoperability && x.Score == 3 && x.WeightedScore == 0.75M);

        competitionSolution.Scores.Should()
            .ContainSingle(x => x.ScoreType == ScoreType.ServiceLevel && x.Score == 2 && x.WeightedScore == 0.5M);
    }

    [Theory]
    [MockAutoData]
    public static void SetWinningSolution_SetsExpectedWinningSolution(
        Competition competition,
        CompetitionSolution winningSolution,
        CompetitionSolution secondWinningSolution,
        CompetitionSolution nonWinningSolution)
    {
        winningSolution.IsWinningSolution = false;
        winningSolution.Scores = new List<SolutionScore>
        {
            new(ScoreType.Price, 1, 0.3M),
            new(ScoreType.Features, 5, 2M),
            new(ScoreType.Implementation, 5, 1.5M),
            new(ScoreType.Interoperability, 5, 1.5M),
        };

        secondWinningSolution.IsWinningSolution = false;
        secondWinningSolution.Scores = new List<SolutionScore>
        {
            new(ScoreType.Price, 1, 0.3M),
            new(ScoreType.Features, 5, 2M),
            new(ScoreType.Implementation, 5, 1.5M),
            new(ScoreType.Interoperability, 5, 1.5M),
        };

        nonWinningSolution.IsWinningSolution = false;
        nonWinningSolution.Scores = new List<SolutionScore>
        {
            new(ScoreType.Price, 5, 1.5M),
            new(ScoreType.Features, 3, 1.2M),
            new(ScoreType.Implementation, 3, 0.9M),
            new(ScoreType.Interoperability, 3, 0.9M),
        };

        competition.Weightings = new() { Price = 30, NonPrice = 70 };
        competition.NonPriceElements = new()
        {
            NonPriceWeights = new() { Features = 40, Implementation = 30, Interoperability = 30 },
        };

        competition.CompetitionSolutions = new List<CompetitionSolution> { winningSolution, secondWinningSolution, nonWinningSolution };

        CompetitionsService.SetWinningSolution(competition);

        winningSolution.IsWinningSolution.Should().BeTrue();
        secondWinningSolution.IsWinningSolution.Should().BeTrue();
        nonWinningSolution.IsWinningSolution.Should().BeFalse();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetNonShortlistedSolutions_ReturnsExpected(
        Organisation organisation,
        Competition competition,
        List<Solution> solutions,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions = solutions.Select(
                x => new CompetitionSolution(competition.Id, x.CatalogueItemId) { IsShortlisted = true })
            .ToList();

        var expectedNonShortlistedSolutions = competition.CompetitionSolutions.Take(2).ToList();
        expectedNonShortlistedSolutions.ForEach(x => x.IsShortlisted = false);

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.Solutions.AddRange(solutions);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var nonShortlistedSolutions =
            await service.GetNonShortlistedSolutions(organisation.InternalIdentifier, competition.Id);

        nonShortlistedSolutions.Should().NotBeEmpty();
        nonShortlistedSolutions.Should()
            .BeEquivalentTo(
                expectedNonShortlistedSolutions,
                opt => opt
                    .Excluding(m => m.Competition)
                    .Excluding(m => m.CatalogueItem)
                    .Excluding(m => m.Services));
    }

    private static Organisation CommonOrganisationFactory(int customId = 0)
    {
        return new Organisation
        {
            Id = customId == 0 ? CommonCompetitionId : customId,
            InternalIdentifier = CommonOrganisationInternalIdentifier,
            ExternalIdentifier = CommonOrganisationExternalIdentifier,
            Name = "A Local ICB",
        };
    }

    private static Competition CommonCompetitionFactory(
        int customId = 0,
        int customOrganisationId = 0,
        ICollection<CompetitionSublocation> competitionSublocations = null)
    {
        return new Competition
        {
            Id = customId == 0 ? CommonCompetitionId : customId,
            OrganisationId = customOrganisationId == 0 ? CommonOrganisationId : customOrganisationId,
            Name = "My Competition",
            Description = "Competition for competitiony things",
            CompetitionSublocations = competitionSublocations is { Count: > 0 } ? competitionSublocations : [],
        };
    }

    private static CompetitionSublocation CommonCompetitionSublocationFactory(
        string sublocationOdsCode,
        List<CompetitionSublocationRecipient> sublocationRecipients = null,
        bool hasOrganisation = false,
        int customCompetitionId = 0)
    {
        return new CompetitionSublocation
        {
            CompetitionId = customCompetitionId == 0 ? CommonCompetitionId : customCompetitionId,
            SublocationOdsCode = sublocationOdsCode,
            OwnerOdsCode = CommonOrganisationExternalIdentifier,
            SublocationRecipients = sublocationRecipients,
            SublocationOrganisation = hasOrganisation ? CommonEntityOdsOrganisationFactory(sublocationOdsCode) : null,
        };
    }

    private static CompetitionSublocationRecipient CommonCompetitionSublocationRecipientFactory(
        string recipientOdsCode,
        string parentSublocationOdsCode,
        bool hasOrganisation = false)
    {
        return new CompetitionSublocationRecipient
        {
            CompetitionId = CommonCompetitionId,
            RecipientOdsCode = recipientOdsCode,
            ParentSublocationOdsCode = parentSublocationOdsCode,
            RecipientOrganisation = hasOrganisation ? CommonEntityOdsOrganisationFactory(recipientOdsCode) : null,
        };
    }

    private static EntityOdsOrganisation CommonEntityOdsOrganisationFactory(string id)
    {
        return new EntityOdsOrganisation { Id = id, Name = $"An organisation - {id}", IsActive = true };
    }

    private static ServiceContractOdsOrganisation CommonServiceContractOdsOrganisationFactory(string id)
    {
        return new ServiceContractOdsOrganisation
        {
            OdsCode = id, OrganisationName = $"An organisation - {id}", IsActive = true,
        };
    }

    private static ServiceRecipient CommonServiceRecipientFactory(string orgId, string locationOrgId)
    {
        return new ServiceRecipient { OrgId = orgId, LocationOrgId = locationOrgId };
    }
}
