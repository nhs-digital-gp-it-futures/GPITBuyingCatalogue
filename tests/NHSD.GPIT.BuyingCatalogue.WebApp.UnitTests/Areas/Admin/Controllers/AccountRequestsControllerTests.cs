using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AccountRequestModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AccountRequestsModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers;

public static class AccountRequestsControllerTests
{
    [Fact]
    public static void ClassIsCorrectlyDecorated()
    {
        typeof(AccountRequestsController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Policy == "ManageAccountCreationRequests");
        typeof(AccountRequestsController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Admin");
    }

    [Theory]
    [MockAutoData]
    public static async Task Index_ReturnsViewWithModel(
        string page,
        AccountRequestStatus status,
        AccountRequestOverviewModel requests,
        [Frozen] IAccountRequestsService requestsService,
        AccountRequestsController controller)
    {
        var expectedModel = new AccountRequestsDashboardModel(status, requests);

        requestsService.GetAccountRequests(status, Arg.Any<PageOptions>()).Returns(requests);

        var result = (await controller.Index(page, status)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [MockAutoData]
    public static void Index_WithStatus_Redirects(
        AccountRequestsDashboardModel model,
        AccountRequestsController controller)
    {
        var result = controller.Index(model).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
        result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { ["status"] = model.SelectedStatus });
    }

    [Theory]
    [MockAutoData]
    public static async Task ViewAccountRequest_ReturnsViewWithModel(
        Guid requestId,
        AccountRequest request,
        [Frozen] IAccountRequestsService requestsService,
        AccountRequestsController controller)
    {
        var expected = new AccountRequestModel(request);

        requestsService.GetAccountRequest(requestId).Returns(request);

        var result = (await controller.ViewAccountRequest(requestId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task ViewAccountRequest_InvalidModelState_ReturnsViewWithModel(
        Guid requestId,
        AccountRequestModel model,
        AccountRequestsController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.ViewAccountRequest(requestId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task ViewAccountRequest_Valid_Redirects(
        Guid requestId,
        AccountRequestModel model,
        [Frozen] IAccountRequestsService requestsService,
        AccountRequestsController controller)
    {
        var result = (await controller.ViewAccountRequest(requestId, model)).As<RedirectToActionResult>();

        await requestsService.Received()
            .ProcessAccountRequest(
                requestId,
                Arg.Any<int>(),
                model.SelectedStatus.GetValueOrDefault(),
                model.DecisionJustification);

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.ViewAccountRequest));
    }
}
