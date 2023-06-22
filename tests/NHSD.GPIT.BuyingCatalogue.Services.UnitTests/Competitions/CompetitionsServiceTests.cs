using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
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

        var result = await service.GetCompetition(organisation.Id, competition.Id);

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

        var result = await service.ExistsAsync(organisation.Id, competition.Name);

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
    public static async Task GetCompetitionName(
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
}
