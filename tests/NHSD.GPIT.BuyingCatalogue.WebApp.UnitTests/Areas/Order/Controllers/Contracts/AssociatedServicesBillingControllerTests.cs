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
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.AssociatedServicesBilling;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.Contracts
{
    public static class AssociatedServicesBillingControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AssociatedServicesBillingController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(AssociatedServicesBillingController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
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
            ContractFlags contract,
            List<OrderItem> orderItems,
            ImplementationPlan defaultPlan,
            [Frozen] Mock<IAssociatedServicesBillingService> mockAssociatedServicesBillingService,
            [Frozen] Mock<IContractsService> mockContractsService,
            [Frozen] Mock<IImplementationPlanService> mockImplementationPlanService,
            AssociatedServicesBillingController controller)
        {
            mockAssociatedServicesBillingService
                .Setup(a => a.GetAssociatedServiceOrderItems(internalOrgId, callOffId))
                .ReturnsAsync(orderItems);

            mockImplementationPlanService
                .Setup(i => i.GetDefaultImplementationPlan())
                .ReturnsAsync(defaultPlan);

            mockContractsService.Setup(c => c.GetContract(callOffId.Id))
                .ReturnsAsync(contract);

            var result = await controller.ReviewBilling(internalOrgId, callOffId);

            mockAssociatedServicesBillingService.VerifyAll();
            mockContractsService.VerifyAll();
            mockImplementationPlanService.VerifyAll();

            var expected = new ReviewBillingModel
            {
                CallOffId = callOffId,
                TargetMilestoneName = defaultPlan.Milestones.OrderBy(m => m.Order).LastOrDefault().Title,
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
            model.TargetMilestoneName = defaultPlan.Milestones.OrderBy(m => m.Order).LastOrDefault().Title;
            model.AssociatedServiceOrderItems = null;

            mockAssociatedServicesBillingService
                .Setup(a => a.GetAssociatedServiceOrderItems(internalOrgId, callOffId))
                .ReturnsAsync(orderItems);

            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.ReviewBilling(internalOrgId, callOffId, model);

            mockAssociatedServicesBillingService.VerifyAll();

            var expected = new ReviewBillingModel
            {
                CallOffId = callOffId,
                TargetMilestoneName = defaultPlan.Milestones.OrderBy(m => m.Order).LastOrDefault().Title,
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
            ImplementationPlan defaultPlan,
            ReviewBillingModel model,
            [Frozen] Mock<IContractsService> mockContractsService,
            AssociatedServicesBillingController controller)
        {
            model.CallOffId = callOffId;
            model.UseDefaultBilling = true;
            model.TargetMilestoneName = defaultPlan.Milestones.OrderBy(m => m.Order).LastOrDefault().Title;
            model.AssociatedServiceOrderItems = null;

            mockContractsService.Setup(c => c.UseDefaultBilling(callOffId.Id, true))
                .Verifiable();

            var result = await controller.ReviewBilling(internalOrgId, callOffId, model);

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
            ImplementationPlan defaultPlan,
            ReviewBillingModel model,
            [Frozen] Mock<IContractsService> mockContractsService,
            AssociatedServicesBillingController controller)
        {
            model.CallOffId = callOffId;
            model.UseDefaultBilling = false;
            model.TargetMilestoneName = defaultPlan.Milestones.OrderBy(m => m.Order).LastOrDefault().Title;
            model.AssociatedServiceOrderItems = null;

            mockContractsService.Setup(c => c.UseDefaultBilling(callOffId.Id, false))
                .Verifiable();

            var result = await controller.ReviewBilling(internalOrgId, callOffId, model);

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
            ContractFlags contract,
            [Frozen] Mock<IUrlHelper> urlHelper,
            [Frozen] Mock<IContractsService> mockContractsService,
            AssociatedServicesBillingController controller)
        {
            mockContractsService.Setup(c => c.GetContract(callOffId.Id))
                .ReturnsAsync(contract);

            var result = await controller.SpecificRequirements(internalOrgId, callOffId, false);

            mockContractsService.VerifyAll();

            var expected = new SpecificRequirementsModel
            {
                CallOffId = callOffId,
                HasSpecificRequirements = contract.HasSpecificRequirements,
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
            ContractFlags contract,
            [Frozen] Mock<IUrlHelper> urlHelper,
            [Frozen] Mock<IContractsService> mockContractsService,
            AssociatedServicesBillingController controller)
        {
            mockContractsService.Setup(c => c.GetContract(callOffId.Id))
                .ReturnsAsync(contract);

            var result = await controller.SpecificRequirements(internalOrgId, callOffId, true);

            mockContractsService.VerifyAll();

            var expected = new SpecificRequirementsModel
            {
                CallOffId = callOffId,
                HasSpecificRequirements = contract.HasSpecificRequirements,
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
            model.HasSpecificRequirements = null;

            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.SpecificRequirements(internalOrgId, callOffId, model);

            var expected = new SpecificRequirementsModel
            {
                CallOffId = callOffId,
                HasSpecificRequirements = null,
            };

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink).Excluding(m => m.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SpecificRequirements_DoesNotHaveSpecificRequirments_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SpecificRequirementsModel model,
            [Frozen] Mock<IContractsService> mockContractsService,
            AssociatedServicesBillingController controller)
        {
            model.CallOffId = callOffId;
            model.HasSpecificRequirements = false;

            mockContractsService.Setup(c => c.HasSpecificRequirements(callOffId.Id, false))
                .Verifiable();

            var result = await controller.SpecificRequirements(internalOrgId, callOffId, model);

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
            SpecificRequirementsModel model,
            [Frozen] Mock<IContractsService> mockContractsService,
            AssociatedServicesBillingController controller)
        {
            model.CallOffId = callOffId;
            model.HasSpecificRequirements = true;

            mockContractsService.Setup(c => c.HasSpecificRequirements(callOffId.Id, true))
                .Verifiable();

            var result = await controller.SpecificRequirements(internalOrgId, callOffId, model);

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
            SpecificRequirementsModel model,
            [Frozen] Mock<IContractsService> mockContractsService,
            AssociatedServicesBillingController controller)
        {
            model.CallOffId = callOffId;
            model.HasSpecificRequirements = true;

            mockContractsService.Setup(c => c.HasSpecificRequirements(callOffId.Id, true))
                .Verifiable();

            var result = await controller.SpecificRequirements(internalOrgId, callOffId, model, true);

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
