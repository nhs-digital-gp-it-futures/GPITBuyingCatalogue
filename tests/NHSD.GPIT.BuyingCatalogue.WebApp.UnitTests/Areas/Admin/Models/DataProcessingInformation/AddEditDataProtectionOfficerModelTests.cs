using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.DataProcessingInformation;

public static class AddEditDataProtectionOfficerModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_WithNullDataProtectionOfficer_SetsPropertiesAsExpected(
        Solution solution)
    {
        solution.DataProcessingInformation = null;

        var model = new AddEditDataProtectionOfficerModel(solution);

        model.SolutionName.Should().Be(solution.CatalogueItem.Name);
        model.Name.Should().BeNull();
        model.EmailAddress.Should().BeNull();
        model.PhoneNumber.Should().BeNull();
    }

    [Theory]
    [MockAutoData]
    public static void Construct_WithDataProtectionOfficer_SetsPropertiesAsExpected(
        Solution solution)
    {
        var model = new AddEditDataProtectionOfficerModel(solution);

        model.SolutionName.Should().Be(solution.CatalogueItem.Name);
        model.Name.Should().Be(solution.DataProcessingInformation.Officer.Name);
        model.EmailAddress.Should().Be(solution.DataProcessingInformation.Officer.EmailAddress);
        model.PhoneNumber.Should().Be(solution.DataProcessingInformation.Officer.PhoneNumber);
    }
}
