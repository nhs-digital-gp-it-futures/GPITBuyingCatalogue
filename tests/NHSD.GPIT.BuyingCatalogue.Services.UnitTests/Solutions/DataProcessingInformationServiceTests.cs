using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.DataProcessingInformationModels;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Solutions;

public static class DataProcessingInformationServiceTests
{
    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetSolutionWithDataProcessingInformation_ReturnsSolution(
        Solution solution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        DataProcessingInformationService service)
    {
        dbContext.Solutions.Add(solution);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = await service.GetSolutionWithDataProcessingInformation(solution.CatalogueItemId);

        result.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static Task SetDataProcessingInformation_NullModel_ThrowsArgumentNullException(
        CatalogueItemId solutionId,
        DataProcessingInformationService service) => FluentActions
        .Awaiting(() => service.SetDataProcessingInformation(solutionId, null))
        .Should()
        .ThrowAsync<ArgumentNullException>();

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetDataProcessingInformation_NullInformation_SetsInformation(
        SetDataProcessingInformationModel model,
        Solution solution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        DataProcessingInformationService service)
    {
        solution.DataProcessingInformation = null;

        dbContext.Solutions.Add(solution);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        await service.SetDataProcessingInformation(solution.CatalogueItemId, model);

        var updatedSolution = await service.GetSolutionWithDataProcessingInformation(solution.CatalogueItemId);

        updatedSolution.DataProcessingInformation.Should().NotBeNull();
        updatedSolution.DataProcessingInformation.Details.Should().NotBeNull();
        updatedSolution.DataProcessingInformation.Location.Should().NotBeNull();

        updatedSolution.DataProcessingInformation.Details.Subject.Should().Be(model.Subject);
        updatedSolution.DataProcessingInformation.Details.Duration.Should().Be(model.Duration);
        updatedSolution.DataProcessingInformation.Details.ProcessingNature.Should().Be(model.ProcessingNature);
        updatedSolution.DataProcessingInformation.Details.PersonalDataTypes.Should().Be(model.PersonalDataTypes);
        updatedSolution.DataProcessingInformation.Details.DataSubjectCategories.Should().Be(model.DataSubjectCategories);

        updatedSolution.DataProcessingInformation.Location.ProcessingLocation.Should().Be(model.ProcessingLocation);
        updatedSolution.DataProcessingInformation.Location.AdditionalJurisdiction.Should().Be(model.AdditionalJurisdiction);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetDataProcessingInformation_ExistingInformation_SetsInformation(
        SetDataProcessingInformationModel model,
        DataProcessingInformation information,
        DataProcessingLocation location,
        DataProcessingDetails details,
        Solution solution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        DataProcessingInformationService service)
    {
        information.Details = details;
        information.Location = location;
        solution.DataProcessingInformation = information;

        dbContext.Solutions.Add(solution);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        await service.SetDataProcessingInformation(solution.CatalogueItemId, model);

        var updatedSolution = await service.GetSolutionWithDataProcessingInformation(solution.CatalogueItemId);

        updatedSolution.DataProcessingInformation.Should().NotBeNull();
        updatedSolution.DataProcessingInformation.Details.Should().NotBeNull();
        updatedSolution.DataProcessingInformation.Location.Should().NotBeNull();

        updatedSolution.DataProcessingInformation.Details.Subject.Should().Be(model.Subject);
        updatedSolution.DataProcessingInformation.Details.Duration.Should().Be(model.Duration);
        updatedSolution.DataProcessingInformation.Details.ProcessingNature.Should().Be(model.ProcessingNature);
        updatedSolution.DataProcessingInformation.Details.PersonalDataTypes.Should().Be(model.PersonalDataTypes);
        updatedSolution.DataProcessingInformation.Details.DataSubjectCategories.Should().Be(model.DataSubjectCategories);

        updatedSolution.DataProcessingInformation.Location.ProcessingLocation.Should().Be(model.ProcessingLocation);
        updatedSolution.DataProcessingInformation.Location.AdditionalJurisdiction.Should().Be(model.AdditionalJurisdiction);
    }
}
