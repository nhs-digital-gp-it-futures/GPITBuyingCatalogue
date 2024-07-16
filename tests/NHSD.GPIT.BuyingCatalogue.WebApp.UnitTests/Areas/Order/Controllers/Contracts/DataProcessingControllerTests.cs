using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DataProcessing;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.Contracts;

public class DataProcessingControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(DataProcessingPlanController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task Get_ReturnsViewWithModel(
        string internalOrgId,
        CallOffId callOffId,
        int orderId,
        ContractFlags contract,
        [Frozen] IOrderService orderService,
        [Frozen] IContractsService contractsService,
        DataProcessingPlanController controller)
    {
        orderService.GetOrderId(internalOrgId, callOffId).Returns(orderId);

        contractsService.GetContractFlags(orderId).Returns(contract);

        var result = (await controller.Index(internalOrgId, callOffId)).As<ViewResult>();

        var expected = new BespokeDataProcessingModel(internalOrgId, callOffId)
        {
            InternalOrgId = internalOrgId,
            CallOffId = callOffId,
        };

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Get_ExistingDataProcessing_ReturnsViewWithModel(
        string internalOrgId,
        CallOffId callOffId,
        int orderId,
        ContractFlags contract,
        [Frozen] IOrderService orderService,
        [Frozen] IContractsService contractsService,
        DataProcessingPlanController controller)
    {
        contract.UseDefaultDataProcessing = false;

        orderService.GetOrderId(internalOrgId, callOffId).Returns(orderId);

        contractsService.GetContractFlags(orderId).Returns(contract);

        var result = (await controller.Index(internalOrgId, callOffId)).As<ViewResult>();

        var expected = new BespokeDataProcessingModel(internalOrgId, callOffId)
        {
            InternalOrgId = internalOrgId,
            CallOffId = callOffId,
        };

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_InvalidModel_ReturnsRedirect(
        string internalOrgId,
        CallOffId callOffId,
        BespokeDataProcessingModel model,
        DataProcessingPlanController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.Index(internalOrgId, callOffId, model)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.RouteValues.Values.Should().Equal(internalOrgId, callOffId);
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_DataProcessingSetAsSeen_SetsSeen(
        string internalOrgId,
        CallOffId callOffId,
        int orderId,
        BespokeDataProcessingModel model,
        [Frozen] IOrderService orderService,
        DataProcessingPlanController controller)
    {
        orderService.GetOrderId(internalOrgId, callOffId).Returns(orderId);

        var result = (await controller.Index(internalOrgId, callOffId, model)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(OrderController.Order));
        result.ControllerName.Should().Be(typeof(OrderController).ControllerName());
        result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
        {
            [nameof(internalOrgId)] = internalOrgId,
            [nameof(callOffId)] = callOffId,
        });
    }
}
