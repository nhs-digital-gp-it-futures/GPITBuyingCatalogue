using System;
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
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.Contracts
{
    public static class DeliveryDatesControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(DeliveryDatesController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(DeliveryDatesController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(DeliveryDatesController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectDate_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderService,
            DeliveryDatesController controller)
        {
            orderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            var result = await controller.SelectDate(internalOrgId, callOffId);

            orderService.VerifyAll();

            var expected = new SelectDateModel(internalOrgId, callOffId, order.CommencementDate!.Value, order.DeliveryDate);
            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectDate_WithDeliveryDate_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            DateTime deliveryDate,
            [Frozen] Mock<IOrderService> orderService,
            DeliveryDatesController controller)
        {
            orderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            var result = await controller.SelectDate(
                internalOrgId,
                callOffId,
                deliveryDate.ToString(DeliveryDatesController.DateFormat));

            orderService.VerifyAll();

            var expected = new SelectDateModel(internalOrgId, callOffId, order.CommencementDate!.Value, deliveryDate);
            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectDate_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectDateModel model,
            DeliveryDatesController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.SelectDate(internalOrgId, callOffId, model);
            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.Model.Should().BeEquivalentTo(model, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectDate_OrderDeliveryDateIsNull_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectDateModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderService,
            DeliveryDatesController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.DeliveryDate = null;

            orderService
                .Setup(x => x.SetDeliveryDate(internalOrgId, callOffId, model.Date.Value))
                .Verifiable();

            orderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            var result = await controller.SelectDate(internalOrgId, callOffId, model);

            orderService.VerifyAll();

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(DeliveryDatesController.EditDates));
            actual.ControllerName.Should().Be(typeof(DeliveryDatesController).ControllerName());
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", order.OrderItems.First().CatalogueItemId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectDate_NewDateSameAsOldDate_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectDateModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderService,
            DeliveryDatesController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.DeliveryDate = model.Date;

            orderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            var result = await controller.SelectDate(internalOrgId, callOffId, model);

            orderService.VerifyAll();

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(DeliveryDatesController.EditDates));
            actual.ControllerName.Should().Be(typeof(DeliveryDatesController).ControllerName());
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", order.OrderItems.First().CatalogueItemId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectDate_NewDateDifferentFromOldDate_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectDateModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderService,
            DeliveryDatesController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.DeliveryDate = model.Date!.Value.AddDays(1);

            orderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            var result = await controller.SelectDate(internalOrgId, callOffId, model);

            orderService.VerifyAll();

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(DeliveryDatesController.ConfirmChanges));
            actual.ControllerName.Should().Be(typeof(DeliveryDatesController).ControllerName());
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "deliveryDate", model.Date.Value.ToString(DeliveryDatesController.DateFormat) },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConfirmChanges_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderService,
            DeliveryDatesController controller)
        {
            orderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            var newDate = order.DeliveryDate!.Value.Date.AddDays(1);

            var result = await controller.ConfirmChanges(
                internalOrgId,
                callOffId,
                newDate.ToString(DeliveryDatesController.DateFormat));

            orderService.VerifyAll();

            var expected = new ConfirmChangesModel(internalOrgId, callOffId, order.DeliveryDate.Value, newDate);
            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmChanges_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmChangesModel model,
            DeliveryDatesController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.ConfirmChanges(internalOrgId, callOffId, model);
            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.Model.Should().BeEquivalentTo(model, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmChanges_NoSelected_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmChangesModel model,
            DeliveryDatesController controller)
        {
            model.ConfirmChanges = false;

            var result = await controller.ConfirmChanges(internalOrgId, callOffId, model);
            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(OrderController.Order));
            actual.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmChanges_YesSelected_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmChangesModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderService,
            DeliveryDatesController controller)
        {
            model.ConfirmChanges = true;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            orderService
                .Setup(x => x.SetDeliveryDate(internalOrgId, callOffId, model.NewDeliveryDate))
                .Verifiable();

            orderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            var result = await controller.ConfirmChanges(internalOrgId, callOffId, model);

            orderService.VerifyAll();

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(DeliveryDatesController.EditDates));
            actual.ControllerName.Should().Be(typeof(DeliveryDatesController).ControllerName());
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", order.OrderItems.First().CatalogueItemId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditDates_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderService,
            DeliveryDatesController controller)
        {
            order.OrderItems.ForEach(x => x.OrderItemRecipients.ForEach(r => r.Recipient = new ServiceRecipient { Name = "Recipient Name" }));
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var catalogueItemId = order.OrderItems.First().CatalogueItemId;

            orderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(order);

            var result = await controller.EditDates(internalOrgId, callOffId, catalogueItemId);

            orderService.VerifyAll();

            var expected = new EditDatesModel(order, catalogueItemId);
            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditDates_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            EditDatesModel model,
            DeliveryDatesController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.EditDates(internalOrgId, callOffId, catalogueItemId, model);
            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.Model.Should().BeEquivalentTo(model, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditDates_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            EditDatesModel model,
            [Frozen] Mock<IOrderService> orderService,
            DeliveryDatesController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var catalogueItemId = order.OrderItems.First().CatalogueItemId;

            orderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            var deliveryDates = new List<OrderDeliveryDateDto>();

            orderService
                .Setup(x => x.SetDeliveryDates(order.Id, catalogueItemId, It.IsAny<List<OrderDeliveryDateDto>>()))
                .Callback<int, CatalogueItemId, List<OrderDeliveryDateDto>>((_, _, x) => deliveryDates = x);

            var result = await controller.EditDates(internalOrgId, callOffId, catalogueItemId, model);

            orderService.VerifyAll();

            deliveryDates.Count.Should().Be(model.Recipients.Length);

            foreach (var deliveryDate in deliveryDates)
            {
                var recipient = model.Recipients.Single(x => x.OdsCode == deliveryDate.OdsCode);
                recipient.Date.Should().Be(deliveryDate.DeliveryDate);
            }

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(OrderController.Order));
            actual.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }
    }
}
