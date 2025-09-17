using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public static class MergerOrSplitServiceRecipientsControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(MergerOrSplitServiceRecipientsController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(MergerOrSplitServiceRecipientsController).Should()
                .BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            ConstructorInfo[] constructors = typeof(MergerOrSplitServiceRecipientsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInlineAutoData(null, OrderTypeEnum.AssociatedServiceSplit)]
        [MockInlineAutoData(SelectionMode.None, OrderTypeEnum.AssociatedServiceSplit)]
        [MockInlineAutoData(SelectionMode.All, OrderTypeEnum.AssociatedServiceSplit)]
        [MockInlineAutoData(null, OrderTypeEnum.AssociatedServiceMerger)]
        [MockInlineAutoData(SelectionMode.None, OrderTypeEnum.AssociatedServiceMerger)]
        [MockInlineAutoData(SelectionMode.All, OrderTypeEnum.AssociatedServiceMerger)]
        public static async Task Get_SelectServiceRecipients_MergerSplitOther_ReturnsExpectedResult(
            SelectionMode selectionMode,
            OrderTypeEnum orderType,
            Organisation organisation,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IOdsService mockOdsService,
            [Frozen] IOrganisationsService organisationsService,
            MergerOrSplitServiceRecipientsController controller)
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

            var expected = new SelectMergerOrSplitRecipientsModel(
                organisation,
                default,
                orderType,
                recipients,
                [],
                selectionMode);

            actualResult.Model.Should().BeEquivalentTo(expected, x => x
                .Excluding(o => o.Title)
                .Excluding(o => o.BackLink)
                .Excluding(o => o.Caption)
                .Excluding(o => o.Advice)
                .Excluding(o => o.HasImportedRecipients));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectServiceRecipients_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectMergerOrSplitRecipientsModel model,
            MergerOrSplitServiceRecipientsController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.SelectServiceRecipients(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static async Task Post_SelectServiceRecipients_RedirectsTo_SelectRecipientForPracticeReorganisation(
            OrderTypeEnum orderType,
            string internalOrgId,
            CallOffId callOffId,
            SelectMergerOrSplitRecipientsModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            MergerOrSplitServiceRecipientsController controller)
        {
            order.OrderType = orderType;
            var selectedRecipientId = order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode;

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var recipientIds = model
                .GetServiceRecipients()
                .Where(x => x.Selected)
                .Select(x => x.OdsCode)
                .ToRecipientsString();

            var result = await controller.SelectServiceRecipients(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(MergerOrSplitServiceRecipientsController).ControllerName());
            actualResult.ActionName.Should()
                .Be(nameof(MergerOrSplitServiceRecipientsController.SelectRecipientForPracticeReorganisation));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "recipientIds", recipientIds },
                { "selectedRecipientId", selectedRecipientId },
            });
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
            MergerOrSplitServiceRecipientsController controller)
        {
            order.OrderType = orderType;

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockOdsService
                .GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                    internalOrgId,
                    Arg.Any<IEnumerable<string>>())
                .Returns(serviceRecipients);

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
            MergerOrSplitServiceRecipientsController controller)
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
            MergerOrSplitServiceRecipientsController controller)
        {
            var recipientIds = "1,2";
            var result = controller.SelectRecipientForPracticeReorganisation(internalOrgId, callOffId, recipientIds, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(MergerOrSplitServiceRecipientsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(MergerOrSplitServiceRecipientsController.ConfirmChanges));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "recipientIds", recipientIds },
                { "selectedRecipientId", model.SelectedOdsCode },
            });
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
            MergerOrSplitServiceRecipientsController controller)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);
            order.OrderType = orderType;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            orderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var recipientIds = serviceRecipients.Select(r => r.OrgId);
            odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                    internalOrgId,
                    Arg.Is<IEnumerable<string>>(x => Enumerable.ToHashSet(x).SetEquals(recipientIds)))
                .Returns(serviceRecipients);

            odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                    internalOrgId,
                    Arg.Is<IEnumerable<string>>(x => x.SequenceEqual(Enumerable.Empty<string>())))
                .Returns(new List<ServiceRecipient>());

            IActionResult result = await controller.ConfirmChanges(
                internalOrgId,
                callOffId,
                recipientIds.ToRecipientsString(),
                odsCodeNotInListOfRecipients);

            result.Should().BeOfType<BadRequestObjectResult>();
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
            MergerOrSplitServiceRecipientsController controller)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);
            order.OrderType = orderType;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            orderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var recipientIds = serviceRecipients.Select(r => r.OrgId);
            var recipientIdFromList = recipientIds.First();
            odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                    internalOrgId,
                    Arg.Is<IEnumerable<string>>(x => Enumerable.ToHashSet(x).SetEquals(recipientIds)))
                .Returns(serviceRecipients);

            odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                    internalOrgId,
                    Arg.Is<IEnumerable<string>>(x => x.SequenceEqual(Enumerable.Empty<string>())))
                .Returns(new List<ServiceRecipient>());

            var result = await controller.ConfirmChanges(
                internalOrgId,
                callOffId,
                recipientIds.ToRecipientsString(),
                recipientIdFromList);

            var actual = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ConfirmChangesModel
            {
                Title = "Confirm service recipients",
                Caption = $"Order {callOffId}",
                Selected = serviceRecipients
                    .Where(r => r.OrgId != recipientIdFromList)
                    .Select(x => new ServiceRecipientModel { Name = x.Name, OdsCode = x.OrgId, Location = x.Location })
                    .ToList(),
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
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static async Task Post_ConfirmChanges_ReturnsExpectedResult(
            OrderTypeEnum orderType,
            string internalOrgId,
            ConfirmChangesModel model,
            CallOffId callOffId,
            MergerOrSplitServiceRecipientsController controller)
        {
            model.OrderType = orderType;

            var result = await controller.ConfirmChanges(internalOrgId, callOffId, model);

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actual.ActionName.Should().Be(nameof(OrderController.Order));
        }
    }
}
