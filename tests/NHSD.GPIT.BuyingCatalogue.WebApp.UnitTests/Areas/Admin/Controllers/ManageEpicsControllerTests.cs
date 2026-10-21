using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageEpics;
using Xunit;
using Epic = NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models.Epic;
using EpicModel = NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageEpics.Epic;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers;

public static class ManageEpicsControllerTests
{
    [Fact]
    public static void ClassIsCorrectlyDecorated()
    {
        typeof(ManageEpicsController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Policy == "ManageEpics");
        typeof(ManageEpicsController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Admin");
        typeof(ManageEpicsController).Should().BeDecoratedWith<RouteAttribute>(r => r.Template == "admin/manage-epics");
    }

    [Theory]
    [MockInlineAutoData(null, 1)]
    [MockInlineAutoData("", 1)]
    [MockInlineAutoData("1", 1)]
    [MockInlineAutoData("2", 2)]
    public static async Task Get_Index_GetsEpicsWithExpectedPageOptions(
        string page,
        int expectedPageNumber,
        PagedList<Epic> epics,
        [Frozen] IEpicsService epicsService,
        ManageEpicsController controller)
    {
        epicsService.GetEpicsAsync(Arg.Any<PageOptions>()).Returns(epics);

        await controller.Index(page);

        await epicsService.Received().GetEpicsAsync(
            Arg.Is<PageOptions>(x => x.PageNumber == expectedPageNumber && x.PageSize == 20));
    }

    [Theory]
    [MockAutoData]
    public static async Task Get_Index_ReturnsViewWithExpectedModel(
        string page,
        string backLink,
        PagedList<Epic> epics,
        [Frozen] IEpicsService epicsService,
        [Frozen] IUrlHelper urlHelper,
        ManageEpicsController controller)
    {
        epics.Items[0].IsActive = true;
        epics.Items.Skip(1).ToList().ForEach(x => x.IsActive = false);

        epicsService.GetEpicsAsync(Arg.Any<PageOptions>()).Returns(epics);
        urlHelper.Action(Arg.Any<UrlActionContext>()).Returns(backLink);

        var expected = new ViewEpicsModel(epics.Items.Select(x => new EpicModel(x.Name, x.Id, x.IsActive)))
        {
            Title = "Manage Epics",
            Advice = "View existing epics",
            BackLink = backLink,
            PageOptions = epics.Options,
        };

        var result = (await controller.Index(page)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expected);
        result.Model.As<ViewEpicsModel>().PageOptions.Should().BeSameAs(epics.Options);
    }

    [Theory]
    [MockAutoData]
    public static async Task Get_Index_NoEpics_ReturnsViewWithEmptyModel(
        PageOptions pageOptions,
        [Frozen] IEpicsService epicsService,
        ManageEpicsController controller)
    {
        epicsService.GetEpicsAsync(Arg.Any<PageOptions>()).Returns(new PagedList<Epic>(new List<Epic>(), pageOptions));

        var result = (await controller.Index(null)).As<ViewResult>();

        result.Should().NotBeNull();
        var model = result.Model.As<ViewEpicsModel>();
        model.Epics.Should().BeEmpty();
        model.PageOptions.Should().BeSameAs(pageOptions);
    }
}
