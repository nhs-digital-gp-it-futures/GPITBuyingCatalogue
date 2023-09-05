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
            order.AssociatedServicesOnly = false;
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
                .Excluding(o => o.BackLink)
                .Excluding(o => o.Caption)
                .Excluding(o => o.Advice)
                .Excluding(o => o.Advice)
                .Excluding(o => o.ImportRecipientsLink)
                .Excluding(o => o.HasImportedRecipients));
        }

        // [Theory]
        // [CommonAutoData]
        // public static async Task Get_AddServiceRecipients_WithPreSelectedSolutionRecipients_ReturnsExpectedResult(
        //     string internalOrgId,
        //     Organisation organisation,
        //     CallOffId callOffId,
        //     EntityFramework.Ordering.Models.Order order,
        //     List<ServiceRecipient> serviceRecipients,
        //     [Frozen] Mock<IOrderService> mockOrderService,
        //     [Frozen] Mock<IOdsService> mockOdsService,
        //     [Frozen] Mock<IOrganisationsService> organisationsService,
        //     ServiceRecipientsController controller)
        // {
        //     order.AssociatedServicesOnly = false;
        //     order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

        //     var solution = order.OrderItems.First();
        //     var additionalService = order.OrderItems.ElementAt(1);

        //     solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

        //     for (var i = 0; i < 3; i++)
        //     {
        //         solution.OrderItemRecipients.ElementAt(i).OdsCode = serviceRecipients[i].OrgId;
        //     }

        //     mockOrderService
        //         .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
        //         .ReturnsAsync(new OrderWrapper(order));

        //     mockOdsService
        //         .Setup(x => x.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
        //         .ReturnsAsync(serviceRecipients);

        //     organisationsService
        //         .Setup(x => x.GetOrganisationByInternalIdentifier(internalOrgId))
        //         .ReturnsAsync(organisation);

        //     var result = await controller.AddServiceRecipients(internalOrgId, callOffId, additionalService.CatalogueItemId);

        //     mockOrderService.VerifyAll();
        //     mockOdsService.VerifyAll();

        //     var actualResult = result.Should().BeOfType<ViewResult>().Subject;
        //     var model = actualResult.Model.Should().BeAssignableTo<SelectRecipientsModel>().Subject;

        //     model.PreSelected.Should().BeTrue();
        //     model.GetServiceRecipients().ForEach(x => x.Selected.Should().BeTrue());
        // }

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

            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var additionalService = order.OrderItems.First();

            //for (var i = 0; i < 3; i++)
            //{
            //    additionalService.OrderItemRecipients.ElementAt(i).OdsCode = serviceRecipients[i].OrgId;
            // }

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
            //model.HasMissingImportedRecipients.Should().BeTrue();
            //model.PreSelected.Should().BeFalse();
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

        // [Theory]
        // [CommonInlineAutoData(null)]
        // [CommonInlineAutoData(SelectionMode.None)]
        // [CommonInlineAutoData(SelectionMode.All)]
        // public static async Task Get_EditServiceRecipients_ReturnsExpectedResult(
        //     SelectionMode? selectionMode,
        //     Organisation organisation,
        //     string internalOrgId,
        //     CallOffId callOffId,
        //     EntityFramework.Ordering.Models.Order order,
        //     List<ServiceRecipient> serviceRecipients,
        //     [Frozen] Mock<IOrderService> mockOrderService,
        //     [Frozen] Mock<IOdsService> mockOdsService,
        //     [Frozen] Mock<IOrganisationsService> organisationsService,
        //     ServiceRecipientsController controller)
        // {
        //     order.AssociatedServicesOnly = false;
        //     order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

        //     var solution = order.OrderItems.First();

        //     solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

        //     mockOrderService
        //         .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
        //         .ReturnsAsync(new OrderWrapper(order));

        //     mockOdsService
        //         .Setup(x => x.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
        //         .ReturnsAsync(serviceRecipients);

        //     organisationsService
        //         .Setup(x => x.GetOrganisationByInternalIdentifier(internalOrgId))
        //         .ReturnsAsync(organisation);

        //     var result = await controller.EditServiceRecipients(internalOrgId, callOffId, solution.CatalogueItemId, selectionMode);

        //     mockOrderService.VerifyAll();
        //     mockOdsService.VerifyAll();

        //     var actualResult = result.Should().BeOfType<ViewResult>().Subject;

        //     var recipients = serviceRecipients
        //         .Select(x => new ServiceRecipientModel
        //         {
        //             Name = x.Name,
        //             OdsCode = x.OrgId,
        //             Location = x.Location,
        //         })
        //         .ToList();

        //     var expected = new SelectRecipientsModel(organisation, solution, null, recipients, selectionMode)
        //     {
        //         InternalOrgId = internalOrgId,
        //         CallOffId = callOffId,
        //         CatalogueItemId = solution.CatalogueItemId,
        //         IsAdding = false,
        //     };

        //     actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        // }

        // [Theory]
        // [CommonAutoData]
        // public static async Task Get_EditServiceRecipients_WithImportedSolutionRecipients_ReturnsExpectedResult(
        //     string internalOrgId,
        //     Organisation organisation,
        //     CallOffId callOffId,
        //     EntityFramework.Ordering.Models.Order order,
        //     EntityFramework.Ordering.Models.Order amendment,
        //     List<ServiceRecipient> serviceRecipients,
        //     [Frozen] Mock<IOrderService> mockOrderService,
        //     [Frozen] Mock<IOdsService> mockOdsService,
        //     [Frozen] Mock<IOrganisationsService> organisationsService,
        //     ServiceRecipientsController controller)
        // {
        //     order.Revision = 1;
        //     amendment.OrderNumber = order.OrderNumber;
        //     amendment.Revision = 2;

        //     order.AssociatedServicesOnly = false;
        //     order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

        //     var additionalService = order.OrderItems.First();

        //     for (var i = 0; i < 3; i++)
        //     {
        //         additionalService.OrderItemRecipients.ElementAt(i).OdsCode = serviceRecipients[i].OrgId;
        //     }

        //     var importedRecipients = string.Join(',', additionalService.OrderItemRecipients.Select(x => x.OdsCode));

        //     mockOrderService
        //         .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
        //         .ReturnsAsync(new OrderWrapper(new[] { order, amendment }));

        //     mockOdsService
        //         .Setup(x => x.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
        //         .ReturnsAsync(serviceRecipients);

        //     organisationsService
        //         .Setup(x => x.GetOrganisationByInternalIdentifier(internalOrgId))
        //         .ReturnsAsync(organisation);

        //     var result = await controller.EditServiceRecipients(internalOrgId, callOffId, additionalService.CatalogueItemId, importedRecipients: importedRecipients);

        //     mockOrderService.VerifyAll();
        //     mockOdsService.VerifyAll();

        //     var actualResult = result.Should().BeOfType<ViewResult>().Subject;
        //     var model = actualResult.Model.Should().BeAssignableTo<SelectRecipientsModel>().Subject;

        //     model.HasImportedRecipients.Should().BeTrue();
        //     model.HasMissingImportedRecipients.Should().BeTrue();
        //     model.PreSelected.Should().BeFalse();
        //     model.GetServiceRecipients().ForEach(x => x.Selected.Should().BeFalse());
        // }

        // [Theory]
        // [CommonAutoData]
        // public static void Post_EditServiceRecipients_WithModelErrors_ReturnsExpectedResult(
        //     SelectRecipientsModel model,
        //     ServiceRecipientsController controller)
        // {
        //     controller.ModelState.AddModelError("key", "errorMessage");

        //     var result = controller.EditServiceRecipients(model.InternalOrgId, model.CallOffId, model.CatalogueItemId, model);

        //     var actualResult = result.Should().BeOfType<ViewResult>().Subject;

        //     actualResult.Model.Should().BeEquivalentTo(model);
        // }

        // [Theory]
        // [CommonAutoData]
        // public static void Post_EditServiceRecipients_ReturnsExpectedResult(
        //     SelectRecipientsModel model,
        //     EntityFramework.Ordering.Models.Order order,
        //     ServiceRecipientsController controller)
        // {
        //     order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

        //     var orderItem = order.OrderItems.First().CatalogueItem;

        //     orderItem.CatalogueItemType = CatalogueItemType.Solution;

        //     var selectedOdsCodes = model.GetServiceRecipients().Where(x => x.Selected).Select(x => x.OdsCode);
        //     var recipientIds = string.Join(ServiceRecipientsController.Separator, selectedOdsCodes);

        //     var result = controller.EditServiceRecipients(model.InternalOrgId, model.CallOffId, orderItem.Id, model);

        //     var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

        //     actualResult.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
        //     actualResult.ActionName.Should().Be(nameof(ServiceRecipientsController.ConfirmChanges));
        //     actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
        //     {
        //         { "internalOrgId", model.InternalOrgId },
        //         { "callOffId", model.CallOffId },
        //         { "catalogueItemId", orderItem.Id },
        //         { "recipientIds", recipientIds },
        //         { "journey", JourneyType.Edit },
        //         { "source", model.Source },
        //     });
        // }

        // [Theory]
        // [CommonAutoData]
        // public static void Post_EditServiceRecipients_NoSelectedRecipients_ReturnsExpectedResult(
        //     SelectRecipientsModel model,
        //     ServiceRecipientsController controller)
        // {
        //     model.GetServiceRecipients().ForEach(x => x.Selected = false);

        //     var result = controller.EditServiceRecipients(model.InternalOrgId, model.CallOffId, model.CatalogueItemId, model);

        //     var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

        //     actualResult.ControllerName.Should().Be(typeof(TaskListController).ControllerName());
        //     actualResult.ActionName.Should().Be(nameof(TaskListController.TaskList));
        //     actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
        //     {
        //         { "internalOrgId", model.InternalOrgId },
        //         { "callOffId", model.CallOffId },
        //     });
        // }

        [Theory]
        //[CommonInlineAutoData(JourneyType.Add)]
        //[CommonInlineAutoData(JourneyType.Edit)]
        [CommonAutoData]
        public static async Task Get_ConfirmChanges_ReturnsExpectedResult(
            // JourneyType journeyType,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            //RoutingResult routingResult,
            [Frozen] Mock<IOrderService> orderService,
            [Frozen] Mock<IOdsService> odsService,
            // [Frozen] Mock<IRoutingService> routingService,
            ServiceRecipientsController controller)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);
            order.AssociatedServicesOnly = false;
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

              //   .ToList() ?? Enumerable.Empty<string>();
            //RouteValues routeValues = null;
            // routingService
            //     .Setup(x => x.GetRoute(RoutingPoint.ConfirmServiceRecipientsBackLink, order, It.IsAny<RouteValues>()))
            //     .Callback<RoutingPoint, EntityFramework.Ordering.Models.Order, RouteValues>((_, _, x) => routeValues = x)
            //     .Returns(routingResult);

            var result = await controller.ConfirmChanges(
                internalOrgId,
                callOffId,
                recipientIds);

            orderService.VerifyAll();
            odsService.VerifyAll();

            // routingService.VerifyAll();
            // routeValues.Should().BeEquivalentTo(new RouteValues(internalOrgId, callOffId, solution.CatalogueItemId)
            // {
            //     Journey = journeyType,
            //     RecipientIds = recipientIds,
            // });

            var actual = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ConfirmChangesModel(order.OrderingParty)
            {
                Caption = callOffId.ToString(), // solution.CatalogueItem.Name,
                Advice = string.Format(ConfirmChangesModel.AdviceText, solution.CatalogueItem.CatalogueItemType.Name()),
                // Journey = journeyType,
                // Selected = new List<ServiceRecipientModel>(),
                Selected = serviceRecipients.Select(x => new ServiceRecipientModel { Name = x.Name, OdsCode = x.OrgId, Location = x.Location }).ToList(),
                PreviouslySelected = new List<ServiceRecipientModel>(),
            };

            actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmChanges_ReturnsExpectedResult(
            string internalOrgId,
            // CatalogueItemId catalogueItemId,
            ConfirmChangesModel model,
            EntityFramework.Ordering.Models.Order order,
            //RoutingResult routingResult,
            //[Frozen] Mock<IOrderService> orderService,
            //[Frozen] Mock<IOrderItemService> orderItemService,
            // [Frozen] Mock<IOrderItemRecipientService> orderItemRecipientService,
            //[Frozen] Mock<IRoutingService> routingService,
            ServiceRecipientsController controller)
        {
            // orderService
            //     .Setup(x => x.GetOrderWithCatalogueItemAndPrices(order.CallOffId, internalOrgId))
            //     .ReturnsAsync(new OrderWrapper(order));

            // IEnumerable<CatalogueItemId> catalogueItemIds = null;

            // orderItemService
            //     .Setup(x => x.CopyOrderItems(internalOrgId, order.CallOffId, It.IsAny<IEnumerable<CatalogueItemId>>()))
            //     .Callback<string, CallOffId, IEnumerable<CatalogueItemId>>((_, _, x) => catalogueItemIds = x);

            // List<ServiceRecipientDto> serviceRecipientDtos = null;

            //orderItemRecipientService
            //    .Setup(x => x.UpdateOrderItemRecipients(order.Id, catalogueItemId, It.IsAny<List<ServiceRecipientDto>>()))
            //    .Callback<int, CatalogueItemId, List<ServiceRecipientDto>>((_, _, x) => serviceRecipientDtos = x);

            // RouteValues routeValues = null;
            // routingService
            //     .Setup(x => x.GetRoute(RoutingPoint.ConfirmServiceRecipients, order, It.IsAny<RouteValues>()))
            //     .Callback<RoutingPoint, EntityFramework.Ordering.Models.Order, RouteValues>((_, _, x) => routeValues = x)
            //     .Returns(routingResult);

            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var result = await controller.ConfirmChanges(internalOrgId, order.CallOffId, model);

            //orderService.VerifyAll();
            //orderItemService.VerifyAll();
            //orderItemRecipientService.VerifyAll();
            //routingService.VerifyAll();

            // catalogueItemIds.Should().BeEquivalentTo(new[] { catalogueItemId });
            //serviceRecipientDtos.Should().BeEquivalentTo(model.Selected.Select(x => x.Dto).ToList());
            //routeValues.Should().BeEquivalentTo(new RouteValues(internalOrgId, order.CallOffId, catalogueItemId) { Source = model.Source });

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actual.ActionName.Should().Be( nameof(OrderController.Order));
        }
    }
}
