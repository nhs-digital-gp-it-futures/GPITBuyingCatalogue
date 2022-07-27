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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders.Contracts;
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
        Contract contract,
        [Frozen] Mock<IContractsService> service,
        DataProcessingPlanController controller)
    {
        service.Setup(s => s.GetContract(callOffId.Id))
            .ReturnsAsync(contract);

        var result = (await controller.Index(internalOrgId, callOffId)).As<ViewResult>();

        var expectedModel = new DataProcessingPlanModel(contract.DataProcessingPlan)
        {
            CallOffId = callOffId,
        };

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Get_ExistingDataProcessing_ReturnsViewWithModel(
        string internalOrgId,
        CallOffId callOffId,
        Contract contract,
        DataProcessingPlan dataProcessingPlan,
        [Frozen] Mock<IContractsService> service,
        DataProcessingPlanController controller)
    {
        dataProcessingPlan.IsDefault = true;
        contract.DataProcessingPlan = dataProcessingPlan;

        service.Setup(s => s.GetContract(callOffId.Id))
            .ReturnsAsync(contract);

        var expectedModel = new DataProcessingPlanModel(contract.DataProcessingPlan)
        {
            CallOffId = callOffId,
        };

        var result = (await controller.Index(internalOrgId, callOffId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
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
        Contract contract,
        DataProcessingPlan plan,
        DataProcessingPlanModel model,
        [Frozen] Mock<IDataProcessingPlanService> dataProcessingService,
        [Frozen] Mock<IContractsService> contractsService,
        DataProcessingPlanController controller)
    {
        contract.DataProcessingPlanId = null;
        model.UseDefaultDataProcessing = true;

        contractsService.Setup(s => s.GetContract(callOffId.Id))
            .ReturnsAsync(contract);

        dataProcessingService.Setup(s => s.GetDefaultDataProcessingPlan())
            .ReturnsAsync(plan);

        var result = (await controller.Index(internalOrgId, callOffId, model)).As<RedirectToActionResult>();

        dataProcessingService.Verify(
            s => s.GetDefaultDataProcessingPlan());

        contractsService.Verify(
            s => s.SetDataProcessingPlanId(callOffId.Id, plan.Id));

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
        Contract contract,
        DataProcessingPlan plan,
        DataProcessingPlanModel model,
        [Frozen] Mock<IDataProcessingPlanService> dataProcessingService,
        [Frozen] Mock<IContractsService> contractsService,
        DataProcessingPlanController controller)
    {
        contract.DataProcessingPlanId = null;
        model.UseDefaultDataProcessing = false;

        contractsService.Setup(s => s.GetContract(callOffId.Id))
            .ReturnsAsync(contract);

        dataProcessingService.Setup(s => s.CreateDataProcessingPlan())
            .ReturnsAsync(plan);

        var result = (await controller.Index(internalOrgId, callOffId, model)).As<RedirectToActionResult>();

        dataProcessingService.Verify(
            s => s.CreateDataProcessingPlan());

        contractsService.Verify(
            s => s.SetDataProcessingPlanId(callOffId.Id, plan.Id));

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
