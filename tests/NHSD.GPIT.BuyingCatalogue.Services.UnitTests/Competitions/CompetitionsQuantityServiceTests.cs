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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.Services.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Competitions;

public static class CompetitionsQuantityServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionsQuantityService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetSolutionRecipientQuantity_SetsQuantity(
        Organisation organisation,
        Competition competition,
        Solution solution,
        OdsOrganisation sublocation,
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
            odsOrganisations.Select(x => new ServiceRecipientQuantityDto(sublocation.Id, x.Id, x.Name, quantity)));

        var updatedCompetition = await context.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Quantities)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        var updatedSolution = updatedCompetition.CompetitionSolutions.First(x => x.CatalogueItemId == solution.CatalogueItemId);

        updatedSolution.Quantities.Should().HaveCount(odsOrganisations.Count);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetServiceRecipientQuantity_SetsQuantity(
        Organisation organisation,
        Competition competition,
        Solution solution,
        AdditionalService additionalService,
        OdsOrganisation sublocation,
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
                Services = new List<CompetitionCatalogueItem>
                {
                    new CompetitionAdditionalService(competition.Id, additionalService.CatalogueItemId, true),
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
            odsOrganisations.Select(x => new ServiceRecipientQuantityDto(sublocation.Id, x.Id, x.Name, quantity)));

        var updatedCompetition = await context.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Services)
            .ThenInclude(x => x.Quantities)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        var updatedSolution = updatedCompetition.CompetitionSolutions.First(x => x.CatalogueItemId == solution.CatalogueItemId);

        var updatedService = updatedSolution.Services.First(x => x.CatalogueItemId == additionalService.CatalogueItemId);

        updatedService.Quantities.Should().HaveCount(odsOrganisations.Count);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetAdditionalServiceAssociatedServiceQuantity_SetsQuantity(
        Organisation organisation,
        Competition competition,
        Solution solution,
        AdditionalService additionalService,
        AssociatedService associatedService,
        OdsOrganisation sublocation,
        List<OdsOrganisation> odsOrganisations,
        int quantity,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsQuantityService service)
    {
        int competitionAdditionalServiceId = 101;

        var competitionAssociatedService = new CompetitionAssociatedService(competition.Id, associatedService.CatalogueItemId)
        {
            CatalogueItem = associatedService.CatalogueItem,
            ParentItemId = competitionAdditionalServiceId,
        };

        var competitionAdditionalService = new CompetitionAdditionalService(competition.Id, additionalService.CatalogueItemId, true)
        {
            Id = competitionAdditionalServiceId,
            CatalogueItem = additionalService.CatalogueItem,
            AssociatedServices = [competitionAssociatedService],
        };

        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions.Add(
            new CompetitionSolution(competition.Id, solution.CatalogueItemId)
            {
                IsShortlisted = true,
                Services = [competitionAdditionalService],
            });

        context.OdsOrganisations.AddRange(odsOrganisations);
        context.AdditionalServices.Add(additionalService);
        context.Solutions.Add(solution);
        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.CompetitionCatalogueItems.Add(competitionAssociatedService);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.SetAdditionalServiceAssociatedServiceQuantity(
            organisation.InternalIdentifier,
            competition.Id,
            solution.CatalogueItemId,
            additionalService.CatalogueItemId,
            associatedService.CatalogueItemId,
            odsOrganisations.Select(x => new ServiceRecipientQuantityDto(sublocation.Id, x.Id, x.Name, quantity)));

        var updatedAssociatedService = await context.CompetitionCatalogueItems
             .Include(x => x.Price)
             .ThenInclude(x => x.Tiers)
             .FirstOrDefaultAsync(x => x.CompetitionId == competition.Id &&
                 x.CatalogueItemId == associatedService.CatalogueItemId &&
                 x.ParentItemId == competitionAdditionalServiceId);

        updatedAssociatedService.Quantities.Should().HaveCount(odsOrganisations.Count);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task ResetSolutionQuantities_Resets(
        Organisation organisation,
        Competition competition,
        Solution solution,
        OdsOrganisation sublocation,
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
                Quantities = odsOrganisations.Select(x => new CompetitionItemQuantity()
                    {
                        CompetitionId = competition.Id,
                        ParentSublocationOdsCode = sublocation.Id,
                        RecipientOdsCode = x.Id,
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

        var updatedSolution = updatedCompetition.CompetitionSolutions.First(x => x.CatalogueItemId == solution.CatalogueItemId);

        updatedSolution.Quantities.Should().BeNullOrEmpty();
        updatedSolution.Quantity.Should().BeNull();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task ResetServiceQuantities_Resets(
        Organisation organisation,
        Competition competition,
        Solution solution,
        AdditionalService additionalService,
        OdsOrganisation sublocation,
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
                Services = new List<CompetitionCatalogueItem>
                {
                    new CompetitionAdditionalService(competition.Id, additionalService.CatalogueItemId, true)
                    {
                        Quantity = quantity,
                        Quantities = odsOrganisations.Select(x => new CompetitionItemQuantity()
                            {
                                CompetitionId = competition.Id,
                                ParentSublocationOdsCode = sublocation.Id,
                                RecipientOdsCode = x.Id,
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
            .ThenInclude(x => x.Services)
            .ThenInclude(x => x.Quantities)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        var updatedSolution = updatedCompetition.CompetitionSolutions.First(x => x.CatalogueItemId == solution.CatalogueItemId);

        var updatedService = updatedSolution.Services.First(x => x.CatalogueItemId == additionalService.CatalogueItemId);

        updatedService.Quantities.Should().BeNullOrEmpty();
        updatedService.Quantity.Should().BeNull();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task ResetAdditionalServiceAssociatedServiceQuantities_Resets(
        Organisation organisation,
        Competition competition,
        Solution solution,
        AdditionalService additionalService,
        AssociatedService associatedService,
        OdsOrganisation sublocation,
        List<OdsOrganisation> odsOrganisations,
        int quantity,
        [Frozen] BuyingCatalogueDbContext context,
        CompetitionsQuantityService service)
    {
        int competitionAdditionalServiceId = 1001;

        var competitionAssociatedService = new CompetitionAssociatedService(competition.Id, associatedService.CatalogueItemId)
        {
            CatalogueItem = associatedService.CatalogueItem,
            ParentItemId = competitionAdditionalServiceId,
        };

        var competitionAdditionalService = new CompetitionAdditionalService(competition.Id, additionalService.CatalogueItemId, true)
        {
            Id = competitionAdditionalServiceId,
            CatalogueItem = additionalService.CatalogueItem,
            AssociatedServices = [competitionAssociatedService],
            Quantity = quantity,
            Quantities = [.. odsOrganisations.Select(x => new CompetitionItemQuantity()
                {
                    CompetitionId = competition.Id,
                    ParentSublocationOdsCode = sublocation.Id,
                    RecipientOdsCode = x.Id,
                    Quantity = quantity,
                })],
        };

        competition.OrganisationId = organisation.Id;
        competition.CompetitionSolutions.Add(
            new CompetitionSolution(competition.Id, solution.CatalogueItemId)
            {
                IsShortlisted = true,
                Services = [competitionAdditionalService],
            });

        context.Solutions.Add(solution);
        context.Organisations.Add(organisation);
        context.Competitions.Add(competition);
        context.CompetitionCatalogueItems.Add(competitionAssociatedService);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.ResetAdditionalServiceAssociatedServiceQuantities(
            organisation.InternalIdentifier,
            competition.Id,
            solution.CatalogueItemId,
            additionalService.CatalogueItemId,
            associatedService.CatalogueItemId);

        var updatedCompetition = await context.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Services)
            .ThenInclude(x => x.Quantities)
            .FirstOrDefaultAsync(x => x.Id == competition.Id);

        var updatedAssociatedService = await context.CompetitionCatalogueItems
             .Include(x => x.Price)
             .ThenInclude(x => x.Tiers)
             .FirstOrDefaultAsync(x => x.CompetitionId == competition.Id &&
                 x.CatalogueItemId == associatedService.CatalogueItemId &&
                 x.ParentItemId == competitionAdditionalServiceId);

        updatedAssociatedService.Quantities.Should().BeNullOrEmpty();
        updatedAssociatedService.Quantity.Should().BeNull();
    }
}
