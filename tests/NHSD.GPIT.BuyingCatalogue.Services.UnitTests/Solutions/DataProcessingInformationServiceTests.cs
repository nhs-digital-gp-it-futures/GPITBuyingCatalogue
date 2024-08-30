using System;
using System.Collections.Generic;
using System.Linq;
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
        updatedSolution.DataProcessingInformation.Details.DataSubjectCategories.Should()
            .Be(model.DataSubjectCategories);

        updatedSolution.DataProcessingInformation.Location.ProcessingLocation.Should().Be(model.ProcessingLocation);
        updatedSolution.DataProcessingInformation.Location.AdditionalJurisdiction.Should()
            .Be(model.AdditionalJurisdiction);
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
        updatedSolution.DataProcessingInformation.Details.DataSubjectCategories.Should()
            .Be(model.DataSubjectCategories);

        updatedSolution.DataProcessingInformation.Location.ProcessingLocation.Should().Be(model.ProcessingLocation);
        updatedSolution.DataProcessingInformation.Location.AdditionalJurisdiction.Should()
            .Be(model.AdditionalJurisdiction);
    }

    [Theory]
    [MockAutoData]
    public static Task SetDataProtectionOfficer_NullModel_ThrowsArgumentNullException(
        CatalogueItemId solutionId,
        DataProcessingInformationService service) => FluentActions
        .Awaiting(() => service.SetDataProtectionOfficer(solutionId, null))
        .Should()
        .ThrowAsync<ArgumentNullException>();

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetDataProtectionOfficer_NullInformation_SetsInformation(
        SetDataProtectionOfficerModel model,
        Solution solution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        DataProcessingInformationService service)
    {
        solution.DataProcessingInformation = null;

        dbContext.Solutions.Add(solution);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        await service.SetDataProtectionOfficer(solution.CatalogueItemId, model);

        var updatedSolution = await service.GetSolutionWithDataProcessingInformation(solution.CatalogueItemId);

        updatedSolution.DataProcessingInformation.Should().NotBeNull();
        updatedSolution.DataProcessingInformation.Officer.Should().NotBeNull();

        updatedSolution.DataProcessingInformation.Officer.Name.Should().Be(model.Name);
        updatedSolution.DataProcessingInformation.Officer.EmailAddress.Should().Be(model.EmailAddress);
        updatedSolution.DataProcessingInformation.Officer.PhoneNumber.Should().Be(model.PhoneNumber);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetDataProtectionOfficer_ExistingInformation_SetsInformation(
        SetDataProtectionOfficerModel model,
        DataProcessingInformation information,
        DataProtectionOfficer officer,
        Solution solution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        DataProcessingInformationService service)
    {
        information.Officer = officer;
        solution.DataProcessingInformation = information;

        dbContext.Solutions.Add(solution);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        await service.SetDataProtectionOfficer(solution.CatalogueItemId, model);

        var updatedSolution = await service.GetSolutionWithDataProcessingInformation(solution.CatalogueItemId);

        updatedSolution.DataProcessingInformation.Should().NotBeNull();
        updatedSolution.DataProcessingInformation.Officer.Should().NotBeNull();

        updatedSolution.DataProcessingInformation.Officer.Name.Should().Be(model.Name);
        updatedSolution.DataProcessingInformation.Officer.EmailAddress.Should().Be(model.EmailAddress);
        updatedSolution.DataProcessingInformation.Officer.PhoneNumber.Should().Be(model.PhoneNumber);
    }

    [Theory]
    [MockAutoData]
    public static Task AddSubProcessor_NullModel_ThrowsArgumentNullException(
        CatalogueItemId solutionId,
        DataProcessingInformationService service) => FluentActions
        .Awaiting(() => service.AddSubProcessor(solutionId, null))
        .Should()
        .ThrowAsync<ArgumentNullException>();

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task AddSubProcessor_NullInformation_SetsInformation(
        SetSubProcessorModel model,
        Solution solution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        DataProcessingInformationService service)
    {
        solution.DataProcessingInformation = null;

        dbContext.Solutions.Add(solution);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        await service.AddSubProcessor(solution.CatalogueItemId, model);

        var updatedSolution = await service.GetSolutionWithDataProcessingInformation(solution.CatalogueItemId);

        updatedSolution.DataProcessingInformation.Should().NotBeNull();
        updatedSolution.DataProcessingInformation.SubProcessors.Should().NotBeEmpty();

        updatedSolution.DataProcessingInformation.SubProcessors.Should()
            .Contain(
                x => string.Equals(model.OrganisationName, x.OrganisationName)
                    && string.Equals(model.PostProcessingPlan, x.PostProcessingPlan)
                    && string.Equals(model.Subject, x.Details.Subject)
                    && string.Equals(model.Duration, x.Details.Duration)
                    && string.Equals(model.ProcessingNature, x.Details.ProcessingNature)
                    && string.Equals(model.PersonalDataTypes, x.Details.PersonalDataTypes)
                    && string.Equals(model.DataSubjectCategories, x.Details.DataSubjectCategories));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task AddSubProcessor_ExistingInformation_AddsSubProcessor(
        SetSubProcessorModel model,
        DataProcessingInformation information,
        Solution solution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        DataProcessingInformationService service)
    {
        solution.DataProcessingInformation = information;
        dbContext.Solutions.Add(solution);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        await service.AddSubProcessor(solution.CatalogueItemId, model);

        var updatedSolution = await service.GetSolutionWithDataProcessingInformation(solution.CatalogueItemId);

        updatedSolution.DataProcessingInformation.Should().NotBeNull();
        updatedSolution.DataProcessingInformation.SubProcessors.Should().NotBeEmpty();

        updatedSolution.DataProcessingInformation.SubProcessors.Should()
            .Contain(
                x => string.Equals(model.OrganisationName, x.OrganisationName)
                    && string.Equals(model.PostProcessingPlan, x.PostProcessingPlan)
                    && string.Equals(model.Subject, x.Details.Subject)
                    && string.Equals(model.Duration, x.Details.Duration)
                    && string.Equals(model.ProcessingNature, x.Details.ProcessingNature)
                    && string.Equals(model.PersonalDataTypes, x.Details.PersonalDataTypes)
                    && string.Equals(model.DataSubjectCategories, x.Details.DataSubjectCategories));
    }

    [Theory]
    [MockAutoData]
    public static Task EditSubProcessor_NullModel_ThrowsArgumentNullException(
        CatalogueItemId solutionId,
        DataProcessingInformationService service) => FluentActions
        .Awaiting(() => service.EditSubProcessor(solutionId, null))
        .Should()
        .ThrowAsync<ArgumentNullException>();

    [Theory]
    [MockAutoData]
    public static Task EditSubProcessor_NullSubProcessorId_ThrowsArgumentException(
        CatalogueItemId solutionId,
        SetSubProcessorModel model,
        DataProcessingInformationService service)
    {
        var updatedModel = model with { SubProcessorId = null };

        return FluentActions
            .Awaiting(() => service.EditSubProcessor(solutionId, updatedModel))
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task EditSubProcessor_NullInformation_ThrowsInvalidOperationException(
        SetSubProcessorModel model,
        Solution solution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        DataProcessingInformationService service)
    {
        solution.DataProcessingInformation = null;

        dbContext.Solutions.Add(solution);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        await FluentActions.Awaiting(() => service.EditSubProcessor(solution.CatalogueItemId, model))
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task EditSubProcessor_InvalidSubProcessorId_SetsInformation(
        SetSubProcessorModel model,
        DataProcessingInformation information,
        DataProtectionSubProcessor subProcessor,
        Solution solution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        DataProcessingInformationService service)
    {
        information.SubProcessors = new List<DataProtectionSubProcessor> { subProcessor };
        solution.DataProcessingInformation = information;
        dbContext.Solutions.Add(solution);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        await FluentActions.Awaiting(() => service.EditSubProcessor(solution.CatalogueItemId, model))
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task EditSubProcessor_ValidSubProcessorId_EditsSubProcessor(
        SetSubProcessorModel model,
        DataProcessingInformation information,
        DataProtectionSubProcessor subProcessor,
        Solution solution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        DataProcessingInformationService service)
    {
        var updatedModel = model with { SubProcessorId = subProcessor.Id };

        information.SubProcessors = new List<DataProtectionSubProcessor> { subProcessor };
        solution.DataProcessingInformation = information;
        dbContext.Solutions.Add(solution);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        await service.EditSubProcessor(solution.CatalogueItemId, updatedModel);

        var updatedSolution = await service.GetSolutionWithDataProcessingInformation(solution.CatalogueItemId);

        var updatedSubProcessor =
            updatedSolution.DataProcessingInformation.SubProcessors.FirstOrDefault(x => x.Id == subProcessor.Id);

        updatedSubProcessor.Should().NotBeNull();
        updatedSubProcessor!.OrganisationName.Should().Be(model.OrganisationName);
        updatedSubProcessor.PostProcessingPlan.Should().Be(model.PostProcessingPlan);

        updatedSubProcessor.Details.Subject.Should().Be(model.Subject);
        updatedSubProcessor.Details.Duration.Should().Be(model.Duration);
        updatedSubProcessor.Details.ProcessingNature.Should().Be(model.ProcessingNature);
        updatedSubProcessor.Details.PersonalDataTypes.Should().Be(model.PersonalDataTypes);
        updatedSubProcessor.Details.DataSubjectCategories.Should().Be(model.DataSubjectCategories);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task DeleteSubProcessor_InvalidId_Returns(
        int invalidSubProcessorId,
        DataProcessingInformation information,
        DataProtectionSubProcessor subProcessor,
        Solution solution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        DataProcessingInformationService service)
    {
        information.SubProcessors = new List<DataProtectionSubProcessor> { subProcessor };
        solution.DataProcessingInformation = information;
        dbContext.Solutions.Add(solution);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        await service.DeleteSubProcessor(solution.CatalogueItemId, invalidSubProcessorId);

        var existingSolution = await service.GetSolutionWithDataProcessingInformation(solution.CatalogueItemId);

        existingSolution.Should().NotBeNull();
        existingSolution.DataProcessingInformation.Should().NotBeNull();
        existingSolution.DataProcessingInformation.SubProcessors.Should().ContainSingle();
        existingSolution.DataProcessingInformation.SubProcessors.Should().ContainEquivalentOf(subProcessor);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task DeleteSubProcessor_ValidId_DeletesSubProcessor(
        DataProcessingInformation information,
        DataProtectionSubProcessor subProcessor,
        Solution solution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        DataProcessingInformationService service)
    {
        information.SubProcessors = new List<DataProtectionSubProcessor> { subProcessor };
        solution.DataProcessingInformation = information;
        dbContext.Solutions.Add(solution);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        await service.DeleteSubProcessor(solution.CatalogueItemId, subProcessor.Id);

        var existingSolution = await service.GetSolutionWithDataProcessingInformation(solution.CatalogueItemId);

        existingSolution.Should().NotBeNull();
        existingSolution.DataProcessingInformation.Should().NotBeNull();
        existingSolution.DataProcessingInformation.SubProcessors.Should().BeEmpty();
    }
}
