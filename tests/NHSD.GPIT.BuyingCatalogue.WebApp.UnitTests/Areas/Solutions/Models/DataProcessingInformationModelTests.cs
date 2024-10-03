using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models;

public static class DataProcessingInformationModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Solution solution,
        CatalogueItemContentStatus contentStatuses)
    {
        var model = new DataProcessingInformationModel(solution.CatalogueItem, contentStatuses);

        model.Index.Should().Be(11);
        model.Information.Should().Be(solution.DataProcessingInformation);
    }
}
