using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.DataProcessingInformation;

public static class AddEditSubProcessorModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_AddingSubProcessor_SetsPropertiesAsExpected(
        Solution solution)
    {
        var model = new AddEditSubProcessorModel(solution);

        model.SolutionId.Should().Be(solution.CatalogueItemId);
        model.SolutionName.Should().Be(solution.CatalogueItem.Name);
        model.PublicationStatus.Should().Be(solution.CatalogueItem.PublishedStatus);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_EditingSubProcessor_SetsPropertiesAsExpected(
        Solution solution,
        List<DataProtectionSubProcessor> subProcessors)
    {
        var subProcessor = subProcessors.First();
        solution.DataProcessingInformation = new EntityFramework.Catalogue.Models.DataProcessingInformation
        {
            SubProcessors = subProcessors,
        };

        var model = new AddEditSubProcessorModel(solution, subProcessor);

        model.SolutionId.Should().Be(solution.CatalogueItemId);
        model.SolutionName.Should().Be(solution.CatalogueItem.Name);
        model.PublicationStatus.Should().Be(solution.CatalogueItem.PublishedStatus);
        model.NumberOfSubProcessors.Should().Be(subProcessors.Count);

        model.OrganisationName.Should().Be(subProcessor.OrganisationName);
        model.PostProcessingPlan.Should().Be(subProcessor.PostProcessingPlan);

        model.Subject.Should().Be(subProcessor.Details.Subject);
        model.Duration.Should().Be(subProcessor.Details.Duration);
        model.ProcessingNature.Should().Be(subProcessor.Details.ProcessingNature);
        model.PersonalDataTypes.Should().Be(subProcessor.Details.PersonalDataTypes);
        model.DataSubjectCategories.Should().Be(subProcessor.Details.DataSubjectCategories);
    }

    [Theory]
    [MockInlineAutoData(PublicationStatus.Unpublished)]
    [MockInlineAutoData(PublicationStatus.Draft)]
    [MockInlineAutoData(PublicationStatus.Suspended)]
    [MockInlineAutoData(PublicationStatus.InRemediation)]
    public static void CanDelete_SolutionNotPublished_ReturnsTrue(
        PublicationStatus publicationStatus,
        Solution solution,
        DataProtectionSubProcessor subProcessor)
    {
        solution.CatalogueItem.PublishedStatus = publicationStatus;
        solution.DataProcessingInformation = new EntityFramework.Catalogue.Models.DataProcessingInformation
        {
            SubProcessors = new List<DataProtectionSubProcessor> { subProcessor },
        };

        var model = new AddEditSubProcessorModel(solution, subProcessor);

        model.CanDelete.Should().BeTrue();
    }

    [Theory]
    [MockAutoData]
    public static void CanDelete_PublishedWithMultipleSubProcessors_ReturnsTrue(
        Solution solution,
        List<DataProtectionSubProcessor> subProcessors)
    {
        var subProcessor = subProcessors.First();

        solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;
        solution.DataProcessingInformation = new EntityFramework.Catalogue.Models.DataProcessingInformation
        {
            SubProcessors = subProcessors,
        };

        var model = new AddEditSubProcessorModel(solution, subProcessor);

        model.CanDelete.Should().BeTrue();
    }

    [Theory]
    [MockAutoData]
    public static void CanDelete_PublishedWithSingleSubProcessors_ReturnsFalse(
        Solution solution,
        DataProtectionSubProcessor subProcessor)
    {
        solution.CatalogueItem.PublishedStatus = PublicationStatus.Published;
        solution.DataProcessingInformation = new EntityFramework.Catalogue.Models.DataProcessingInformation
        {
            SubProcessors = new List<DataProtectionSubProcessor> { subProcessor },
        };

        var model = new AddEditSubProcessorModel(solution, subProcessor);

        model.CanDelete.Should().BeFalse();
    }
}
