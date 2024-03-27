using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Dashboard;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Shared.Dashboard;

public static class DashboardCardBaseModelTests
{
    [Theory]
    [MockAutoData]
    public static void Constructing_DashboardView_SetsPropertiesAsExpected(
        string internalOrgId,
        List<DashboardCardBaseModelStub> items)
    {
        var model = new DashboardCardBaseModel<DashboardCardBaseModelStub>(
            internalOrgId,
            items,
            isDashboardView: true);

        model.InternalOrgId.Should().Be(internalOrgId);
        model.Items.Should().BeEquivalentTo(items);
        model.NumberOfItems.Should().Be(items.Count);
        model.IsDashboardView.Should().Be(true);
        model.ItemsToIterate.Should().Be(5);
        model.PageOptions.Should().BeNull();
        model.ShouldUsePagination.Should().BeFalse();
    }

    [Theory]
    [MockAutoData]
    public static void Constructing_SetsPropertiesAsExpected(
        string internalOrgId,
        List<DashboardCardBaseModelStub> items,
        PageOptions pageOptions)
    {
        var model = new DashboardCardBaseModel<DashboardCardBaseModelStub>(
            internalOrgId,
            items,
            pageOptions: pageOptions);

        model.InternalOrgId.Should().Be(internalOrgId);
        model.Items.Should().BeEquivalentTo(items);
        model.NumberOfItems.Should().Be(pageOptions.TotalNumberOfItems);
        model.IsDashboardView.Should().Be(false);
        model.ItemsToIterate.Should().Be(model.NumberOfItems);
        model.PageOptions.Should().Be(pageOptions);
        model.ShouldUsePagination.Should().BeTrue();
    }

    public class DashboardCardBaseModelStub;
}
