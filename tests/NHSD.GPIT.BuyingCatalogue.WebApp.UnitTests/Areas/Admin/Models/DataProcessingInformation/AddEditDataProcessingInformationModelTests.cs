using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.DataProcessingInformation;

public static class AddEditDataProcessingInformationModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_WithNullDataProcessingInformation_SetsPropertiesAsExpected(
        Solution solution)
    {
        solution.DataProcessingInformation = null;

        var model = new AddEditDataProcessingInformationModel(solution);

        model.SolutionName.Should().Be(solution.CatalogueItem.Name);
        model.Subject.Should().BeNull();
        model.Duration.Should().BeNull();
        model.ProcessingNature.Should().BeNull();
        model.PersonalDataTypes.Should().BeNull();
        model.DataSubjectCategories.Should().BeNull();
        model.ProcessingLocation.Should().BeNull();
        model.AdditionalJurisdiction.Should().BeNull();
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithDataProcessingInformation_SetsPropertiesAsExpected(
        Solution solution)
    {
        var model = new AddEditDataProcessingInformationModel(solution);

        model.SolutionName.Should().Be(solution.CatalogueItem.Name);
        model.Subject.Should().Be(solution.DataProcessingInformation.Details.Subject);
        model.Duration.Should().Be(solution.DataProcessingInformation.Details.Duration);
        model.ProcessingNature.Should().Be(solution.DataProcessingInformation.Details.ProcessingNature);
        model.PersonalDataTypes.Should().Be(solution.DataProcessingInformation.Details.PersonalDataTypes);
        model.DataSubjectCategories.Should().Be(solution.DataProcessingInformation.Details.DataSubjectCategories);
        model.ProcessingLocation.Should().Be(solution.DataProcessingInformation.Location.ProcessingLocation);
        model.AdditionalJurisdiction.Should().Be(solution.DataProcessingInformation.Location.AdditionalJurisdiction);
    }
}
