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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using Xunit;
using ServiceRecipient = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceRecipient;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.SolutionSelection
{
    public static class ServiceRecipientsControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(ServiceRecipientsController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(ServiceRecipientsController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
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
        public static async Task Get_AddServiceRecipients_ReturnsExpectedResult(
            SelectionMode? selectionMode,
            Organisation organisation,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOdsService> mockOdsService,
            [Frozen] Mock<IOrganisationsService> organisationsService,
            ServiceRecipientsController controller)
        {
            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockOdsService
                .Setup(x => x.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
                .ReturnsAsync(serviceRecipients);

            organisationsService
                .Setup(x => x.GetOrganisationByInternalIdentifier(internalOrgId))
                .ReturnsAsync(organisation);

            var result = await controller.SelectServiceRecipients(internalOrgId, callOffId, selectionMode);

            mockOrderService.VerifyAll();
            mockOdsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var recipients = serviceRecipients
                .Select(x => new ServiceRecipientModel
                {
                    Name = x.Name,
                    OdsCode = x.OrgId,
                    Location = x.Location,
                })
                .ToList();

            var expected = new SelectRecipientsModel(
                organisation,
                recipients,
                order.AddedOrderRecipients(null).Select(r => r.OdsCode),
                Enumerable.Empty<string>().ToList(),
                new string[] { },
                selectionMode) { };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x
                .Excluding(o => o.Title)
                .Excluding(o => o.BackLink)
                .Excluding(o => o.Caption)
                .Excluding(o => o.Advice)
                .Excluding(o => o.Advice)
                .Excluding(o => o.ImportRecipientsLink)
                .Excluding(o => o.HasImportedRecipients));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddServiceRecipients_WithImportedSolutionRecipients_ReturnsExpectedResult(
            string internalOrgId,
            Organisation organisation,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            EntityFramework.Ordering.Models.Order amendment,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOdsService> mockOdsService,
            [Frozen] Mock<IOrganisationsService> organisationsService,
            ServiceRecipientsController controller)
        {
            order.Revision = 1;
            amendment.OrderNumber = order.OrderNumber;
            amendment.Revision = 2;

            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var additionalService = order.OrderItems.First();

            var importedRecipients = string.Join(',', order.OrderRecipients.Select(x => x.OdsCode));

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(new[] { order, amendment }));

            mockOdsService
                .Setup(x => x.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
                .ReturnsAsync(serviceRecipients);

            organisationsService
                .Setup(x => x.GetOrganisationByInternalIdentifier(internalOrgId))
                .ReturnsAsync(organisation);

            var result = await controller.SelectServiceRecipients(internalOrgId, callOffId, importedRecipients: importedRecipients);

            mockOrderService.VerifyAll();
            mockOdsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeAssignableTo<SelectRecipientsModel>().Subject;

            model.HasImportedRecipients.Should().BeTrue();
            model.GetServiceRecipients().ForEach(x => x.Selected.Should().BeFalse());
        }

        [Theory]
        [CommonAutoData]
        public static void Post_AddServiceRecipients_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectRecipientsModel model,
            ServiceRecipientsController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = controller.SelectServiceRecipients(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_AddServiceRecipients_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectRecipientsModel model,
            EntityFramework.Ordering.Models.Order order,
            ServiceRecipientsController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var orderItem = order.OrderItems.First().CatalogueItem;

            orderItem.CatalogueItemType = CatalogueItemType.Solution;

            var selectedOdsCodes = model.GetServiceRecipients().Where(x => x.Selected).Select(x => x.OdsCode);
            var recipientIds = string.Join(ServiceRecipientsController.Separator, selectedOdsCodes);

            var result = controller.SelectServiceRecipients(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(ServiceRecipientsController.ConfirmChanges));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "recipientIds", recipientIds },
                { "journey", JourneyType.Add },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConfirmChanges_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] Mock<IOrderService> orderService,
            [Frozen] Mock<IOdsService> odsService,
            ServiceRecipientsController controller)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);
            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            orderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var recipientIds = order.OrderRecipients.First().OdsCode;
            odsService
                .Setup(x => x.GetServiceRecipientsById(internalOrgId, It.Is<IEnumerable<string>>(x => x.SequenceEqual(new[] { recipientIds }))))
                .ReturnsAsync(serviceRecipients);

            odsService
                .Setup(x => x.GetServiceRecipientsById(internalOrgId, It.Is<IEnumerable<string>>(x => x.SequenceEqual(Enumerable.Empty<string>()))))
                .ReturnsAsync(new List<ServiceRecipient>());

            var result = await controller.ConfirmChanges(
                internalOrgId,
                callOffId,
                recipientIds);

            orderService.VerifyAll();
            odsService.VerifyAll();

            var actual = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ConfirmChangesModel()
            {
                Title = "Confirm Service Recipients",
                Caption = $"Order {callOffId}",
                Advice = string.Format(ConfirmChangesModel.AdviceText, solution.CatalogueItem.CatalogueItemType.Name()),
                Selected = serviceRecipients.Select(x => new ServiceRecipientModel { Name = x.Name, OdsCode = x.OrgId, Location = x.Location }).ToList(),
                PreviouslySelected = new List<ServiceRecipientModel>(),
            };

            actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmChanges_ReturnsExpectedResult(
            string internalOrgId,
            ConfirmChangesModel model,
            EntityFramework.Ordering.Models.Order order,
            ServiceRecipientsController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var result = await controller.ConfirmChanges(internalOrgId, order.CallOffId, model);

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actual.ActionName.Should().Be(nameof(OrderController.Order));
        }
    }
}
