using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageEpics;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.ManageEpics;

public static class ViewEpicsModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        List<Epic> epics)
    {
        var model = new ViewEpicsModel(epics);

        model.Epics.Should().BeEquivalentTo(epics);
    }
}
