using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.Competitions;
using NHSD.GPIT.BuyingCatalogue.Services.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Competitions;

public static class CompetitionsServiceTests
{
    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetCompetitionCriteriaReview_ReturnsCompetition(
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

        var result = await service.GetCompetitionCriteriaReview(organisation.InternalIdentifier, competition.Id);

        result.Should().BeEquivalentTo(competition, opt => opt.Excluding(x => x.Organisation));
    }

    [Theory]
    [InMemoryDbAutoData]
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
                x.Organisation = organisation;
                x.IsDeleted = false;
            });

        context.Organisations.Add(organisation);
        context.Competitions.AddRange(competitions);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var result = await service.GetCompetitionsDashboard(organisation.Id);

        result.Should()
            .BeEquivalentTo(
                competitions,
                opt => opt.Excluding(x => x.Organisation).Excluding(x => x.LastUpdatedByUser).Excluding(x => x.Filter));
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task AddCompetition_AddsCompetition(
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

        await service.AddCompetition(organisation.Id, filter.Id, name, description);

        context.Competitions.Should()
            .Contain(
                x => x.Name == name && x.Description == description && x.OrganisationId == organisation.Id
                    && x.FilterId == filter.Id);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetCompetitionWithServices_ReturnsCompetition(
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

        var result = await service.GetCompetitionWithServices(organisation.Id, competition.Id);

        result.Should().BeEquivalentTo(competition, opt => opt.Excluding(x => x.Organisation));
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetCompetition_ReturnsExpected(
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

        var result = await service.GetCompetition(organisation.InternalIdentifier, competition.Id);

        result.Should().BeEquivalentTo(competition, opt => opt.Excluding(x => x.Organisation));
    }

    [Theory]
    [InMemoryDbAutoData]
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

        await service.AddCompetitionSolutions(organisation.Id, competition.Id, competitionSolutions);

        var updatedCompetition = await context.Competitions.AsNoTracking()
            .IgnoreQueryFilters()
            .Include(x => x.CompetitionSolutions)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.CompetitionSolutions.Select(x => x.SolutionId)
            .Should()
            .BeEquivalentTo(competitionSolutions.Select(x => x.SolutionId));
    }

    [Theory]
    [InMemoryDbAutoData]
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

        await service.SetShortlistedSolutions(organisation.Id, competition.Id, shortlisted);

        var updatedCompetition = await context.Competitions.AsNoTracking()
            .IgnoreQueryFilters()
            .Include(x => x.CompetitionSolutions)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        var actualShortlistedSolutions = updatedCompetition.CompetitionSolutions.Where(x => x.IsShortlisted)
            .Select(x => x.SolutionId);
        var actualNonShortlistedSolutions = updatedCompetition.CompetitionSolutions.Where(x => !x.IsShortlisted)
            .Select(x => x.SolutionId);

        actualShortlistedSolutions.Should().BeEquivalentTo(shortlisted);
        actualNonShortlistedSolutions.Should().BeEquivalentTo(nonShortlisted);
    }

    [Theory]
    [InMemoryDbAutoData]
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

        await service.SetShortlistedSolutions(organisation.Id, competition.Id, shortlisted);

        var updatedCompetition = await context.Competitions.AsNoTracking()
            .IgnoreQueryFilters()
            .Include(x => x.CompetitionSolutions)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        var actualShortlistedSolutions = updatedCompetition.CompetitionSolutions.Where(x => x.IsShortlisted)
            .Select(x => x.SolutionId);
        var actualNonShortlistedSolutions = updatedCompetition.CompetitionSolutions.Where(x => !x.IsShortlisted)
            .Select(x => x.SolutionId);

        actualShortlistedSolutions.Should().BeEquivalentTo(shortlisted);
        actualNonShortlistedSolutions.Should().Contain(x => x == previouslyShortlistedSolution);
    }

    [Theory]
    [InMemoryDbAutoData]
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
            organisation.Id,
            competition.Id,
            solutionIdsJustifications);

        var updatedCompetition = await context.Competitions.AsNoTracking()
            .IgnoreQueryFilters()
            .Include(x => x.CompetitionSolutions)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.CompetitionSolutions.Should()
            .Contain(x => x.Justification == solutionIdsJustifications[x.SolutionId]);
    }

    [Theory]
    [InMemoryDbAutoData]
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
            organisation.Id,
            competition.Id,
            solutionIdsJustifications);

        var updatedCompetition = await context.Competitions.AsNoTracking()
            .IgnoreQueryFilters()
            .Include(x => x.CompetitionSolutions)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.CompetitionSolutions.Should()
            .Contain(x => x.SolutionId == previouslyJustified.CatalogueItemId && string.IsNullOrEmpty(x.Justification));
    }

    [Theory]
    [InMemoryDbAutoData]
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

        await service.AcceptShortlist(organisation.Id, competition.Id);

        var updatedCompetition =
            await context.Competitions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.ShortlistAccepted.Should().NotBeNull();
    }

    [Theory]
    [InMemoryDbAutoData]
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

        await service.AcceptShortlist(organisation.Id, competition.Id);

        var refreshedCompetition =
            await context.Competitions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == competition.Id);

        refreshedCompetition.ShortlistAccepted.Should().Be(acceptedDate);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task CompleteCompetition_SetsCompletionDate(
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

        await service.CompleteCompetition(organisation.Id, competition.Id);

        var updatedCompetition =
            await context.Competitions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.Completed.Should().NotBeNull();
    }

    [Theory]
    [InMemoryDbAutoData]
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

        await service.CompleteCompetition(organisation.Id, competition.Id);

        var refreshedCompetition =
            await context.Competitions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == competition.Id);

        refreshedCompetition.Completed.Should().Be(completedDate);
    }

    [Theory]
    [InMemoryDbAutoData]
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

        await service.DeleteCompetition(organisation.Id, competition.Id);

        var updatedCompetition =
            await context.Competitions.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.IsDeleted.Should().BeTrue();
    }

    [Theory]
    [InMemoryDbAutoData]
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

        await service.DeleteCompetition(organisation.Id, competition.Id);

        var refreshedCompetition =
            await context.Competitions.IgnoreQueryFilters().AsNoTracking().FirstOrDefaultAsync(x => x.Id == competition.Id);

        refreshedCompetition.IsDeleted.Should().BeTrue();
    }

    [Theory]
    [InMemoryDbAutoData]
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

        var result = await service.Exists(organisation.Id, competition.Name);

        result.Should().BeTrue();
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetCompetitionWithRecipients_ReturnsExpected(
        Organisation organisation,
        Competition competition,
        List<OdsOrganisation> odsOrganisations,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.Recipients = odsOrganisations;

        context.Competitions.Add(competition);
        context.Organisations.Add(organisation);
        context.OdsOrganisations.AddRange(odsOrganisations);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var competitionWithRecipients = await service.GetCompetitionWithRecipients(organisation.Id, competition.Id);

        competitionWithRecipients.Should()
            .BeEquivalentTo(competition, opt => opt.Excluding(m => m.Organisation).Excluding(m => m.Recipients));
        competitionWithRecipients.Recipients.Should()
            .BeEquivalentTo(odsOrganisations, opt => opt.Excluding(m => m.Roles).Excluding(m => m.Related).Excluding(m => m.Parents));
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetCompetitionRecipients_AddsNewRecipients(
        Organisation organisation,
        Competition competition,
        List<OdsOrganisation> odsOrganisations,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;

        context.Competitions.Add(competition);
        context.Organisations.Add(organisation);
        context.OdsOrganisations.AddRange(odsOrganisations);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        await service.SetCompetitionRecipients(competition.Id, odsOrganisations.Select(x => x.Id));

        var updatedCompetition = await context.Competitions.AsNoTracking().Include(x => x.Recipients)
            .FirstOrDefaultAsync(x => x.OrganisationId == organisation.Id && x.Id == competition.Id);

        updatedCompetition.Recipients.Should()
            .BeEquivalentTo(
                odsOrganisations,
                opt => opt.Excluding(m => m.Roles).Excluding(m => m.Related).Excluding(m => m.Parents));
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetCompetitionRecipients_RemovesStaleRecipients(
        Organisation organisation,
        Competition competition,
        List<OdsOrganisation> odsOrganisations,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.Recipients = odsOrganisations;

        context.Competitions.Add(competition);
        context.Organisations.Add(organisation);
        context.OdsOrganisations.AddRange(odsOrganisations);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var selectedRecipients = odsOrganisations.Skip(2).ToList();
        var staleRecipients = odsOrganisations.Except(selectedRecipients).ToList();

        await service.SetCompetitionRecipients(competition.Id, selectedRecipients.Select(x => x.Id));

        var updatedCompetition = await context.Competitions.AsNoTracking().Include(x => x.Recipients)
            .FirstOrDefaultAsync(x => x.OrganisationId == organisation.Id && x.Id == competition.Id);

        selectedRecipients.ForEach(x => updatedCompetition.Recipients.Should().Contain(y => y.Id == x.Id));
        staleRecipients.ForEach(x => updatedCompetition.Recipients.Should().NotContain(y => y.Id == x.Id));
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetCompetitionTaskList_ReturnsExpected(
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

        var expectedModel = new CompetitionTaskListModel(competition);

        var result = await service.GetCompetitionTaskList(organisation.Id, competition.Id);

        result.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [InMemoryDbAutoData]
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

        var result = await service.GetCompetitionName(organisation.Id, competition.Id);

        result.Should().Be(competition.Name);
    }

    [Theory]
    [InMemoryDbAutoData]
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

        await service.SetContractLength(organisation.Id, competition.Id, contractLength);

        var updatedCompetition = await context.Competitions.FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.ContractLength.Should().Be(contractLength);
    }

    [Theory]
    [InMemoryDbAutoData]
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

        await service.SetCompetitionWeightings(organisation.Id, competition.Id, priceWeighting, nonPriceWeighting);

        var updatedCompetition = await context.Competitions.Include(x => x.Weightings)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        updatedCompetition.Weightings.Should().NotBeNull();
        updatedCompetition.Weightings.Price.Should().Be(priceWeighting);
        updatedCompetition.Weightings.NonPrice.Should().Be(nonPriceWeighting);
    }

    [Theory]
    [InMemoryDbAutoData]
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

        var updatedCompetition = await service.GetCompetitionWithWeightings(organisation.Id, competition.Id);

        updatedCompetition.Weightings.Should().BeEquivalentTo(weightings, opt => opt.Excluding(m => m.CompetitionId));
    }

    [Theory]
    [InMemoryDbAutoData]
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
    [InMemoryDbAutoData]
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
    [InMemoryDbAutoData]
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
    [InMemoryDbAutoData]
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
    [InMemoryDbAutoData]
    public static async Task SetServiceLevelCriteria_NullNonPriceElements_CreatesNonPriceElements(
        string applicableDays,
        DateTime timeFrom,
        DateTime timeUntil,
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
            applicableDays);

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        updatedCompetition.NonPriceElements.Should().NotBeNull();
        updatedCompetition.NonPriceElements.ServiceLevel.Should().NotBeNull();
        updatedCompetition.NonPriceElements.ServiceLevel.ApplicableDays.Should().Be(applicableDays);
        updatedCompetition.NonPriceElements.ServiceLevel.TimeFrom.Should().Be(timeFrom);
        updatedCompetition.NonPriceElements.ServiceLevel.TimeUntil.Should().Be(timeUntil);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetServiceLevelCriteria_NullServiceLevelCriteria_CreatesServiceLevelCriteria(
        string applicableDays,
        DateTime timeFrom,
        DateTime timeUntil,
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
            applicableDays);

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        updatedCompetition.NonPriceElements.Should().NotBeNull();
        updatedCompetition.NonPriceElements.ServiceLevel.Should().NotBeNull();
        updatedCompetition.NonPriceElements.ServiceLevel.ApplicableDays.Should().Be(applicableDays);
        updatedCompetition.NonPriceElements.ServiceLevel.TimeFrom.Should().Be(timeFrom);
        updatedCompetition.NonPriceElements.ServiceLevel.TimeUntil.Should().Be(timeUntil);
    }

    [Theory]
    [InMemoryDbAutoData]
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

        await service.SetServiceLevelCriteria(
            organisation.InternalIdentifier,
            competition.Id,
            newServiceLevelCriteria.TimeFrom,
            newServiceLevelCriteria.TimeUntil,
            newServiceLevelCriteria.ApplicableDays);

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        updatedCompetition.NonPriceElements.Should().NotBeNull();
        updatedCompetition.NonPriceElements.ServiceLevel.Should().NotBeNull();
        updatedCompetition.NonPriceElements.ServiceLevel.Should()
            .BeEquivalentTo(newServiceLevelCriteria, opt => opt.Excluding(m => m.Id));
    }

    [Theory]
    [InMemoryDbAutoData]
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

        await service.SetServiceLevelCriteria(
            organisation.InternalIdentifier,
            competition.Id,
            newServiceLevelCriteria.TimeFrom,
            newServiceLevelCriteria.TimeUntil,
            newServiceLevelCriteria.ApplicableDays);

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
    [InMemoryDbAutoData]
    public static async Task SetInteroperabilityCriteria_AddsIntegrations(
        Organisation organisation,
        Competition competition,
        List<string> im1Integrations,
        List<string> gpConnectIntegrations,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.SetInteroperabilityCriteria(organisation.InternalIdentifier, competition.Id, im1Integrations, gpConnectIntegrations);

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        var integrations = im1Integrations.Concat(gpConnectIntegrations).ToList();

        integrations.ForEach(
            x => updatedCompetition.NonPriceElements.Interoperability.Should().Contain(y => x == y.Qualifier));
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetInteroperabilityCriteria_StaleIntegrations_RemovesStaleIntegrations(
        Organisation organisation,
        Competition competition,
        List<string> im1Integrations,
        List<string> gpConnectIntegrations,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        var staleIntegration = im1Integrations.First();

        competition.NonPriceElements = new()
        {
            Interoperability = new List<InteroperabilityCriteria>()
            {
                new() { IntegrationType = InteropIntegrationType.Im1, Qualifier = staleIntegration },
            },
        };

        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.SetInteroperabilityCriteria(organisation.InternalIdentifier, competition.Id, im1Integrations.Skip(1), gpConnectIntegrations);

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        var integrations = im1Integrations.Skip(1).Concat(gpConnectIntegrations).ToList();

        updatedCompetition.NonPriceElements.Interoperability.Should().NotContain(x => x.Qualifier == staleIntegration);
        integrations.ForEach(
            x => updatedCompetition.NonPriceElements.Interoperability.Should().Contain(y => x == y.Qualifier));
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetInteroperabilityCriteria_WithInteroperabilityScore_RemovesScores(
        Organisation organisation,
        Competition competition,
        Solution solution,
        List<string> im1Integrations,
        List<string> gpConnectIntegrations,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsService service)
    {
        var staleIntegration = im1Integrations.First();

        competition.CompetitionSolutions = new List<CompetitionSolution>
        {
            new(competition.Id, solution.CatalogueItemId)
            {
                Scores = new List<SolutionScore> { new(ScoreType.Interoperability, 3), },
            },
        };

        competition.NonPriceElements = new()
        {
            Interoperability = new List<InteroperabilityCriteria>()
            {
                new() { IntegrationType = InteropIntegrationType.Im1, Qualifier = staleIntegration },
            },
        };

        competition.OrganisationId = organisation.Id;

        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.SetInteroperabilityCriteria(
            organisation.InternalIdentifier,
            competition.Id,
            im1Integrations.Skip(1),
            gpConnectIntegrations);

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        var integrations = im1Integrations.Skip(1).Concat(gpConnectIntegrations).ToList();

        updatedCompetition.CompetitionSolutions.Should().NotContain(x => x.HasScoreType(ScoreType.Interoperability));
        updatedCompetition.NonPriceElements.Interoperability.Should().NotContain(x => x.Qualifier == staleIntegration);
        integrations.ForEach(
            x => updatedCompetition.NonPriceElements.Interoperability.Should().Contain(y => x == y.Qualifier));
    }

    [Theory]
    [InMemoryDbAutoData]
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
    [InMemoryDbAutoData]
    public static async Task SetNonPriceWeights_NullNonPriceWeights_CreatesAndSetsWeights(
        int implementationWeight,
        int interoperabilityWeight,
        int serviceLevelWeight,
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
            serviceLevelWeight);

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        var nonPriceElementWeightings = updatedCompetition.NonPriceElements.NonPriceWeights;

        nonPriceElementWeightings.Should().NotBeNull();
        nonPriceElementWeightings.Implementation.Should().Be(implementationWeight);
        nonPriceElementWeightings.Interoperability.Should().Be(interoperabilityWeight);
        nonPriceElementWeightings.ServiceLevel.Should().Be(serviceLevelWeight);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetNonPriceWeights_ExistingNonPriceWeights_SetsWeights(
        int implementationWeight,
        int interoperabilityWeight,
        int serviceLevelWeight,
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
            serviceLevelWeight);

        var updatedCompetition = await service.GetCompetitionWithNonPriceElements(
            organisation.InternalIdentifier,
            competition.Id);

        var nonPriceElementWeightings = updatedCompetition.NonPriceElements.NonPriceWeights;

        nonPriceElementWeightings.Should().NotBeNull();
        nonPriceElementWeightings.Implementation.Should().Be(implementationWeight);
        nonPriceElementWeightings.Interoperability.Should().Be(interoperabilityWeight);
        nonPriceElementWeightings.ServiceLevel.Should().Be(serviceLevelWeight);
    }

    [Theory]
    [CommonAutoData]
    public static Task SetSolutionsImplementationScores_NullSolutionsScores_ThrowsArgumentNullException(
        string internalOrgId,
        int competitionId,
        CompetitionsService service)
        => FluentActions
            .Awaiting(() => service.SetSolutionsImplementationScores(internalOrgId, competitionId, null))
            .Should()
            .ThrowAsync<ArgumentNullException>();

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetSolutionsImplementationScores_ExistingScore_UpdatesScore(
        Organisation organisation,
        Competition competition,
        Solution solution,
        int score,
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
            new Dictionary<CatalogueItemId, int> { { competitionSolution.SolutionId, score } });

        var updatedCompetition =
            await service.GetCompetitionWithSolutions(organisation.InternalIdentifier, competition.Id);
        var updatedSolution =
            updatedCompetition.CompetitionSolutions.First(x => x.SolutionId == competitionSolution.SolutionId);

        updatedSolution.HasScoreType(scoreType).Should().BeTrue();
        updatedSolution.GetScoreByType(scoreType).Score.Should().Be(score);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetSolutionsImplementationScores_NewScore_AddsScore(
        Organisation organisation,
        Competition competition,
        Solution solution,
        int score,
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
            new Dictionary<CatalogueItemId, int> { { competitionSolution.SolutionId, score } });

        var updatedCompetition =
            await service.GetCompetitionWithSolutions(organisation.InternalIdentifier, competition.Id);
        var updatedSolution =
            updatedCompetition.CompetitionSolutions.First(x => x.SolutionId == competitionSolution.SolutionId);

        updatedSolution.HasScoreType(scoreType).Should().BeTrue();
        updatedSolution.GetScoreByType(scoreType).Score.Should().Be(score);
    }

    [Theory]
    [CommonAutoData]
    public static Task SetSolutionsInteroperabilityScores_NullSolutionsScores_ThrowsArgumentNullException(
        string internalOrgId,
        int competitionId,
        CompetitionsService service)
        => FluentActions
            .Awaiting(() => service.SetSolutionsInteroperabilityScores(internalOrgId, competitionId, null))
            .Should()
            .ThrowAsync<ArgumentNullException>();

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetSolutionsInteroperabilityScores_ExistingScore_UpdatesScore(
        Organisation organisation,
        Competition competition,
        Solution solution,
        int score,
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
            new Dictionary<CatalogueItemId, int> { { competitionSolution.SolutionId, score } });

        var updatedCompetition =
            await service.GetCompetitionWithSolutions(organisation.InternalIdentifier, competition.Id);
        var updatedSolution =
            updatedCompetition.CompetitionSolutions.First(x => x.SolutionId == competitionSolution.SolutionId);

        updatedSolution.HasScoreType(scoreType).Should().BeTrue();
        updatedSolution.GetScoreByType(scoreType).Score.Should().Be(score);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetSolutionsInteroperabilityScores_NewScore_AddsScore(
        Organisation organisation,
        Competition competition,
        Solution solution,
        int score,
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
            new Dictionary<CatalogueItemId, int> { { competitionSolution.SolutionId, score } });

        var updatedCompetition =
            await service.GetCompetitionWithSolutions(organisation.InternalIdentifier, competition.Id);
        var updatedSolution =
            updatedCompetition.CompetitionSolutions.First(x => x.SolutionId == competitionSolution.SolutionId);

        updatedSolution.HasScoreType(scoreType).Should().BeTrue();
        updatedSolution.GetScoreByType(scoreType).Score.Should().Be(score);
    }

    [Theory]
    [CommonAutoData]
    public static Task SetSolutionsServiceLevelScores_NullSolutionsScores_ThrowsArgumentNullException(
        string internalOrgId,
        int competitionId,
        CompetitionsService service)
        => FluentActions
            .Awaiting(() => service.SetSolutionsServiceLevelScores(internalOrgId, competitionId, null))
            .Should()
            .ThrowAsync<ArgumentNullException>();

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetSolutionsServiceLevelScores_ExistingScore_UpdatesScore(
        Organisation organisation,
        Competition competition,
        Solution solution,
        int score,
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
            new Dictionary<CatalogueItemId, int> { { competitionSolution.SolutionId, score } });

        var updatedCompetition =
            await service.GetCompetitionWithSolutions(organisation.InternalIdentifier, competition.Id);
        var updatedSolution =
            updatedCompetition.CompetitionSolutions.First(x => x.SolutionId == competitionSolution.SolutionId);

        updatedSolution.HasScoreType(scoreType).Should().BeTrue();
        updatedSolution.GetScoreByType(scoreType).Score.Should().Be(score);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetSolutionsServiceLevelScores_NewScore_AddsScore(
        Organisation organisation,
        Competition competition,
        Solution solution,
        int score,
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
            new Dictionary<CatalogueItemId, int> { { competitionSolution.SolutionId, score } });

        var updatedCompetition =
            await service.GetCompetitionWithSolutions(organisation.InternalIdentifier, competition.Id);
        var updatedSolution =
            updatedCompetition.CompetitionSolutions.First(x => x.SolutionId == competitionSolution.SolutionId);

        updatedSolution.HasScoreType(scoreType).Should().BeTrue();
        updatedSolution.GetScoreByType(scoreType).Score.Should().Be(score);
    }
}
