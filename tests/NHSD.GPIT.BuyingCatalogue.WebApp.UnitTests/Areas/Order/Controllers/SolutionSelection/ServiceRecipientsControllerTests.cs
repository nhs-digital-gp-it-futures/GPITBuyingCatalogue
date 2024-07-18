using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MoreLinq.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
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
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ServiceRecipientsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData(SelectionMode.None)]
        [MockInlineAutoData(SelectionMode.All)]
        public static async Task Get_SelectServiceRecipients_Solution_ReturnsExpectedResult(
            SelectionMode? selectionMode,
            Organisation organisation,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IOdsService mockOdsService,
            [Frozen] IOrganisationsService organisationsService,
            ServiceRecipientsController controller)
        {
            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockOdsService.GetServiceRecipientsByParentInternalIdentifier(internalOrgId).Returns(serviceRecipients);

            organisationsService.GetOrganisationByInternalIdentifier(internalOrgId).Returns(organisation);

            var result = await controller.SelectServiceRecipients(internalOrgId, callOffId, selectionMode);

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
                selectionMode)
            { };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x
                .Excluding(o => o.Title)
                .Excluding(o => o.BackLink)
                .Excluding(o => o.Caption)
                .Excluding(o => o.Advice)
                .Excluding(o => o.HasImportedRecipients));
        }

        [Theory]
        [MockInlineAutoData(null, OrderTypeEnum.AssociatedServiceSplit, 2)]
        [MockInlineAutoData(SelectionMode.None, OrderTypeEnum.AssociatedServiceSplit, 2)]
        [MockInlineAutoData(SelectionMode.All, OrderTypeEnum.AssociatedServiceSplit, 2)]
        [MockInlineAutoData(null, OrderTypeEnum.AssociatedServiceMerger, 2)]
        [MockInlineAutoData(SelectionMode.None, OrderTypeEnum.AssociatedServiceMerger, 2)]
        [MockInlineAutoData(SelectionMode.All, OrderTypeEnum.AssociatedServiceMerger, 2)]
        [MockInlineAutoData(null, OrderTypeEnum.AssociatedServiceOther, null)]
        [MockInlineAutoData(SelectionMode.None, OrderTypeEnum.AssociatedServiceOther, null)]
        [MockInlineAutoData(SelectionMode.All, OrderTypeEnum.AssociatedServiceOther, null)]
        public static async Task Get_SelectServiceRecipients_MergerSplitOther_ReturnsExpectedResult(
            SelectionMode selectionMode,
            OrderTypeEnum orderType,
            int? atLeast,
            Organisation organisation,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IOdsService mockOdsService,
            [Frozen] IOrganisationsService organisationsService,
            ServiceRecipientsController controller)
        {
            order.OrderType = orderType;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            order.OrderItems = order.OrderItems.Take(1).ToList();

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockOdsService.GetServiceRecipientsByParentInternalIdentifier(internalOrgId).Returns(serviceRecipients);

            organisationsService.GetOrganisationByInternalIdentifier(internalOrgId).Returns(organisation);

            var result = await controller.SelectServiceRecipients(internalOrgId, callOffId, selectionMode);

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
                selectionMode)
            { SelectAtLeast = atLeast };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x
                .Excluding(o => o.Title)
                .Excluding(o => o.BackLink)
                .Excluding(o => o.Caption)
                .Excluding(o => o.Advice)
                .Excluding(o => o.HasImportedRecipients));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SelectServiceRecipients_WithImportedSolutionRecipients_ReturnsExpectedResult(
            string internalOrgId,
            Organisation organisation,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            EntityFramework.Ordering.Models.Order amendment,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IOdsService mockOdsService,
            [Frozen] IOrganisationsService organisationsService,
            ServiceRecipientsController controller)
        {
            order.Revision = 1;
            amendment.OrderNumber = order.OrderNumber;
            amendment.Revision = 2;

            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var additionalService = order.OrderItems.First();

            var importedRecipients = string.Join(',', order.OrderRecipients.Select(x => x.OdsCode));

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(new[] { order, amendment }));

            mockOdsService.GetServiceRecipientsByParentInternalIdentifier(internalOrgId).Returns(serviceRecipients);

            organisationsService.GetOrganisationByInternalIdentifier(internalOrgId).Returns(organisation);

            var result = await controller.SelectServiceRecipients(internalOrgId, callOffId, importedRecipients: importedRecipients);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeAssignableTo<SelectRecipientsModel>().Subject;

            model.HasImportedRecipients.Should().BeTrue();
            model.GetServiceRecipients().ForEach(x => x.Selected.Should().BeFalse());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectServiceRecipients_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectRecipientsModel model,
            ServiceRecipientsController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.SelectServiceRecipients(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.Solution)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        public static async Task Post_SelectServiceRecipients_RedirectsTo_ConfirmChanges(
            OrderTypeEnum orderType,
            string internalOrgId,
            CallOffId callOffId,
            SelectRecipientsModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            ServiceRecipientsController controller)
        {
            order.OrderType = orderType;

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(new[] { order }));

            var selectedOdsCodes = model.GetServiceRecipients().Where(x => x.Selected).Select(x => x.OdsCode);
            var recipientIds = selectedOdsCodes.ToRecipientsString();

            var result = await controller.SelectServiceRecipients(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(ServiceRecipientsController.ConfirmChanges));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "recipientIds", recipientIds },
            });
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static async Task Post_SelectServiceRecipients_RedirectsTo_SelectRecipientForPracticeReorganisation(
            OrderTypeEnum orderType,
            string internalOrgId,
            CallOffId callOffId,
            SelectRecipientsModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            ServiceRecipientsController controller)
        {
            order.OrderType = orderType;
            var selectedRecipientId = order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode;

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(new[] { order }));

            var recipientIds = model
                .GetServiceRecipients()
                .Where(x => x.Selected)
                .Select(x => x.OdsCode)
                .ToRecipientsString();

            var result = await controller.SelectServiceRecipients(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(ServiceRecipientsController.SelectRecipientForPracticeReorganisation));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "recipientIds", recipientIds },
                { "selectedRecipientId", selectedRecipientId },
            });
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.Solution)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        public static async Task Get_SelectRecipientForPracticeReorganisation_With_WrongTypesOfOrders_ReturnsBadRequest(
            OrderTypeEnum orderType,
            string internalOrgId,
            Organisation organisation,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IOrganisationsService organisationsService,
            ServiceRecipientsController controller)
        {
            order.OrderType = orderType;

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(new[] { order }));

            organisationsService.GetOrganisationByInternalIdentifier(internalOrgId).Returns(organisation);

            var result = await controller.SelectRecipientForPracticeReorganisation(internalOrgId, callOffId, default, default(string));

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static async Task Get_SelectRecipientForPracticeReorganisation_With_MergerOrSplit_Returns(
            OrderTypeEnum orderType,
            string internalOrgId,
            string selectedOdsCode,
            Organisation organisation,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IOdsService mockOdsService,
            [Frozen] IOrganisationsService organisationsService,
            ServiceRecipientsController controller)
        {
            order.OrderType = orderType;

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(new[] { order }));

            mockOdsService.GetServiceRecipientsById(internalOrgId, Arg.Any<IEnumerable<string>>()).Returns(serviceRecipients);

            organisationsService.GetOrganisationByInternalIdentifier(internalOrgId).Returns(organisation);

            var result = await controller.SelectRecipientForPracticeReorganisation(internalOrgId, callOffId, string.Empty, selectedOdsCode);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeAssignableTo<RecipientForPracticeReorganisationModel>().Subject;

            model.OrganisationName.Should().Be(organisation.Name);
            model.OrganisationType.Should().Be(organisation.OrganisationType);
            model.SelectedOdsCode.Should().Be(selectedOdsCode);
            model.SubLocations
                .SelectMany(s => s.ServiceRecipients)
                .Select(s => new
                {
                    s.Name,
                    s.OdsCode,
                    s.Location,
                })
                .Should()
                .BeEquivalentTo(serviceRecipients.Select(s => new
                {
                    s.Name,
                    OdsCode = s.OrgId,
                    s.Location,
                }));
        }

        [Theory]
        [MockAutoData]
        public static void Post_SelectRecipientForPracticeReorganisation_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            RecipientForPracticeReorganisationModel model,
            ServiceRecipientsController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = controller.SelectRecipientForPracticeReorganisation(internalOrgId, callOffId, string.Empty, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static void Post_SelectRecipientForPracticeReorganisation_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            RecipientForPracticeReorganisationModel model,
            ServiceRecipientsController controller)
        {
            var recipientIds = "1,2";
            var result = controller.SelectRecipientForPracticeReorganisation(internalOrgId, callOffId, recipientIds, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(ServiceRecipientsController.ConfirmChanges));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "recipientIds", recipientIds },
                { "selectedRecipientId", model.SelectedOdsCode },
            });
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.Solution)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        public static async Task Get_ConfirmChanges_ReturnsExpectedResult(
            OrderTypeEnum orderType,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] IOrderService orderService,
            [Frozen] IOdsService odsService,
            ServiceRecipientsController controller)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);
            order.OrderType = orderType;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            orderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var recipientIds = serviceRecipients.Select(r => r.OrgId);
            odsService.GetServiceRecipientsById(internalOrgId, Arg.Is<IEnumerable<string>>(x => Enumerable.ToHashSet(x).SetEquals(recipientIds))).Returns(serviceRecipients);

            odsService.GetServiceRecipientsById(internalOrgId, Arg.Is<IEnumerable<string>>(x => x.SequenceEqual(Enumerable.Empty<string>()))).Returns(new List<ServiceRecipient>());

            var result = await controller.ConfirmChanges(
                internalOrgId,
                callOffId,
                recipientIds.ToRecipientsString(),
                string.Empty);

            var actual = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ConfirmChangesModel()
            {
                Title = "Confirm Service Recipients",
                Caption = $"Order {callOffId}",
                Selected = serviceRecipients.Select(x => new ServiceRecipientModel { Name = x.Name, OdsCode = x.OrgId, Location = x.Location }).ToList(),
                PreviouslySelected = new List<ServiceRecipientModel>(),
                OrderType = orderType,
            };

            actual.Model.Should().BeEquivalentTo(expected, x => x
                .Excluding(o => o.Advice)
                .Excluding(o => o.BackLink)
                .Excluding(o => o.AddRemoveRecipientsLink));
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static async Task Get_ConfirmChanges_MergerOrSplit_Throws(
            OrderTypeEnum orderType,
            string internalOrgId,
            string odsCodeNotInListOfRecipients,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] IOrderService orderService,
            [Frozen] IOdsService odsService,
            ServiceRecipientsController controller)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);
            order.OrderType = orderType;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            orderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var recipientIds = serviceRecipients.Select(r => r.OrgId);
            odsService.GetServiceRecipientsById(internalOrgId, Arg.Is<IEnumerable<string>>(x => Enumerable.ToHashSet(x).SetEquals(recipientIds))).Returns(serviceRecipients);

            odsService.GetServiceRecipientsById(internalOrgId, Arg.Is<IEnumerable<string>>(x => x.SequenceEqual(Enumerable.Empty<string>()))).Returns(new List<ServiceRecipient>());

            await FluentActions.Invoking(async () => await controller.ConfirmChanges(
                internalOrgId,
                callOffId,
                recipientIds.ToRecipientsString(),
                odsCodeNotInListOfRecipients))
                .Should()
                .ThrowAsync<InvalidOperationException>();
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static async Task Get_ConfirmChanges_MergerOrSplit_ReturnsExpectedResult(
            OrderTypeEnum orderType,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] IOrderService orderService,
            [Frozen] IOdsService odsService,
            ServiceRecipientsController controller)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);
            order.OrderType = orderType;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            orderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var recipientIds = serviceRecipients.Select(r => r.OrgId);
            var recipientIdFromList = recipientIds.First();
            odsService.GetServiceRecipientsById(internalOrgId, Arg.Is<IEnumerable<string>>(x => Enumerable.ToHashSet(x).SetEquals(recipientIds))).Returns(serviceRecipients);

            odsService.GetServiceRecipientsById(internalOrgId, Arg.Is<IEnumerable<string>>(x => x.SequenceEqual(Enumerable.Empty<string>()))).Returns(new List<ServiceRecipient>());

            var result = await controller.ConfirmChanges(
                internalOrgId,
                callOffId,
                recipientIds.ToRecipientsString(),
                recipientIdFromList);

            var actual = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ConfirmChangesModel()
            {
                Title = "Confirm Service Recipients",
                Caption = $"Order {callOffId}",
                Selected = serviceRecipients
                    .Where(r => r.OrgId != recipientIdFromList)
                    .Select(x => new ServiceRecipientModel { Name = x.Name, OdsCode = x.OrgId, Location = x.Location }).ToList(),
                PreviouslySelected = new List<ServiceRecipientModel>(),
                PracticeReorganisationRecipient = serviceRecipients
                    .Where(r => r.OrgId == recipientIdFromList)
                    .Select(x => new ServiceRecipientModel { Name = x.Name, OdsCode = x.OrgId, Location = x.Location })
                    .First(),
                OrderType = orderType,
            };

            actual.Model.Should().BeEquivalentTo(expected, x => x
                .Excluding(o => o.Advice)
                .Excluding(o => o.BackLink)
                .Excluding(o => o.AddRemoveRecipientsLink));
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.Solution)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static async Task Post_ConfirmChanges_ReturnsExpectedResult(
            OrderTypeEnum orderType,
            string internalOrgId,
            ConfirmChangesModel model,
            CallOffId callOffId,
            ServiceRecipientsController controller)
        {
            model.OrderType = orderType;

            var result = await controller.ConfirmChanges(internalOrgId, callOffId, model);

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actual.ActionName.Should().Be(nameof(OrderController.Order));
        }

        [Theory]
        [MockAutoData]
        public static void UploadOrSelectServiceRecipients_Get_ReturnsViewWithModel(
        string internalOrgId,
        CallOffId callOffId,
        ServiceRecipientsController controller)
        {
            var result = controller.UploadOrSelectServiceRecipients(internalOrgId, callOffId);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<UploadOrSelectServiceRecipientModel>().Subject;

            model.Should().NotBeNull();
            model.Caption.Should().Be($"Order {callOffId}");
            model.BackLink.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static void UploadOrSelectServiceRecipients_Post_InvalidModel_ReturnsViewWithModel(
            UploadOrSelectServiceRecipientModel model,
            string internalOrgId,
            CallOffId callOffId,
            ServiceRecipientsController controller)
        {
            controller.ModelState.AddModelError("SomeError", "Error message");

            var result = controller.UploadOrSelectServiceRecipients(model, internalOrgId, callOffId);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var returnedModel = viewResult.Model.Should().BeOfType<UploadOrSelectServiceRecipientModel>().Subject;

            returnedModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static void UploadOrSelectServiceRecipients_Post_UploadRecipients_RedirectsToImportController(
            UploadOrSelectServiceRecipientModel model,
            string internalOrgId,
            CallOffId callOffId,
            ServiceRecipientsController controller)
        {
            model.ShouldUploadRecipients = true;

            var result = controller.UploadOrSelectServiceRecipients(model, internalOrgId, callOffId);

            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be(nameof(ImportServiceRecipientsController.Index));
            redirectToActionResult.ControllerName.Should().Be(typeof(ImportServiceRecipientsController).ControllerName());
        }

        [Theory]
        [MockAutoData]
        public static void UploadOrSelectServiceRecipients_Post_DoNotUploadRecipients_RedirectsToSelectServiceRecipientsAction(
            UploadOrSelectServiceRecipientModel model,
            string internalOrgId,
            CallOffId callOffId,
            ServiceRecipientsController controller)
        {
            model.ShouldUploadRecipients = false;

            var result = controller.UploadOrSelectServiceRecipients(model, internalOrgId, callOffId);

            var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be(nameof(ServiceRecipientsController.SelectServiceRecipients));
            redirectToActionResult.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
        }
    }
}
