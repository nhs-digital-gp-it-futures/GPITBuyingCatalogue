﻿using System.Threading.Tasks;
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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.DataProcessing;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.Contracts;

public class DataProcessingControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(DataProcessingPlanController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Get_ReturnsViewWithModel(
        string internalOrgId,
        CallOffId callOffId,
        int orderId,
        ContractFlags contract,
        [Frozen] Mock<IOrderService> orderService,
        [Frozen] Mock<IContractsService> contractsService,
        DataProcessingPlanController controller)
    {
        orderService
            .Setup(x => x.GetOrderId(internalOrgId, callOffId))
            .ReturnsAsync(orderId);

        contractsService
            .Setup(s => s.GetContract(orderId))
            .ReturnsAsync(contract);

        var result = (await controller.Index(internalOrgId, callOffId)).As<ViewResult>();

        orderService.VerifyAll();
        contractsService.VerifyAll();

        var expected = new DataProcessingPlanModel(contract)
        {
            CallOffId = callOffId,
        };

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Get_ExistingDataProcessing_ReturnsViewWithModel(
        string internalOrgId,
        CallOffId callOffId,
        int orderId,
        ContractFlags contract,
        [Frozen] Mock<IOrderService> orderService,
        [Frozen] Mock<IContractsService> contractsService,
        DataProcessingPlanController controller)
    {
        contract.UseDefaultDataProcessing = false;

        orderService
            .Setup(x => x.GetOrderId(internalOrgId, callOffId))
            .ReturnsAsync(orderId);

        contractsService
            .Setup(s => s.GetContract(orderId))
            .ReturnsAsync(contract);

        var result = (await controller.Index(internalOrgId, callOffId)).As<ViewResult>();

        orderService.VerifyAll();
        contractsService.VerifyAll();

        var expected = new DataProcessingPlanModel(contract)
        {
            CallOffId = callOffId,
        };

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_InvalidModel_ReturnsView(
        string internalOrgId,
        CallOffId callOffId,
        DataProcessingPlanModel model,
        DataProcessingPlanController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.Index(internalOrgId, callOffId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(model);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_UseDefaultPlan_SetsDefaultPlan(
        string internalOrgId,
        CallOffId callOffId,
        int orderId,
        DataProcessingPlanModel model,
        [Frozen] Mock<IOrderService> orderService,
        [Frozen] Mock<IContractsService> contractsService,
        DataProcessingPlanController controller)
    {
        model.UseDefaultDataProcessing = true;

        orderService
            .Setup(x => x.GetOrderId(internalOrgId, callOffId))
            .ReturnsAsync(orderId);

        contractsService
            .Setup(x => x.UseDefaultDataProcessing(orderId, true))
            .Verifiable();

        var result = (await controller.Index(internalOrgId, callOffId, model)).As<RedirectToActionResult>();

        orderService.VerifyAll();
        contractsService.VerifyAll();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(OrderController.Order));
        result.ControllerName.Should().Be(typeof(OrderController).ControllerName());
        result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
        {
            [nameof(internalOrgId)] = internalOrgId,
            [nameof(callOffId)] = callOffId,
        });
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_UseCustomPlan_SetsPlan(
        string internalOrgId,
        CallOffId callOffId,
        int orderId,
        DataProcessingPlanModel model,
        [Frozen] Mock<IOrderService> orderService,
        [Frozen] Mock<IContractsService> contractsService,
        DataProcessingPlanController controller)
    {
        model.UseDefaultDataProcessing = false;

        orderService
            .Setup(x => x.GetOrderId(internalOrgId, callOffId))
            .ReturnsAsync(orderId);

        contractsService
            .Setup(x => x.UseDefaultDataProcessing(orderId, false))
            .Verifiable();

        var result = (await controller.Index(internalOrgId, callOffId, model)).As<RedirectToActionResult>();

        orderService.VerifyAll();
        contractsService.VerifyAll();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(DataProcessingPlanController.BespokeDataProcessingPlan));
        result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
        {
            [nameof(internalOrgId)] = internalOrgId,
            [nameof(callOffId)] = callOffId,
        });
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_BespokeDataProcessingPlan_Redirects(
        string internalOrgId,
        CallOffId callOffId,
        DataProcessingPlanModel model,
        DataProcessingPlanController controller)
    {
        model.UseDefaultDataProcessing = false;

        var result = (await controller.Index(internalOrgId, callOffId, model)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(DataProcessingPlanController.BespokeDataProcessingPlan));
        result.ControllerName.Should().BeNull();
        result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
        {
            [nameof(internalOrgId)] = internalOrgId,
            [nameof(callOffId)] = callOffId,
        });
    }

    [Theory]
    [CommonAutoData]
    public static void Get_BespokeDataProcessingPlan_ReturnsViewWithModel(
        string internalOrgId,
        CallOffId callOffId,
        DataProcessingPlanController controller)
    {
        var expectedModel = new BespokeDataProcessingModel(internalOrgId, callOffId);

        var result = controller.BespokeDataProcessingPlan(internalOrgId, callOffId).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }
}
