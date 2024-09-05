using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DataProcessingInformation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.DataProcessingInformation;

public static class DeleteSubProcessorModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        DataProtectionSubProcessor subProcessor)
    {
        var model = new DeleteSubProcessorModel(subProcessor);

        model.OrganisationName.Should().Be(subProcessor.OrganisationName);
    }
}
