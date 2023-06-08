using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.AssociatedServicesBilling;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.Contracts
{
    public static class AssociatedServicesBillingControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AssociatedServicesBillingController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(AssociatedServicesBillingController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AssociatedServicesBillingController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ReviewBilling_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            int orderId,
            ContractFlags contract,
            List<OrderItem> orderItems,
            ImplementationPlan defaultPlan,
            [Frozen] Mock<IAssociatedServicesBillingService> mockAssociatedServicesBillingService,
            [Frozen] Mock<IContractsService> mockContractsService,
            [Frozen] Mock<IImplementationPlanService> mockImplementationPlanService,
            [Frozen] Mock<IOrderService> orderService,
            AssociatedServicesBillingController controller)
        {
            mockAssociatedServicesBillingService
                .Setup(a => a.GetAssociatedServiceOrderItems(internalOrgId, callOffId))
                .ReturnsAsync(orderItems);

            mockImplementationPlanService
                .Setup(i => i.GetDefaultImplementationPlan())
                .ReturnsAsync(defaultPlan);

            orderService
                .Setup(x => x.GetOrderId(internalOrgId, callOffId))
                .ReturnsAsync(orderId);

            mockContractsService
                .Setup(c => c.GetContractFlags(orderId))
                .ReturnsAsync(contract);

            var result = await controller.ReviewBilling(internalOrgId, callOffId);

            mockAssociatedServicesBillingService.VerifyAll();
            mockContractsService.VerifyAll();
            mockImplementationPlanService.VerifyAll();
            orderService.VerifyAll();

            var expected = new ReviewBillingModel
            {
                CallOffId = callOffId,
                TargetMilestoneName = defaultPlan.Milestones.OrderBy(m => m.Order).Last().Title,
                AssociatedServiceOrderItems = orderItems,
                UseDefaultBilling = contract.UseDefaultBilling,
            };

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ReviewBilling_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            List<OrderItem> orderItems,
            ImplementationPlan defaultPlan,
            ReviewBillingModel model,
            [Frozen] Mock<IAssociatedServicesBillingService> mockAssociatedServicesBillingService,
            AssociatedServicesBillingController controller)
        {
            model.CallOffId = callOffId;
            model.UseDefaultBilling = null;
            model.TargetMilestoneName = defaultPlan.Milestones.OrderBy(m => m.Order).Last().Title;
            model.AssociatedServiceOrderItems = null;

            mockAssociatedServicesBillingService
                .Setup(a => a.GetAssociatedServiceOrderItems(internalOrgId, callOffId))
                .ReturnsAsync(orderItems);

            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.ReviewBilling(internalOrgId, callOffId, model);

            mockAssociatedServicesBillingService.VerifyAll();

            var expected = new ReviewBillingModel
            {
                Title = model.Title,
                Caption = model.Caption,
                Advice = model.Advice,
                AdditionalAdvice = model.AdditionalAdvice,
                CallOffId = callOffId,
                TargetMilestoneName = defaultPlan.Milestones.OrderBy(m => m.Order).Last().Title,
                AssociatedServiceOrderItems = orderItems,
                UseDefaultBilling = null,
            };

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink).Excluding(m => m.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ReviewBilling_UseDefaultBilling_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            int orderId,
            ImplementationPlan defaultPlan,
            ReviewBillingModel model,
            [Frozen] Mock<IOrderService> orderService,
            [Frozen] Mock<IContractsService> mockContractsService,
            AssociatedServicesBillingController controller)
        {
            model.CallOffId = callOffId;
            model.UseDefaultBilling = true;
            model.TargetMilestoneName = defaultPlan.Milestones.OrderBy(m => m.Order).Last().Title;
            model.AssociatedServiceOrderItems = null;

            orderService
                .Setup(x => x.GetOrderId(internalOrgId, callOffId))
                .ReturnsAsync(orderId);

            mockContractsService
                .Setup(c => c.UseDefaultBilling(orderId, true))
                .Verifiable();

            var result = await controller.ReviewBilling(internalOrgId, callOffId, model);

            orderService.VerifyAll();
            mockContractsService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(AssociatedServicesBillingController.SpecificRequirements));
            actualResult.ControllerName.Should().Be(typeof(AssociatedServicesBillingController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "fromBespoke", false },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ReviewBilling_DoNotUseDefaultBilling_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            int orderId,
            ImplementationPlan defaultPlan,
            ReviewBillingModel model,
            [Frozen] Mock<IOrderService> orderService,
            [Frozen] Mock<IContractsService> mockContractsService,
            AssociatedServicesBillingController controller)
        {
            model.CallOffId = callOffId;
            model.UseDefaultBilling = false;
            model.TargetMilestoneName = defaultPlan.Milestones.OrderBy(m => m.Order).Last().Title;
            model.AssociatedServiceOrderItems = null;

            orderService
                .Setup(x => x.GetOrderId(internalOrgId, callOffId))
                .ReturnsAsync(orderId);

            mockContractsService
                .Setup(c => c.UseDefaultBilling(orderId, false))
                .Verifiable();

            var result = await controller.ReviewBilling(internalOrgId, callOffId, model);

            orderService.VerifyAll();
            mockContractsService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(AssociatedServicesBillingController.BespokeBilling));
            actualResult.ControllerName.Should().Be(typeof(AssociatedServicesBillingController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static void Get_BespokeBilling_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            AssociatedServicesBillingController controller)
        {
            var result = controller.BespokeBilling(internalOrgId, callOffId);

            var expected = new BasicBillingModel
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SpecificRequirements_NotFromBespoke_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            int orderId,
            ContractFlags contract,
            [Frozen] Mock<IUrlHelper> urlHelper,
            [Frozen] Mock<IOrderService> orderService,
            [Frozen] Mock<IContractsService> mockContractsService,
            AssociatedServicesBillingController controller)
        {
            orderService
                .Setup(x => x.GetOrderId(internalOrgId, callOffId))
                .ReturnsAsync(orderId);

            mockContractsService
                .Setup(c => c.GetContractFlags(orderId))
                .ReturnsAsync(contract);

            var result = await controller.SpecificRequirements(internalOrgId, callOffId, fromBespoke: false);

            orderService.VerifyAll();
            mockContractsService.VerifyAll();

            var expected = new SpecificRequirementsModel
            {
                CallOffId = callOffId,
                ProceedWithoutSpecificRequirements = !contract.HasSpecificRequirements,
            };

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));

            urlHelper.Verify(u => u.Action(
                It.Is<UrlActionContext>(match => match.Action == nameof(controller.ReviewBilling))));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SpecificRequirements_FromBespoke_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            int orderId,
            ContractFlags contract,
            [Frozen] Mock<IUrlHelper> urlHelper,
            [Frozen] Mock<IContractsService> mockContractsService,
            [Frozen] Mock<IOrderService> orderService,
            AssociatedServicesBillingController controller)
        {
            orderService
                .Setup(x => x.GetOrderId(internalOrgId, callOffId))
                .ReturnsAsync(orderId);

            mockContractsService
                .Setup(c => c.GetContractFlags(orderId))
                .ReturnsAsync(contract);

            var result = await controller.SpecificRequirements(internalOrgId, callOffId, true);

            mockContractsService.VerifyAll();
            orderService.VerifyAll();

            var expected = new SpecificRequirementsModel
            {
                CallOffId = callOffId,
                ProceedWithoutSpecificRequirements = !contract.HasSpecificRequirements,
            };

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));

            urlHelper.Verify(u => u.Action(
                It.Is<UrlActionContext>(match => match.Action == nameof(controller.BespokeBilling))));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SpecificRequirements_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SpecificRequirementsModel model,
            AssociatedServicesBillingController controller)
        {
            model.CallOffId = callOffId;
            model.ProceedWithoutSpecificRequirements = null;

            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.SpecificRequirements(internalOrgId, callOffId, model);

            var expected = new SpecificRequirementsModel
            {
                Title = model.Title,
                Caption = model.Caption,
                Advice = model.Advice,
                AdditionalAdvice = model.AdditionalAdvice,
                CallOffId = callOffId,
                ProceedWithoutSpecificRequirements = null,
            };

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink).Excluding(m => m.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SpecificRequirements_DoesNotHaveSpecificRequirements_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            int orderId,
            SpecificRequirementsModel model,
            [Frozen] Mock<IOrderService> orderService,
            [Frozen] Mock<IContractsService> mockContractsService,
            AssociatedServicesBillingController controller)
        {
            model.CallOffId = callOffId;
            model.ProceedWithoutSpecificRequirements = true;

            orderService
                .Setup(x => x.GetOrderId(internalOrgId, callOffId))
                .ReturnsAsync(orderId);

            mockContractsService
                .Setup(c => c.HasSpecificRequirements(orderId, false))
                .Verifiable();

            var result = await controller.SpecificRequirements(internalOrgId, callOffId, model);

            orderService.VerifyAll();
            mockContractsService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SpecificRequirements_HasSpecificRequirements_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            int orderId,
            SpecificRequirementsModel model,
            [Frozen] Mock<IOrderService> orderService,
            [Frozen] Mock<IContractsService> mockContractsService,
            AssociatedServicesBillingController controller)
        {
            model.CallOffId = callOffId;
            model.ProceedWithoutSpecificRequirements = false;

            orderService
                .Setup(x => x.GetOrderId(internalOrgId, callOffId))
                .ReturnsAsync(orderId);

            mockContractsService
                .Setup(c => c.HasSpecificRequirements(orderId, true))
                .Verifiable();

            var result = await controller.SpecificRequirements(internalOrgId, callOffId, model);

            orderService.VerifyAll();
            mockContractsService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(AssociatedServicesBillingController.BespokeRequirements));
            actualResult.ControllerName.Should().Be(typeof(AssociatedServicesBillingController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "fromBespoke", false },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SpecificRequirements_PassesOnQueryParameter_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            int orderId,
            SpecificRequirementsModel model,
            [Frozen] Mock<IOrderService> orderService,
            [Frozen] Mock<IContractsService> mockContractsService,
            AssociatedServicesBillingController controller)
        {
            model.CallOffId = callOffId;
            model.ProceedWithoutSpecificRequirements = false;

            orderService
                .Setup(x => x.GetOrderId(internalOrgId, callOffId))
                .ReturnsAsync(orderId);

            mockContractsService
                .Setup(c => c.HasSpecificRequirements(orderId, true))
                .Verifiable();

            var result = await controller.SpecificRequirements(internalOrgId, callOffId, model, true);

            orderService.VerifyAll();
            mockContractsService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(AssociatedServicesBillingController.BespokeRequirements));
            actualResult.ControllerName.Should().Be(typeof(AssociatedServicesBillingController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "fromBespoke", true },
            });
        }

        [Theory]
        [CommonAutoData]
        public static void Get_BespokeRequirements_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            AssociatedServicesBillingController controller)
        {
            var result = controller.BespokeRequirements(internalOrgId, callOffId);

            var expected = new BasicBillingModel
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }
    }
}
