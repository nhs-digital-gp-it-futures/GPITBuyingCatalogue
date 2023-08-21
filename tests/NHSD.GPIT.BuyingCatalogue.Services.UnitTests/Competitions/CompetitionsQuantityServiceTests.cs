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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.Services.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Competitions;

public static class CompetitionsQuantityServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionsQuantityService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetSolutionGlobalQuantity_SetsQuantity(
        Organisation organisation,
        Competition competition,
        Solution solution,
        int quantity,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsQuantityService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions.Add(new CompetitionSolution(competition.Id, solution.CatalogueItemId) { IsShortlisted = true });

        context.Solutions.Add(solution);
        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        await service.SetSolutionGlobalQuantity(
            organisation.InternalIdentifier,
            competition.Id,
            solution.CatalogueItemId,
            quantity);

        var updatedCompetition = await context.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Quantities)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        var updatedSolution = updatedCompetition.CompetitionSolutions.First(x => x.SolutionId == solution.CatalogueItemId);

        updatedSolution.Quantity.Should().Be(quantity);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetSolutionRecipientQuantity_SetsQuantity(
        Organisation organisation,
        Competition competition,
        Solution solution,
        List<OdsOrganisation> odsOrganisations,
        int quantity,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsQuantityService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions.Add(new CompetitionSolution(competition.Id, solution.CatalogueItemId) { IsShortlisted = true });

        context.OdsOrganisations.AddRange(odsOrganisations);
        context.Solutions.Add(solution);
        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        await service.SetSolutionRecipientQuantity(
            organisation.InternalIdentifier,
            competition.Id,
            solution.CatalogueItemId,
            odsOrganisations.Select(x => new ServiceRecipientDto(x.Id, x.Name, quantity)));

        var updatedCompetition = await context.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Quantities)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        var updatedSolution = updatedCompetition.CompetitionSolutions.First(x => x.SolutionId == solution.CatalogueItemId);

        updatedSolution.Quantities.Should().HaveCount(odsOrganisations.Count);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetServiceGlobalQuantity_SetsQuantity(
        Organisation organisation,
        Competition competition,
        Solution solution,
        AdditionalService additionalService,
        int quantity,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsQuantityService service)
    {
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

        await service.SetServiceGlobalQuantity(
            organisation.InternalIdentifier,
            competition.Id,
            solution.CatalogueItemId,
            additionalService.CatalogueItemId,
            quantity);

        var updatedCompetition = await context.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices)
            .ThenInclude(x => x.Quantities)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        var updatedSolution = updatedCompetition.CompetitionSolutions.First(x => x.SolutionId == solution.CatalogueItemId);

        var updatedService = updatedSolution.SolutionServices.First(x => x.ServiceId == additionalService.CatalogueItemId);

        updatedService.Quantity.Should().Be(quantity);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task SetServiceRecipientQuantity_SetsQuantity(
        Organisation organisation,
        Competition competition,
        Solution solution,
        AdditionalService additionalService,
        List<OdsOrganisation> odsOrganisations,
        int quantity,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsQuantityService service)
    {
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

        context.OdsOrganisations.AddRange(odsOrganisations);
        context.AdditionalServices.Add(additionalService);
        context.Solutions.Add(solution);
        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        await service.SetServiceRecipientQuantity(
            organisation.InternalIdentifier,
            competition.Id,
            solution.CatalogueItemId,
            additionalService.CatalogueItemId,
            odsOrganisations.Select(x => new ServiceRecipientDto(x.Id, x.Name, quantity)));

        var updatedCompetition = await context.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices)
            .ThenInclude(x => x.Quantities)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        var updatedSolution = updatedCompetition.CompetitionSolutions.First(x => x.SolutionId == solution.CatalogueItemId);

        var updatedService = updatedSolution.SolutionServices.First(x => x.ServiceId == additionalService.CatalogueItemId);

        updatedService.Quantities.Should().HaveCount(odsOrganisations.Count);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task ResetSolutionQuantities_Resets(
        Organisation organisation,
        Competition competition,
        Solution solution,
        List<OdsOrganisation> odsOrganisations,
        int quantity,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsQuantityService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions.Add(
            new CompetitionSolution(competition.Id, solution.CatalogueItemId)
            {
                IsShortlisted = true,
                Quantity = quantity,
                Quantities = odsOrganisations.Select(
                        x => new SolutionQuantity
                        {
                            CompetitionId = competition.Id,
                            SolutionId = solution.CatalogueItemId,
                            OdsCode = x.Id,
                            Quantity = quantity,
                        })
                    .ToList(),
            });

        context.Solutions.Add(solution);
        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        await service.ResetSolutionQuantities(
            organisation.InternalIdentifier,
            competition.Id,
            solution.CatalogueItemId);

        var updatedCompetition = await context.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Quantities)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        var updatedSolution = updatedCompetition.CompetitionSolutions.First(x => x.SolutionId == solution.CatalogueItemId);

        updatedSolution.Quantities.Should().BeNullOrEmpty();
        updatedSolution.Quantity.Should().BeNull();
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task ResetServiceQuantities_Resets(
        Organisation organisation,
        Competition competition,
        Solution solution,
        AdditionalService additionalService,
        List<OdsOrganisation> odsOrganisations,
        int quantity,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsQuantityService service)
    {
        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions.Add(
            new CompetitionSolution(competition.Id, solution.CatalogueItemId)
            {
                IsShortlisted = true,

                SolutionServices = new List<SolutionService>
                {
                    new(competition.Id, solution.CatalogueItemId, additionalService.CatalogueItemId, true)
                    {
                        Quantity = quantity,
                        Quantities = odsOrganisations.Select(
                                x => new ServiceQuantity
                                {
                                    CompetitionId = competition.Id,
                                    SolutionId = solution.CatalogueItemId,
                                    ServiceId = additionalService.CatalogueItemId,
                                    OdsCode = x.Id,
                                    Quantity = quantity,
                                })
                            .ToList(),
                    },
                },
            });

        context.Solutions.Add(solution);
        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);

        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        await service.ResetServiceQuantities(
            organisation.InternalIdentifier,
            competition.Id,
            solution.CatalogueItemId,
            additionalService.CatalogueItemId);

        var updatedCompetition = await context.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices)
            .ThenInclude(x => x.Quantities)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        var updatedSolution = updatedCompetition.CompetitionSolutions.First(x => x.SolutionId == solution.CatalogueItemId);

        var updatedService = updatedSolution.SolutionServices.First(x => x.ServiceId == additionalService.CatalogueItemId);

        updatedService.Quantities.Should().BeNullOrEmpty();
        updatedService.Quantity.Should().BeNull();
    }
}
