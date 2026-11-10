using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageEpics;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.ManageEpics;

public static class EpicTests
{
    [Theory]
    [MockInlineAutoData(true)]
    [MockInlineAutoData(false)]
    public static void Construct_SetsPropertiesAsExpected(
        bool isActive,
        string name,
        string reference)
    {
        var model = new Epic(name, reference, isActive);

        model.Name.Should().Be(name);
        model.Reference.Should().Be(reference);
        model.IsActive.Should().Be(isActive);
    }
}
