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
using Microsoft.AspNetCore.Routing;
using Moq;
using MoreLinq.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.ServiceRecipients;
using Xunit;
using ServiceRecipient = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceRecipient;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class ServiceRecipientsControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(ServiceRecipientsController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(ServiceRecipientsController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ServiceRecipientsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData(SelectionMode.None)]
        [CommonInlineAutoData(SelectionMode.All)]
        public static async Task Get_SolutionRecipients_ReturnsExpectedResult(
            SelectionMode? selectionMode,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOdsService> mockOdsService,
            ServiceRecipientsController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            mockOdsService
                .Setup(x => x.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
                .ReturnsAsync(serviceRecipients);

            var result = await controller.SolutionRecipients(internalOrgId, callOffId, selectionMode);

            mockOrderService.VerifyAll();
            mockOdsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var recipients = serviceRecipients
                .Select(x => new ServiceRecipientModel { Name = x.Name, OdsCode = x.OrgId })
                .ToList();

            var expected = new SelectRecipientsModel(recipients, selectionMode)
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                ItemName = order.OrderItems.First().CatalogueItem.Name,
                ItemType = CatalogueItemType.Solution,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SolutionRecipients_WithModelErrors_ReturnsExpectedResult(
            SelectRecipientsModel model,
            ServiceRecipientsController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.SolutionRecipients(model.InternalOrgId, model.CallOffId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SolutionRecipients_ReturnsExpectedResult(
            SelectRecipientsModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOrderItemRecipientService> mockOrderRecipientService,
            ServiceRecipientsController controller)
        {
            IEnumerable<ServiceRecipientDto> serviceRecipients = null;

            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var catalogueItemId = order.OrderItems.First().CatalogueItem.Id;

            mockOrderService
                .Setup(x => x.GetOrderThin(model.CallOffId, model.InternalOrgId))
                .ReturnsAsync(order);

            mockOrderRecipientService
                .Setup(x => x.AddOrderItemRecipients(order.Id, catalogueItemId, It.IsAny<IEnumerable<ServiceRecipientDto>>()))
                .Callback<int, CatalogueItemId, IEnumerable<ServiceRecipientDto>>((_, _, recipients) => serviceRecipients = recipients)
                .Returns(Task.CompletedTask);

            var result = await controller.SolutionRecipients(model.InternalOrgId, model.CallOffId, model);

            mockOrderService.VerifyAll();
            mockOrderRecipientService.VerifyAll();

            var expected = model.ServiceRecipients
                .Where(x => x.Selected)
                .Select(x => x.Dto);

            serviceRecipients.Should().BeEquivalentTo(expected);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", model.InternalOrgId },
                { "callOffId", model.CallOffId },
            });
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData(SelectionMode.None)]
        [CommonInlineAutoData(SelectionMode.All)]
        public static async Task Get_AdditionalServiceRecipients_ReturnsExpectedResult(
            SelectionMode? selectionMode,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOdsService> mockOdsService,
            ServiceRecipientsController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var orderItem = order.OrderItems.First(x => x.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService);
            var catalogueItemId = orderItem.CatalogueItem.Id;

            mockOrderService
                .Setup(x => x.GetOrderSummary(callOffId, internalOrgId))
                .ReturnsAsync(order);

            mockOdsService
                .Setup(x => x.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
                .ReturnsAsync(serviceRecipients);

            var result = await controller.AdditionalServiceRecipients(
                internalOrgId,
                callOffId,
                catalogueItemId,
                selectionMode);

            mockOrderService.VerifyAll();
            mockOdsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var recipients = serviceRecipients
                .Select(x => new ServiceRecipientModel { Name = x.Name, OdsCode = x.OrgId })
                .ToList();

            var expected = new SelectRecipientsModel(recipients, selectionMode)
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CatalogueItemId = catalogueItemId,
                ItemName = orderItem.CatalogueItem.Name,
                ItemType = CatalogueItemType.AdditionalService,
                PreSelected = selectionMode == null,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalServiceRecipients_WithModelErrors_ReturnsExpectedResult(
            SelectRecipientsModel model,
            ServiceRecipientsController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.AdditionalServiceRecipients(
                model.InternalOrgId,
                model.CallOffId,
                model.CatalogueItemId.Value,
                model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalServiceRecipients_ReturnsExpectedResult(
            SelectRecipientsModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOrderItemRecipientService> mockOrderRecipientService,
            ServiceRecipientsController controller)
        {
            IEnumerable<ServiceRecipientDto> serviceRecipients = null;

            mockOrderService
                .Setup(x => x.GetOrderThin(model.CallOffId, model.InternalOrgId))
                .ReturnsAsync(order);

            mockOrderRecipientService
                .Setup(x => x.AddOrderItemRecipients(order.Id, model.CatalogueItemId.Value, It.IsAny<IEnumerable<ServiceRecipientDto>>()))
                .Callback<int, CatalogueItemId, IEnumerable<ServiceRecipientDto>>((_, _, recipients) => serviceRecipients = recipients)
                .Returns(Task.CompletedTask);

            var result = await controller.AdditionalServiceRecipients(
                model.InternalOrgId,
                model.CallOffId,
                model.CatalogueItemId.Value,
                model);

            mockOrderService.VerifyAll();
            mockOrderRecipientService.VerifyAll();

            var expected = model.ServiceRecipients
                .Where(x => x.Selected)
                .Select(x => x.Dto);

            serviceRecipients.Should().BeEquivalentTo(expected);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", model.InternalOrgId },
                { "callOffId", model.CallOffId },
            });
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData(SelectionMode.None)]
        [CommonInlineAutoData(SelectionMode.All)]
        public static async Task Get_AssociatedServiceRecipients_ReturnsExpectedResult(
            SelectionMode? selectionMode,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOdsService> mockOdsService,
            ServiceRecipientsController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var orderItem = order.OrderItems.First(x => x.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService);
            var catalogueItemId = orderItem.CatalogueItem.Id;

            mockOrderService
                .Setup(x => x.GetOrderSummary(callOffId, internalOrgId))
                .ReturnsAsync(order);

            mockOdsService
                .Setup(x => x.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
                .ReturnsAsync(serviceRecipients);

            var result = await controller.AssociatedServiceRecipients(
                internalOrgId,
                callOffId,
                catalogueItemId,
                selectionMode);

            mockOrderService.VerifyAll();
            mockOdsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var recipients = serviceRecipients
                .Select(x => new ServiceRecipientModel { Name = x.Name, OdsCode = x.OrgId })
                .ToList();

            var expected = new SelectRecipientsModel(recipients, selectionMode)
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CatalogueItemId = catalogueItemId,
                ItemName = orderItem.CatalogueItem.Name,
                ItemType = CatalogueItemType.AssociatedService,
                PreSelected = selectionMode == null,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AssociatedServiceRecipients_WithModelErrors_ReturnsExpectedResult(
            SelectRecipientsModel model,
            ServiceRecipientsController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.AssociatedServiceRecipients(
                model.InternalOrgId,
                model.CallOffId,
                model.CatalogueItemId.Value,
                model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AssociatedServiceRecipients_ReturnsExpectedResult(
            SelectRecipientsModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOrderItemRecipientService> mockOrderRecipientService,
            ServiceRecipientsController controller)
        {
            IEnumerable<ServiceRecipientDto> serviceRecipients = null;

            mockOrderService
                .Setup(x => x.GetOrderThin(model.CallOffId, model.InternalOrgId))
                .ReturnsAsync(order);

            mockOrderRecipientService
                .Setup(x => x.AddOrderItemRecipients(order.Id, model.CatalogueItemId.Value, It.IsAny<IEnumerable<ServiceRecipientDto>>()))
                .Callback<int, CatalogueItemId, IEnumerable<ServiceRecipientDto>>((_, _, recipients) => serviceRecipients = recipients)
                .Returns(Task.CompletedTask);

            var result = await controller.AssociatedServiceRecipients(
                model.InternalOrgId,
                model.CallOffId,
                model.CatalogueItemId.Value,
                model);

            mockOrderService.VerifyAll();
            mockOrderRecipientService.VerifyAll();

            var expected = model.ServiceRecipients
                .Where(x => x.Selected)
                .Select(x => x.Dto);

            serviceRecipients.Should().BeEquivalentTo(expected);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", model.InternalOrgId },
                { "callOffId", model.CallOffId },
            });
        }
    }
}
