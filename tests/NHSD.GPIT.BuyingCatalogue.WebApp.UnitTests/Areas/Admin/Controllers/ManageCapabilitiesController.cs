using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageCapabilities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageCapabilities;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers;

public static class ManageCapabilitiesControllerTests
{
    [Fact]
    public static void ClassIsCorrectlyDecorated()
    {
        typeof(ManageCapabilitiesController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Admin");
        typeof(ManageCapabilitiesController).Should().BeDecoratedWith<RouteAttribute>(a => a.Template == "admin/manage-capabilities");
    }

    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(ManageCapabilitiesController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task Index_CallsServiceWithExpectedPageOptions(
        string page,
        IList<AdminManageCapability> items,
        [Frozen] ICapabilitiesAdminService capabilitiesAdminService,
        ManageCapabilitiesController controller)
    {
        var pagedList = new PagedList<AdminManageCapability>(items, new PageOptions(page, 20));

        capabilitiesAdminService.GetPagedCapabilitiesAsync(Arg.Any<PageOptions>()).Returns(pagedList);

        await controller.Index(page);

        var expectedPage = new PageOptions(page);

        await capabilitiesAdminService.Received().GetPagedCapabilitiesAsync(
            Arg.Is<PageOptions>(o => o.PageSize == 20 && o.PageNumber == expectedPage.PageNumber));
    }

    [Theory]
    [MockAutoData]
    public static async Task Index_ReturnsViewWithExpectedModel(
        string page,
        IList<AdminManageCapability> items,
        [Frozen] ICapabilitiesAdminService capabilitiesAdminService,
        ManageCapabilitiesController controller)
    {
        var pagedList = new PagedList<AdminManageCapability>(items, new PageOptions(page, 20));
        var expected = new ManageCapabilitiesModel(pagedList.Items, pagedList.Options);

        capabilitiesAdminService.GetPagedCapabilitiesAsync(Arg.Any<PageOptions>()).Returns(pagedList);

        var result = (await controller.Index(page)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.BackLink));
    }
}
