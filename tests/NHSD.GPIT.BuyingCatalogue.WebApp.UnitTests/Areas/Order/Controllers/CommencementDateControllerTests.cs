using System;
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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.CommencementDate;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class CommencementDateControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(CommencementDateController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(CommencementDateController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CommencementDateController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CommencementDate_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            CommencementDateController controller)
        {
            var expected = new CommencementDateModel(internalOrgId, order);

            orderServiceMock
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var actualResult = await controller.CommencementDate(internalOrgId, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_CommencementDate_WithModelErrors_ExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            CommencementDateController controller)
        {
            var date = DateTime.UtcNow.AddDays(1).Date;

            var model = new CommencementDateModel
            {
                Day = date.Day.ToString(),
                Month = date.Month.ToString(),
                Year = date.Year.ToString(),
                InitialPeriod = $"{3}",
                MaximumTerm = $"{12}",
            };

            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.CommencementDate(internalOrgId, order.CallOffId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.ViewData.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_CommencementDate_ExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderService,
            [Frozen] Mock<ICommencementDateService> commencementDateService,
            CommencementDateController controller)
        {
            var date = DateTime.UtcNow.AddDays(1).Date;
            const int initialPeriod = 3;
            const int maximumTerm = 12;

            var model = new CommencementDateModel
            {
                Day = date.Day.ToString(),
                Month = date.Month.ToString(),
                Year = date.Year.ToString(),
                InitialPeriod = $"{initialPeriod}",
                MaximumTerm = $"{maximumTerm}",
            };

            order.OrderRecipients.SelectMany(x => x.OrderItemRecipients).ForEach(x => x.DeliveryDate = null);

            orderService
                .Setup(x => x.GetOrderWithOrderItems(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            commencementDateService
                .Setup(x => x.SetCommencementDate(order.CallOffId, internalOrgId, date, initialPeriod, maximumTerm))
                .Returns(Task.CompletedTask);

            var result = await controller.CommencementDate(internalOrgId, order.CallOffId, model);

            orderService.VerifyAll();
            commencementDateService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", order.CallOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_CommencementDate_WithClashingDeliveryDates_ExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderService,
            [Frozen] Mock<ICommencementDateService> commencementDateService,
            CommencementDateController controller)
        {
            var date = DateTime.UtcNow.AddDays(1).Date;
            const int initialPeriod = 3;
            const int maximumTerm = 12;

            var model = new CommencementDateModel
            {
                Day = date.Day.ToString(),
                Month = date.Month.ToString(),
                Year = date.Year.ToString(),
                InitialPeriod = $"{initialPeriod}",
                MaximumTerm = $"{maximumTerm}",
            };

            order.OrderItems.ForEach(i => order.OrderRecipients.ForEach(r => r.SetDeliveryDateForItem(i.CatalogueItemId, DateTime.Today)));

            orderService
                .Setup(x => x.GetOrderWithOrderItems(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.CommencementDate(internalOrgId, order.CallOffId, model);

            orderService.VerifyAll();
            commencementDateService.VerifyNoOtherCalls();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            var details = string.Join(
                CommencementDateController.Delimiter,
                model.Date!.Value.ToString(CommencementDateController.DateFormat),
                $"{model.InitialPeriodValue!.Value}",
                $"{model.MaximumTermValue!.Value}");

            actualResult.ActionName.Should().Be(nameof(CommencementDateController.ConfirmChanges));
            actualResult.ControllerName.Should().Be(typeof(CommencementDateController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", order.CallOffId },
                { "details", details },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConfirmChanges_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderService,
            CommencementDateController controller)
        {
            order.CommencementDate = DateTime.Today;
            var dates = order.OrderRecipients.SelectMany(x => x.OrderItemRecipients).ToList();
            dates.ForEach(x => x.DeliveryDate = DateTime.Today);

            orderService
                .Setup(s => s.GetOrderWithOrderItems(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var tomorrow = DateTime.Today.AddDays(1).ToString(CommencementDateController.DateFormat);
            var details = string.Join(CommencementDateController.Delimiter, tomorrow, "3", "12");
            var result = await controller.ConfirmChanges(internalOrgId, order.CallOffId, details);

            orderService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ConfirmChangesModel
            {
                InternalOrgId = internalOrgId,
                CallOffId = order.CallOffId,
                CurrentDate = order.CommencementDate.Value,
                NewDate = DateTime.Today.AddDays(1),
                AffectedPlannedDeliveryDates = dates.Count(),
                TotalPlannedDeliveryDates = dates.Count(),
            };

            actualResult.ViewData.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmChanges_WithModelErrors_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            string details,
            CommencementDateController controller)
        {
            var model = new ConfirmChangesModel
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CurrentDate = DateTime.Today,
                NewDate = DateTime.Today.AddDays(1),
                AffectedPlannedDeliveryDates = 1,
                TotalPlannedDeliveryDates = 2,
            };

            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.ConfirmChanges(internalOrgId, callOffId, details, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.ViewData.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmChanges_ConfirmChangesIsFalse_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            string details,
            CommencementDateController controller)
        {
            var model = new ConfirmChangesModel
            {
                ConfirmChanges = false,
            };

            var result = await controller.ConfirmChanges(internalOrgId, callOffId, details, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(CommencementDateController.CommencementDate));
            actualResult.ControllerName.Should().Be(typeof(CommencementDateController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmChanges_ConfirmChangesIsTrue_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            int initialPeriod,
            int maximumTerm,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<ICommencementDateService> commencementDateService,
            [Frozen] Mock<IOrderService> orderService,
            [Frozen] Mock<IDeliveryDateService> deliveryDateService,
            CommencementDateController controller)
        {
            var newDate = DateTime.Today.AddDays(1);

            var model = new ConfirmChangesModel
            {
                ConfirmChanges = true,
                NewDate = newDate,
            };

            var details = string.Join(
                CommencementDateController.Delimiter,
                DateTime.Today.ToString(CommencementDateController.DateFormat),
                $"{initialPeriod}",
                $"{maximumTerm}");

            commencementDateService
                .Setup(x => x.SetCommencementDate(callOffId, internalOrgId, newDate, initialPeriod, maximumTerm))
                .Verifiable();

            orderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            deliveryDateService
                .Setup(x => x.ResetDeliveryDates(order.Id, newDate))
                .Verifiable();

            var result = await controller.ConfirmChanges(internalOrgId, callOffId, details, model);

            commencementDateService.VerifyAll();
            orderService.VerifyAll();
            deliveryDateService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }
    }
}
