using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.DataProcessingInformation;

public static class DataProcessingInformationModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Solution solution)
    {
        var model = new DataProcessingInformationModel(solution);

        model.Solution.Should().Be(solution);
    }
}
