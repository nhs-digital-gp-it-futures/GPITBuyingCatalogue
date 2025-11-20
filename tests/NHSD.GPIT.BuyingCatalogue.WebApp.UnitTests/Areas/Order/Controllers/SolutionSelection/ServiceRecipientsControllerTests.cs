using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using NSubstitute.ReturnsExtensions;
using Xunit;
using EntityOdsOrganisation = NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models.OdsOrganisation;
using ServiceContractOdsOrganisation = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations.OdsOrganisation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.SolutionSelection
{
    public static class ServiceRecipientsControllerTests
    {
        private const int CommonOrderId = 865;
        private const int CommonOrganisationId = 21;
        private const int CommonOrderNumber = 10001;
        private const string CommonOrganisationInternalIdentifier = "BB-FFGG";
        private const string CommonOrganisationExternalIdentifier = "FFGG";

        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(ServiceRecipientsController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(ServiceRecipientsController).Should()
                .BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            ConstructorInfo[] constructors = typeof(ServiceRecipientsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static void UploadOrSelectServiceRecipients_Get_ReturnsViewWithModel(
            string internalOrgId,
            CallOffId callOffId,
            ServiceRecipientsController controller)
        {
            IActionResult result = controller.UploadOrSelectServiceRecipients(internalOrgId, callOffId);

            ViewResult viewResult = result.Should().BeOfType<ViewResult>().Subject;
            UploadOrSelectServiceRecipientModel model = viewResult.Model.Should()
                .BeOfType<UploadOrSelectServiceRecipientModel>()
                .Subject;

            model.Should().NotBeNull();
            model.Caption.Should().Be($"Order {callOffId}");
            model.BackLink.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task UploadOrSelectServiceRecipients_Post_InvalidModel_ReturnsViewWithModel(
            UploadOrSelectServiceRecipientModel model,
            string internalOrgId,
            CallOffId callOffId,
            ServiceRecipientsController controller)
        {
            controller.ModelState.AddModelError("SomeError", "Error message");

            IActionResult result = await controller.UploadOrSelectServiceRecipients(model, internalOrgId, callOffId);

            ViewResult viewResult = result.Should().BeOfType<ViewResult>().Subject;
            UploadOrSelectServiceRecipientModel returnedModel =
                viewResult.Model.Should().BeOfType<UploadOrSelectServiceRecipientModel>().Subject;

            returnedModel.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task UploadOrSelectServiceRecipients_Post_UploadRecipients_RedirectsToImportController(
            UploadOrSelectServiceRecipientModel model,
            string internalOrgId,
            CallOffId callOffId,
            ServiceRecipientsController controller)
        {
            model.ShouldUploadRecipients = true;

            IActionResult result = await controller.UploadOrSelectServiceRecipients(model, internalOrgId, callOffId);

            RedirectToActionResult redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should().Be(nameof(ImportServiceRecipientsController.Index));
            redirectToActionResult.ControllerName.Should()
                .Be(typeof(ImportServiceRecipientsController).ControllerName());
        }

        [Theory]
        [MockAutoData]
        public static async Task
            UploadOrSelectServiceRecipients_Post_DoNotUploadRecipients_RedirectsToSelectSublocationAction(
                UploadOrSelectServiceRecipientModel model,
                string internalOrgId,
                CallOffId callOffId,
                [Frozen] IOrderService orderService,
                ServiceRecipientsController controller)
        {
            model.ShouldUploadRecipients = false;

            orderService.GetOrderHasAnySublocations(callOffId, internalOrgId).Returns(false);

            IActionResult result = await controller.UploadOrSelectServiceRecipients(model, internalOrgId, callOffId);

            RedirectToActionResult redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should()
                .Be(nameof(ServiceRecipientsController.SelectSublocations));
            redirectToActionResult.ControllerName.Should()
                .Be(typeof(ServiceRecipientsController).ControllerName());
        }

        [Theory]
        [MockAutoData]
        public static async Task
            UploadOrSelectServiceRecipients_Post_DoNotUploadRecipients_HasSublocations_RedirectsToConfirmSublocationAction(
                UploadOrSelectServiceRecipientModel model,
                string internalOrgId,
                CallOffId callOffId,
                [Frozen] IOrderService orderService,
                ServiceRecipientsController controller)
        {
            model.ShouldUploadRecipients = false;

            orderService.GetOrderHasAnySublocations(callOffId, internalOrgId).Returns(true);

            IActionResult result = await controller.UploadOrSelectServiceRecipients(model, internalOrgId, callOffId);

            RedirectToActionResult redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirectToActionResult.ActionName.Should()
                .Be(nameof(ServiceRecipientsController.ConfirmSublocations));
            redirectToActionResult.ControllerName.Should()
                .Be(typeof(ServiceRecipientsController).ControllerName());
        }

        public static IEnumerable<object[]> ExistingAndNewSublocationsToRenderedSublocations()
        {
            return
            [
                // 2 Existing sublocations that should be ticked, and a new one that shouldn't
                [
                    CommonOrganisationFactory(),
                    CommonOrderFactory(),
                    new List<OrderSublocation>
                    {
                        CommonOrderSublocationFactory(
                            "XXXX",
                            [
                                CommonOrderSublocationRecipientFactory("AAAA", "XXXX"),
                                CommonOrderSublocationRecipientFactory("AAAB", "XXXX"),
                            ]),
                        CommonOrderSublocationFactory(
                            "XXXA",
                            [
                                CommonOrderSublocationRecipientFactory("AAAC", "XXXA"),
                            ]),
                    },
                    new List<OdsOrganisation>
                    {
                        new() { OdsCode = "XXXX", OrganisationName = "An existing sublocation - XXXX" },
                        new() { OdsCode = "XXXA", OrganisationName = "An existing sublocation - XXXA" },
                        new() { OdsCode = "XXXE", OrganisationName = "A new sublocation - XXXE" },
                    },
                    new List<SelectOption<string>>
                    {
                        new() { Text = "XXXX", Value = "XXXX", Selected = true },
                        new() { Text = "XXXA", Value = "XXXA", Selected = true },
                        new() { Text = "XXXE", Value = "XXXE", Selected = true },
                    },
                ],

                // 3 Existing sublocations with no new - should all be ticked
                [
                    CommonOrganisationFactory(),
                    CommonOrderFactory(),
                    new List<OrderSublocation>
                    {
                        CommonOrderSublocationFactory(
                            "XXXX",
                            [
                                CommonOrderSublocationRecipientFactory("AAAA", "XXXX"),
                                CommonOrderSublocationRecipientFactory("AAAB", "XXXX"),
                            ]),
                        CommonOrderSublocationFactory(
                            "XXXA",
                            [
                                CommonOrderSublocationRecipientFactory("AAAC", "XXXA"),
                            ]),
                        CommonOrderSublocationFactory("XXXE"),
                    },
                    new List<OdsOrganisation>
                    {
                        new() { OdsCode = "XXXX", OrganisationName = "An existing sublocation - XXXX" },
                        new() { OdsCode = "XXXA", OrganisationName = "An existing sublocation - XXXA" },
                        new() { OdsCode = "XXXE", OrganisationName = "A new sublocation - XXXE" },
                    },
                    new List<SelectOption<string>>
                    {
                        new() { Text = "XXXX", Value = "XXXX", Selected = true },
                        new() { Text = "XXXA", Value = "XXXA", Selected = true },
                        new() { Text = "XXXE", Value = "XXXE", Selected = true },
                    },
                ],

                // 3 all new sublocations - none should be ticked
                [
                    CommonOrganisationFactory(),
                    CommonOrderFactory(),
                    new List<OrderSublocation>(),
                    new List<OdsOrganisation>
                    {
                        new() { OdsCode = "XXXX", OrganisationName = "An existing sublocation - XXXX" },
                        new() { OdsCode = "XXXA", OrganisationName = "An existing sublocation - XXXA" },
                        new() { OdsCode = "XXXE", OrganisationName = "A new sublocation - XXXE" },
                    },
                    new List<SelectOption<string>>
                    {
                        new() { Text = "XXXX", Value = "XXXX", Selected = false },
                        new() { Text = "XXXA", Value = "XXXA", Selected = false },
                        new() { Text = "XXXE", Value = "XXXE", Selected = false },
                    },
                ],
            ];
        }

        [Theory]
        [MockMemberAutoData(nameof(ExistingAndNewSublocationsToRenderedSublocations))]
        public static async Task SelectSublocations_ReturnsViewWithModel(
            Organisation organisation,
            EntityFramework.Ordering.Models.Order order,
            List<OrderSublocation> existingSublocations,
            List<ServiceContractOdsOrganisation> possibleSublocations,
            List<SelectOption<string>> renderedSelections,
            [Frozen] IOrderService ordersService,
            [Frozen] IOdsService odsService,
            ServiceRecipientsController controller)
        {
            order.OrderSublocations = existingSublocations;
            order.OrderingParty = organisation;

            ordersService.GetOrderWithSublocations(order.CallOffId, organisation.InternalIdentifier)
                .Returns(new OrderWrapper(order));

            odsService.GetSublocationsByParentOdsCode(organisation.ExternalIdentifier).Returns(possibleSublocations);

            var expectedModel =
                new SelectSublocationsModel { RenderedSublocations = renderedSelections, IsAmendment = false };

            var result = (await controller.SelectSublocations(organisation.InternalIdentifier, order.CallOffId))
                .As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should()
                .BeEquivalentTo(
                    expectedModel,
                    opt => opt.Excluding(m => m.BackLink)
                        .Excluding(m => m.Title)
                        .Excluding(m => m.Caption)
                        .Excluding(m => m.Advice)
                        .Excluding(m => m.FormLabelText));
        }

        [Theory]
        [MockAutoData]
        public static async Task SelectSublocations_Post_ErrorStateRejectsRequest(
            string internalOrgId,
            CallOffId orderId,
            SelectSublocationsModel selectSublocationsModel,
            ServiceRecipientsController controller)
        {
            controller.ModelState.AddModelError("some-property", "some-error");

            var result =
                (await controller.SelectSublocations(selectSublocationsModel, internalOrgId, orderId))
                .As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().Be(selectSublocationsModel);
        }

        public static IEnumerable<object[]> ExpectedSetsAndRemoves()
        {
            return
            [
                // 2 existing, 1 unticked and 2 ticked resulting in 2 sublocations and 1 remove
                [
                    CommonOrganisationFactory(),
                    CommonOrderFactory(),
                    new List<OrderSublocation>
                    {
                        CommonOrderSublocationFactory("XXXX"), CommonOrderSublocationFactory("XXXA"),
                    },
                    new List<SelectOption<string>>
                    {
                        new() { Text = "XXXX", Value = "XXXX", Selected = false },
                        new() { Text = "XXXA", Value = "XXXA", Selected = true },
                        new() { Text = "XXXE", Value = "XXXE", Selected = true },
                    },
                    "XXXA,XXXE",
                    "XXXX",
                ],

                // 3 existing and 1 ticked resulting in 1 sublocation and 2 removes
                [
                    CommonOrganisationFactory(),
                    CommonOrderFactory(),
                    new List<OrderSublocation>
                    {
                        CommonOrderSublocationFactory("XXXX"),
                        CommonOrderSublocationFactory("XXXA"),
                        CommonOrderSublocationFactory("XXXE"),
                    },
                    new List<SelectOption<string>>
                    {
                        new() { Text = "XXXX", Value = "XXXX", Selected = false },
                        new() { Text = "XXXA", Value = "XXXA", Selected = false },
                        new() { Text = "XXXE", Value = "XXXE", Selected = true },
                    },
                    "XXXE",
                    "XXXX,XXXA",
                ],

                // 3 existing and 0 ticked resulting in 0 sublocations and 3 removes
                [
                    CommonOrganisationFactory(),
                    CommonOrderFactory(),
                    new List<OrderSublocation>
                    {
                        CommonOrderSublocationFactory("XXXX"),
                        CommonOrderSublocationFactory("XXXA"),
                        CommonOrderSublocationFactory("XXXE"),
                    },
                    new List<SelectOption<string>>
                    {
                        new() { Text = "XXXX", Value = "XXXX", Selected = false },
                        new() { Text = "XXXA", Value = "XXXA", Selected = false },
                        new() { Text = "XXXE", Value = "XXXE", Selected = false },
                    },
                    string.Empty,
                    "XXXX,XXXA,XXXE",
                ],
            ];
        }

        [Theory]
        [MockMemberAutoData(nameof(ExpectedSetsAndRemoves))]
        public static async Task SelectSublocations_Post_SetsAndRemoves_RedirectsToRemovePage(
            Organisation organisation,
            EntityFramework.Ordering.Models.Order order,
            List<OrderSublocation> existingSublocations,
            List<SelectOption<string>> checkboxSelections,
            string sublocationsConcatString,
            string removesConcatString,
            [Frozen] IOrderService ordersService,
            ServiceRecipientsController controller)
        {
            order.OrderSublocations = existingSublocations;
            order.OrderingParty = organisation;

            ordersService.GetOrderWithSublocations(order.CallOffId, organisation.InternalIdentifier)
                .Returns(new OrderWrapper(order));

            var callingModel = new SelectSublocationsModel { RenderedSublocations = checkboxSelections };

            var result =
                (await controller.SelectSublocations(callingModel, organisation.InternalIdentifier, order.CallOffId))
                .As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.RemoveSublocations));
            result.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary
                    {
                        { "internalOrgId", organisation.InternalIdentifier },
                        { "callOffId", order.CallOffId },
                        { "sublocations", sublocationsConcatString },
                        { "removes", removesConcatString },
                    });
        }

        public static IEnumerable<object[]> RemoveSublocationsUrlParamsToModel()
        {
            return
            [
                // No removes
                [
                    "AAAA,AAAB,AAAC",
                    string.Empty,
                    new RemoveSublocationsModel
                    {
                        SublocationOdsCodes = ["AAAA", "AAAB", "AAAC"],
                        Removes = [],
                        Pluralisation = "sublocations",
                        Title = "Remove sublocations",
                        Advice = "Confirm you want to remove sublocations from this order",
                    },
                ],

                // Remove
                [
                    "AAAA,AAAB",

                    "AAAC",

                    new RemoveSublocationsModel
                    {
                        SublocationOdsCodes = ["AAAA", "AAAB"],
                        Removes = ["AAAC"],
                        Pluralisation = "sublocation",
                        Title = "Remove sublocation",
                        Advice = "Confirm you want to remove sublocations from this order",
                    },
                ],

                // Removes
                [
                    "AAAA",

                    "AAAB,AAAC",

                    new RemoveSublocationsModel
                    {
                        SublocationOdsCodes = ["AAAA"],
                        Removes = ["AAAB", "AAAC"],
                        Pluralisation = "sublocations",
                        Title = "Remove sublocations",
                        Advice = "Confirm you want to remove sublocations from this order",
                    },
                ],
            ];
        }

        [Theory]
        [MockMemberAutoData(nameof(RemoveSublocationsUrlParamsToModel))]
        public static async Task RemoveSublocations_ReturnsView(
            string sublocations,
            string removes,
            RemoveSublocationsModel expectedModel,
            Organisation organisation,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService ordersService,
            ServiceRecipientsController controller)
        {
            order.OrderingParty = organisation;

            ordersService.GetOrderThin(order.CallOffId, organisation.InternalIdentifier)
                .Returns(new OrderWrapper(order));

            var result =
                (await controller.RemoveSublocations(
                    organisation.InternalIdentifier,
                    order.CallOffId,
                    sublocations,
                    removes))
                .As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should()
                .BeEquivalentTo(
                    expectedModel,
                    opt => opt
                        .Excluding(m => m.BackLink)
                        .Excluding(m => m.Caption)
                        .Excluding(m => m.ListHeaderText));

            var modelForFurtherComparison = result.Model.As<RemoveSublocationsModel>();

            modelForFurtherComparison.ListHeaderText.Should()
                .Be($"{order.OrderingParty.Name} {expectedModel.Pluralisation} to be removed:");
            modelForFurtherComparison.Title.Should().Be($"Remove {expectedModel.Pluralisation}");
            modelForFurtherComparison.Caption.Should().Be(order.CallOffId.ToString());
            modelForFurtherComparison.Advice.Should().Be("Confirm you want to remove sublocations from this order");
        }

        [Theory]
        [MockAutoData]
        public static async Task RemoveSublocations_Post_NoServiceCallsIfNo(
            string internalOrganisationId,
            CallOffId orderId,
            List<string> sublocationOdsCodes,
            List<string> sublocationOdsCodesToDisplayForRemove,
            [Frozen] IOrderService ordersService,
            ServiceRecipientsController controller)
        {
            var callingModel =
                new RemoveSublocationsModel
                {
                    ConfirmRemove = false,
                    SublocationOdsCodes = sublocationOdsCodes,
                    Removes = sublocationOdsCodesToDisplayForRemove,
                };

            IActionResult result = await controller.RemoveSublocations(callingModel, internalOrganisationId, orderId);

            await ordersService.DidNotReceiveWithAnyArgs().SetSublocations(default, string.Empty, null);

            result.As<RedirectToActionResult>()
                .ActionName
                .Should()
                .BeEquivalentTo(nameof(controller.ConfirmSublocations));
        }

        [Theory]
        [MockAutoData]
        public static async Task RemoveSublocations_Post_NoServiceCallsAndBadRequestNotPopulated(
            string internalOrganisationId,
            CallOffId orderId,
            [Frozen] IOrderService ordersService,
            ServiceRecipientsController controller)
        {
            var callingModel =
                new RemoveSublocationsModel { ConfirmRemove = true, Removes = [], SublocationOdsCodes = [] };

            var result =
                (await controller.RemoveSublocations(callingModel, internalOrganisationId, orderId))
                .As<BadRequestResult>();

            await ordersService.DidNotReceiveWithAnyArgs().SetSublocations(default, string.Empty, null);

            result.Should().BeOfType<BadRequestResult>();
        }

        [Theory]
        [MockAutoData]
        public static async Task RemoveSublocations_Post_PerformsServiceCallsAndRedirects(
            string internalOrganisationId,
            CallOffId orderId,
            List<string> sublocationOdsCodes,
            List<string> sublocationOdsCodesToDisplayForRemove,
            [Frozen] IOrderService ordersService,
            ServiceRecipientsController controller)
        {
            var callingModel =
                new RemoveSublocationsModel
                {
                    ConfirmRemove = true,
                    SublocationOdsCodes = sublocationOdsCodes,
                    Removes = sublocationOdsCodesToDisplayForRemove,
                };

            var result =
                (await controller.RemoveSublocations(callingModel, internalOrganisationId, orderId))
                .As<RedirectToActionResult>();

            await ordersService.Received()
                .SetSublocations(
                    orderId,
                    internalOrganisationId,
                    Arg.Is<HashSet<string>>(hs => AreStringHashSetsEquivalent(hs, sublocationOdsCodes.ToHashSet())));

            result.ActionName.Should().Be(nameof(controller.ConfirmSublocations));
            result.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrganisationId }, { "callOffId", orderId } });
        }

        [Theory]
        [MockAutoData]
        public static async Task SelectSublocationRecipients_SelectionMode_ReturnsNotFound(
            string internalOrgId,
            CallOffId orderId,
            string sublocationOdsCode,
            [Frozen] IOrganisationsService organisationsService,
            ServiceRecipientsController controller)
        {
            organisationsService.GetOrganisationExternalIdentifierByInternalIdentifier(internalOrgId).ReturnsNull();

            var result =
                (await controller.SelectSublocationRecipients(
                    internalOrgId,
                    orderId,
                    sublocationOdsCode)).As<NotFoundResult>();

            result.Should().NotBeNull();
        }

        public static IEnumerable<object[]> SelectionModesAndExpectedResults()
        {
            List<ServiceRecipient> possibleRecipientRepo =
            [
                CommonServiceRecipientFactory("AAAA", "XXXX"),
                CommonServiceRecipientFactory("AAAB", "XXXX"),
                CommonServiceRecipientFactory("AAAC", "XXXX"),
            ];

            return
            [
                // Repo + all mode = 3 selected
                [
                    CommonOrganisationFactory(), CommonOrderFactory(),
                    CommonOrderSublocationFactory(
                        "XXXX",
                        []),
                    possibleRecipientRepo,

                    new List<SelectOption<string>>
                    {
                        new(string.Empty, "AAAA", true),
                        new(string.Empty, "AAAB", true),
                        new(string.Empty, "AAAC", true),
                    },
                    SelectionMode.All,
                ],

                // Repo + 1 selected all mode = 3 selected
                [
                    CommonOrganisationFactory(), CommonOrderFactory(),
                    CommonOrderSublocationFactory(
                        "XXXX",
                        [
                            CommonOrderSublocationRecipientFactory("AAAA", "XXXX"),
                        ]),
                    possibleRecipientRepo,

                    new List<SelectOption<string>>
                    {
                        new(string.Empty, "AAAA", true),
                        new(string.Empty, "AAAB", true),
                        new(string.Empty, "AAAC", true),
                    },
                    SelectionMode.All,
                ],

                // Repo + 3 selected all mode = 3 selected
                [
                    CommonOrganisationFactory(), CommonOrderFactory(),
                    CommonOrderSublocationFactory(
                        "XXXX",
                        [
                            CommonOrderSublocationRecipientFactory("AAAA", "XXXX"),
                            CommonOrderSublocationRecipientFactory("AAAB", "XXXX"),
                            CommonOrderSublocationRecipientFactory("AAAC", "XXXX"),
                        ]),
                    possibleRecipientRepo,

                    new List<SelectOption<string>>
                    {
                        new(string.Empty, "AAAA", true),
                        new(string.Empty, "AAAB", true),
                        new(string.Empty, "AAAC", true),
                    },
                    SelectionMode.All,
                ],

                // Repo + 0 selected none mode = 0 selected
                [
                    CommonOrganisationFactory(), CommonOrderFactory(),
                    CommonOrderSublocationFactory(
                        "XXXX",
                        []),
                    possibleRecipientRepo,

                    new List<SelectOption<string>>
                    {
                        new(string.Empty, "AAAA", false),
                        new(string.Empty, "AAAB", false),
                        new(string.Empty, "AAAC", false),
                    },
                    SelectionMode.None,
                ],

                // Repo + 1 selected none mode = 0 selected
                [
                    CommonOrganisationFactory(), CommonOrderFactory(),
                    CommonOrderSublocationFactory(
                        "XXXX",
                        [CommonOrderSublocationRecipientFactory("AAAA", "XXXX")]),
                    possibleRecipientRepo,

                    new List<SelectOption<string>>
                    {
                        new(string.Empty, "AAAA", false),
                        new(string.Empty, "AAAB", false),
                        new(string.Empty, "AAAC", false),
                    },
                    SelectionMode.None,
                ],

                // Repo + 3 selected none mode = 0 selected
                [
                    CommonOrganisationFactory(), CommonOrderFactory(),
                    CommonOrderSublocationFactory(
                        "XXXX",
                        [
                            CommonOrderSublocationRecipientFactory("AAAA", "XXXX"),
                            CommonOrderSublocationRecipientFactory("AAAB", "XXXX"),
                            CommonOrderSublocationRecipientFactory("AAAC", "XXXX"),
                        ]),
                    possibleRecipientRepo,

                    new List<SelectOption<string>>
                    {
                        new(string.Empty, "AAAA", false),
                        new(string.Empty, "AAAB", false),
                        new(string.Empty, "AAAC", false),
                    },
                    SelectionMode.None,
                ],
            ];
        }

        [Theory]
        [MockMemberAutoData(nameof(SelectionModesAndExpectedResults))]
        public static async Task SelectSublocationRecipients_SelectionMode_ReturnsViewAsExpected(
            Organisation organisation,
            EntityFramework.Ordering.Models.Order order,
            OrderSublocation workingSublocation,
            List<ServiceRecipient> possibleRecipients,
            List<SelectOption<string>> expectedRendered,
            SelectionMode? selectionMode,
            [Frozen] IOrderSublocationService orderSublocationService,
            [Frozen] IOrganisationsService organisationsService,
            [Frozen] IOdsService odsOrganisationsService,
            [Frozen] IOrderService orderService,
            ServiceRecipientsController controller)
        {
            order.OrderingPartyId = organisation.Id;
            order.OrderingParty = organisation;

            workingSublocation.Order = order;

            organisationsService.GetOrganisationExternalIdentifierByInternalIdentifier(organisation.InternalIdentifier)
                .Returns(organisation.ExternalIdentifier);

            orderSublocationService.GetOrderSublocationWithRecipients(
                    organisation.ExternalIdentifier,
                    order.Id,
                    workingSublocation.SublocationOdsCode)
                .Returns(workingSublocation);

            odsOrganisationsService.GetServiceRecipientsBySublocation(workingSublocation.SublocationOdsCode)
                .Returns(possibleRecipients);

            orderService.GetOrderId(order.CallOffId).Returns(order.Id);

            var expectedModel = new SelectSublocationRecipientsModel
            {
                SublocationName = workingSublocation.SublocationOrganisation?.Name,
                RenderedServiceRecipients = expectedRendered,
                SelectionMode = selectionMode,
            };

            var result =
                (await controller.SelectSublocationRecipients(
                    organisation.InternalIdentifier,
                    order.CallOffId,
                    workingSublocation.SublocationOdsCode,
                    selectionMode))
                .As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should()
                .BeEquivalentTo(
                    expectedModel,
                    opt => opt.Excluding(m => m.Title)
                        .Excluding(m => m.Caption)
                        .Excluding(m => m.Advice)
                        .Excluding(m => m.BackLink)
                        .Excluding(m => m.RenderedServiceRecipients));

            IReadOnlyList<SelectOption<string>> renderedRecipientsForFurtherEvaluation =
                result.Model.As<SelectSublocationRecipientsModel>().RenderedServiceRecipients;

            renderedRecipientsForFurtherEvaluation.Should()
                .BeEquivalentTo(expectedModel.RenderedServiceRecipients);
        }

        public static IEnumerable<object[]> PreviousSelectionsAndPotentialRecipientsToExpected()
        {
            List<ServiceRecipient> possibleRecipientRepo =
            [
                CommonServiceRecipientFactory("AAAA", "XXXX"),
                CommonServiceRecipientFactory("AAAB", "XXXX"),
                CommonServiceRecipientFactory("AAAC", "XXXX"),
            ];

            return
            [
                // 1 existing + repo = 3 rendered with 1 existing selected
                [
                    CommonOrganisationFactory(), CommonOrderFactory(),
                    CommonOrderSublocationFactory(
                        "XXXX",
                        [CommonOrderSublocationRecipientFactory("AAAA", "XXXX")]),
                    possibleRecipientRepo,

                    new List<SelectOption<string>>
                    {
                        new(string.Empty, "AAAA", true),
                        new(string.Empty, "AAAB", false),
                        new(string.Empty, "AAAC", false),
                    },
                ],

                // 0 existing + repo = 3 rendered with 0 existing selected
                [
                    CommonOrganisationFactory(), CommonOrderFactory(),
                    CommonOrderSublocationFactory(
                        "XXXX",
                        []),
                    possibleRecipientRepo,

                    new List<SelectOption<string>>
                    {
                        new(string.Empty, "AAAA", false),
                        new(string.Empty, "AAAB", false),
                        new(string.Empty, "AAAC", false),
                    },
                ],

                // 3 existing + repo = 3 rendered with 3 existing selected
                [
                    CommonOrganisationFactory(), CommonOrderFactory(),
                    CommonOrderSublocationFactory(
                        "XXXX",
                        [
                            CommonOrderSublocationRecipientFactory("AAAA", "XXXX"),
                            CommonOrderSublocationRecipientFactory("AAAB", "XXXX"),
                            CommonOrderSublocationRecipientFactory("AAAC", "XXXX"),
                        ]),
                    possibleRecipientRepo,

                    new List<SelectOption<string>>
                    {
                        new(string.Empty, "AAAA", true),
                        new(string.Empty, "AAAB", true),
                        new(string.Empty, "AAAC", true),
                    },
                ],

                // 3 existing + empty repo = 3 rendered with 3 existing selected
                [
                    CommonOrganisationFactory(), CommonOrderFactory(),
                    CommonOrderSublocationFactory(
                        "XXXX",
                        [
                            CommonOrderSublocationRecipientFactory("AAAA", "XXXX"),
                            CommonOrderSublocationRecipientFactory("AAAB", "XXXX"),
                            CommonOrderSublocationRecipientFactory("AAAC", "XXXX"),
                        ]),
                    new List<ServiceRecipient>(),

                    new List<SelectOption<string>>
                    {
                        new(string.Empty, "AAAA", true),
                        new(string.Empty, "AAAB", true),
                        new(string.Empty, "AAAC", true),
                    },
                ],

                // 3 existing (not in repo) + repo = 6 rendered with 3 existing selected
                [
                    CommonOrganisationFactory(), CommonOrderFactory(),
                    CommonOrderSublocationFactory(
                        "XXXX",
                        [
                            CommonOrderSublocationRecipientFactory("BAAA", "XXXX"),
                            CommonOrderSublocationRecipientFactory("BAAB", "XXXX"),
                            CommonOrderSublocationRecipientFactory("BAAC", "XXXX"),
                        ]),
                    possibleRecipientRepo,

                    new List<SelectOption<string>>
                    {
                        new(string.Empty, "AAAA", false),
                        new(string.Empty, "AAAB", false),
                        new(string.Empty, "AAAC", false),
                        new(string.Empty, "BAAA", true),
                        new(string.Empty, "BAAB", true),
                        new(string.Empty, "BAAC", true),
                    },
                ],
            ];
        }

        [Theory]
        [MockMemberAutoData(nameof(PreviousSelectionsAndPotentialRecipientsToExpected))]
        public static async Task
            SelectSublocationRecipients_PreviousSelectionsScenarios_ReturnsViewWithSelectionsAsExpected(
                Organisation organisation,
                EntityFramework.Ordering.Models.Order order,
                OrderSublocation workingSublocation,
                List<ServiceRecipient> possibleRecipients,
                List<SelectOption<string>> expectedRendered,
                [Frozen] IOrderSublocationService orderSublocationService,
                [Frozen] IOrganisationsService organisationsService,
                [Frozen] IOdsService odsOrganisationsService,
                [Frozen] IOrderService orderService,
                ServiceRecipientsController controller)
        {
            await SelectSublocationRecipients_SelectionMode_ReturnsViewAsExpected(
                organisation,
                order,
                workingSublocation,
                possibleRecipients,
                expectedRendered,
                null,
                orderSublocationService,
                organisationsService,
                odsOrganisationsService,
                orderService,
                controller);
        }

        public static IEnumerable<object[]> AmendmentSelectionModesAndExpectedResults()
        {
            List<ServiceRecipient> possibleRecipientRepo =
            [
                CommonServiceRecipientFactory("AAAA", "XXXX"),
                CommonServiceRecipientFactory("AAAB", "XXXX"),
                CommonServiceRecipientFactory("AAAC", "XXXX"),
            ];

            return
            [
                // Current logic will copy previous recipients to the current order so they need to be included in both
                // 1 in previous order = 2 visible for selection
                [
                    CommonOrganisationFactory(), CommonOrderFactory(0, 0, 0, 1),
                    CommonOrderSublocationFactory("XXXX", [CommonOrderSublocationRecipientFactory("AAAA", "XXXX")]),
                    CommonOrderFactory(0, 0, 0, 2),
                    CommonOrderSublocationFactory(
                        "XXXX",
                        [CommonOrderSublocationRecipientFactory("AAAA", "XXXX")]),
                    possibleRecipientRepo,

                    new List<SelectOption<string>>
                    {
                        new(string.Empty, "AAAA", true, true),
                        new(string.Empty, "AAAB", false),
                        new(string.Empty, "AAAC", false),
                    },
                    null,
                ],

                // 2 in previous order = 1 visible for selection
                [
                    CommonOrganisationFactory(), CommonOrderFactory(0, 0, 0, 1),
                    CommonOrderSublocationFactory("XXXX", [CommonOrderSublocationRecipientFactory("AAAA", "XXXX")]),
                    CommonOrderFactory(0, 0, 0, 2),
                    CommonOrderSublocationFactory(
                        "XXXX",
                        [CommonOrderSublocationRecipientFactory("AAAA", "XXXX")]),
                    possibleRecipientRepo,

                    new List<SelectOption<string>>
                    {
                        new(string.Empty, "AAAA", true, true),
                        new(string.Empty, "AAAB", false),
                        new(string.Empty, "AAAC", false),
                    },
                    null,
                ],

                // Select all selects all remaining recipients
                [
                    CommonOrganisationFactory(), CommonOrderFactory(0, 0, 0, 1),
                    CommonOrderSublocationFactory("XXXX", [CommonOrderSublocationRecipientFactory("AAAA", "XXXX")]),
                    CommonOrderFactory(0, 0, 0, 2),
                    CommonOrderSublocationFactory(
                        "XXXX",
                        [CommonOrderSublocationRecipientFactory("AAAA", "XXXX")]),
                    possibleRecipientRepo,

                    new List<SelectOption<string>>
                    {
                        new(string.Empty, "AAAA", true, true),
                        new(string.Empty, "AAAB", true),
                        new(string.Empty, "AAAC", true),
                    },
                    SelectionMode.All,
                ],

                // Select none clears current order selection but keeps previous order
                [
                    CommonOrganisationFactory(), CommonOrderFactory(0, 0, 0, 1),
                    CommonOrderSublocationFactory("XXXX", [CommonOrderSublocationRecipientFactory("AAAA", "XXXX")]),
                    CommonOrderFactory(0, 0, 0, 2),
                    CommonOrderSublocationFactory(
                        "XXXX",
                        [
                            CommonOrderSublocationRecipientFactory("AAAA", "XXXX"),
                            CommonOrderSublocationRecipientFactory("AAAC", "XXXX"),
                        ]),
                    possibleRecipientRepo,

                    new List<SelectOption<string>>
                    {
                        new(string.Empty, "AAAA", true, true),
                        new(string.Empty, "AAAB", false),
                        new(string.Empty, "AAAC", false),
                    },
                    SelectionMode.None,
                ],
            ];
        }

        [Theory]
        [MockMemberAutoData(nameof(AmendmentSelectionModesAndExpectedResults))]
        public static async Task AmendmentSelectSublocationRecipients_SelectionMode_ReturnsViewAsExpected(
            Organisation organisation,
            EntityFramework.Ordering.Models.Order previousOrder,
            OrderSublocation previousWorkingSublocation,
            EntityFramework.Ordering.Models.Order order,
            OrderSublocation workingSublocation,
            List<ServiceRecipient> possibleRecipients,
            List<SelectOption<string>> expectedRendered,
            SelectionMode? selectionMode,
            [Frozen] IOrderSublocationService orderSublocationService,
            [Frozen] IOrganisationsService organisationsService,
            [Frozen] IOdsService odsOrganisationsService,
            [Frozen] IOrderService orderService,
            ServiceRecipientsController controller)
        {
            previousOrder.OrderingPartyId = organisation.Id;
            previousOrder.OrderingParty = organisation;
            previousOrder.OrderSublocations = [previousWorkingSublocation];
            previousWorkingSublocation.Order = previousOrder;

            order.OrderingPartyId = organisation.Id;
            order.OrderingParty = organisation;

            workingSublocation.Order = order;

            var orderWrapper = new OrderWrapper(order, [previousOrder]);

            organisationsService.GetOrganisationExternalIdentifierByInternalIdentifier(organisation.InternalIdentifier)
                .Returns(organisation.ExternalIdentifier);

            orderSublocationService.GetOrderSublocationWithRecipients(
                    organisation.ExternalIdentifier,
                    order.Id,
                    workingSublocation.SublocationOdsCode)
                .Returns(workingSublocation);

            odsOrganisationsService.GetServiceRecipientsBySublocation(workingSublocation.SublocationOdsCode)
                .Returns(possibleRecipients);

            orderService
                .GetOrderWithSublocationsAndSublocationRecipients(order.CallOffId, organisation.InternalIdentifier)
                .Returns(orderWrapper);

            orderService.GetOrderId(order.CallOffId).Returns(order.Id);

            var expectedModel = new SelectSublocationRecipientsModel
            {
                IsAmendment = true,
                SublocationName = workingSublocation.SublocationOrganisation?.Name,
                RenderedServiceRecipients = expectedRendered,
                SelectionMode = selectionMode,
            };

            var result =
                (await controller.SelectSublocationRecipients(
                    organisation.InternalIdentifier,
                    order.CallOffId,
                    workingSublocation.SublocationOdsCode,
                    selectionMode))
                .As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should()
                .BeEquivalentTo(
                    expectedModel,
                    opt => opt.Excluding(m => m.Title)
                        .Excluding(m => m.Caption)
                        .Excluding(m => m.Advice)
                        .Excluding(m => m.BackLink)
                        .Excluding(m => m.RenderedServiceRecipients));

            IReadOnlyList<SelectOption<string>> renderedRecipientsForFurtherEvaluation =
                result.Model.As<SelectSublocationRecipientsModel>().RenderedServiceRecipients;

            renderedRecipientsForFurtherEvaluation.Should()
                .BeEquivalentTo(expectedModel.RenderedServiceRecipients);
        }

        [Theory]
        [MockAutoData]
        public static async Task AmendmentSelectSublocationRecipients_NoNewRecipientsAvailable_ReturnsViewAsExpected(
            Organisation organisation,
            [Frozen] IOrderSublocationService orderSublocationService,
            [Frozen] IOrganisationsService organisationsService,
            [Frozen] IOdsService odsOrganisationsService,
            [Frozen] IOrderService orderService,
            ServiceRecipientsController controller)
        {
            List<ServiceRecipient> possibleRecipients =
            [
                CommonServiceRecipientFactory("AAAA", "XXXX"),
                CommonServiceRecipientFactory("AAAB", "XXXX"),
                CommonServiceRecipientFactory("AAAC", "XXXX"),
            ];

            EntityFramework.Ordering.Models.Order previousOrder = CommonOrderFactory(0, organisation.Id, 0, 1);

            OrderSublocation previousWorkingSublocation = CommonOrderSublocationFactory(
                "XXXX",
                [
                    CommonOrderSublocationRecipientFactory("AAAA", "XXXX", true),
                    CommonOrderSublocationRecipientFactory("AAAB", "XXXX", true),
                    CommonOrderSublocationRecipientFactory("AAAC", "XXXX", true),
                ],
                true);

            EntityFramework.Ordering.Models.Order order = CommonOrderFactory(0, 0, 0, 2);

            OrderSublocation workingSublocation = CommonOrderSublocationFactory(
                "XXXX",
                [],
                true);

            previousOrder.OrderingPartyId = organisation.Id;
            previousOrder.OrderingParty = organisation;
            previousOrder.OrderSublocations = [previousWorkingSublocation];
            previousWorkingSublocation.Order = previousOrder;

            order.OrderingPartyId = organisation.Id;
            order.OrderingParty = organisation;

            workingSublocation.Order = order;

            var orderWrapper = new OrderWrapper(order, [previousOrder]);

            organisationsService.GetOrganisationExternalIdentifierByInternalIdentifier(organisation.InternalIdentifier)
                .Returns(organisation.ExternalIdentifier);

            orderSublocationService.GetOrderSublocationWithRecipients(
                    organisation.ExternalIdentifier,
                    order.Id,
                    workingSublocation.SublocationOdsCode)
                .Returns(workingSublocation);

            odsOrganisationsService.GetServiceRecipientsBySublocation(workingSublocation.SublocationOdsCode)
                .Returns(possibleRecipients);

            orderService
                .GetOrderWithSublocationsAndSublocationRecipients(order.CallOffId, organisation.InternalIdentifier)
                .Returns(orderWrapper);

            orderService.GetOrderId(order.CallOffId).Returns(order.Id);

            var expectedModel = new NoNewRecipientsForSublocationAmendmentModel
            {
                PreviousOrderRecipients = previousWorkingSublocation.SublocationRecipients
                    .Select(x => $"{x.RecipientOdsOrganisation.Name} ({x.RecipientOdsCode})")
                    .ToList(),
                PreviousNounPhrase = "the previous revision",
            };

            var result =
                (await controller.SelectSublocationRecipients(
                    organisation.InternalIdentifier,
                    order.CallOffId,
                    workingSublocation.SublocationOdsCode))
                .As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should()
                .BeEquivalentTo(
                    expectedModel,
                    opt => opt.Excluding(m => m.Title)
                        .Excluding(m => m.Caption)
                        .Excluding(m => m.Advice)
                        .Excluding(m => m.BackLink)
                        .Excluding(m => m.SaveAndContinueLink)
                        .Excluding(m => m.PreviousOrderRecipients));

            IReadOnlyList<string> previousRecipientsForFurtherEvaluation =
                result.Model.As<NoNewRecipientsForSublocationAmendmentModel>().PreviousOrderRecipients;

            previousRecipientsForFurtherEvaluation.Should()
                .BeEquivalentTo(expectedModel.PreviousOrderRecipients);
        }

        [Theory]
        [MockAutoData]
        public static async Task SelectSublocationRecipients_Post_ReturnsBadRequest(
            SelectSublocationRecipientsModel selectSublocationRecipientsModel,
            string internalOrgId,
            CallOffId orderId,
            string sublocationOdsCode,
            [Frozen] IOrganisationsService organisationsService,
            ServiceRecipientsController controller)
        {
            organisationsService.GetOrganisationExternalIdentifierByInternalIdentifier(internalOrgId).ReturnsNull();

            var result =
                (await controller.SelectSublocationRecipients(
                    selectSublocationRecipientsModel,
                    internalOrgId,
                    orderId,
                    sublocationOdsCode)).As<BadRequestResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task SelectSublocationRecipients_Post_ReturnsViewOnError(
            SelectSublocationRecipientsModel selectSublocationRecipientsModel,
            string internalOrgId,
            CallOffId orderId,
            string externalOrgId,
            string sublocationOdsCode,
            [Frozen] IOrganisationsService organisationsService,
            ServiceRecipientsController controller)
        {
            controller.ModelState.AddModelError("SomeError", "Error message");

            organisationsService.GetOrganisationExternalIdentifierByInternalIdentifier(internalOrgId)
                .Returns(externalOrgId);

            IActionResult result = await controller.SelectSublocationRecipients(
                selectSublocationRecipientsModel,
                internalOrgId,
                orderId,
                sublocationOdsCode);

            await organisationsService.DidNotReceiveWithAnyArgs()
                    .GetOrganisationExternalIdentifierByInternalIdentifier(null)
                ;

            ViewResult viewResult = result.Should().BeOfType<ViewResult>().Subject;
            SelectSublocationRecipientsModel returnedModel =
                viewResult.Model.Should().BeOfType<SelectSublocationRecipientsModel>().Subject;

            returnedModel.Should().BeEquivalentTo(selectSublocationRecipientsModel);
        }

        public static IEnumerable<object[]> SublocationExpectedSets()
        {
            return
            [
                // no existing sublocation recipients + 2 new selected = 2 adds
                [
                    CommonOrganisationFactory(), CommonOrderFactory(), "XXXX",
                    CommonOrderSublocationFactory("XXXX"),
                    new List<SelectOption<string>>
                    {
                        new(string.Empty, "AAAA", true),
                        new(string.Empty, "AAAB", true),
                        new(string.Empty, "AAAC", false),
                    },
                    new HashSet<string> { "AAAA", "AAAB" },
                ],

                // 1 existing sublocation recipient + 1 new selected = 2 set
                [
                    CommonOrganisationFactory(), CommonOrderFactory(), "XXXX",
                    CommonOrderSublocationFactory(
                        "XXXX",
                        [CommonOrderSublocationRecipientFactory("AAAA", "XXXX")]),
                    new List<SelectOption<string>>
                    {
                        new(string.Empty, "AAAA", true),
                        new(string.Empty, "AAAB", true),
                        new(string.Empty, "AAAC", false),
                    },
                    new HashSet<string> { "AAAA", "AAAB" },
                ],
            ];
        }

        [Theory]
        [MockMemberAutoData(nameof(SublocationExpectedSets))]
        public static async Task SelectSublocationRecipients_Post_PerformsServiceCallsAndRedirects(
            Organisation organisation,
            EntityFramework.Ordering.Models.Order order,
            string sublocationOdsCode,
            OrderSublocation existingOrderSublocation,
            List<SelectOption<string>> newRenderedServiceRecipients,
            HashSet<string> expectedSets,
            [Frozen] IOrganisationsService organisationsService,
            [Frozen] IOrderSublocationService orderSublocationService,
            [Frozen] IOrderService orderService,
            ServiceRecipientsController controller)
        {
            existingOrderSublocation.SublocationOdsCode = sublocationOdsCode;
            existingOrderSublocation.OrderId = order.Id;
            existingOrderSublocation.OwnerOdsCode = organisation.ExternalIdentifier;

            var selectSublocationRecipientsModel = new SelectSublocationRecipientsModel
            {
                RenderedServiceRecipients = newRenderedServiceRecipients,
            };

            organisationsService.GetOrganisationExternalIdentifierByInternalIdentifier(organisation.InternalIdentifier)
                .Returns(organisation.ExternalIdentifier);

            orderSublocationService
                .GetOrderSublocationWithRecipients(
                    organisation.ExternalIdentifier,
                    order.Id,
                    sublocationOdsCode)
                .Returns(existingOrderSublocation);

            orderService.GetOrderId(order.CallOffId).Returns(order.Id);

            var result =
                (await controller.SelectSublocationRecipients(
                    selectSublocationRecipientsModel,
                    organisation.InternalIdentifier,
                    order.CallOffId,
                    sublocationOdsCode))
                .As<RedirectToActionResult>();

            await orderSublocationService.Received()
                .SetSublocationRecipients(
                    organisation.ExternalIdentifier,
                    order.Id,
                    sublocationOdsCode,
                    Arg.Is<HashSet<string>>(hs => AreStringHashSetsEquivalent(hs, expectedSets)));

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.ConfirmSublocations));
            result.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary
                    {
                        { "internalOrgId", organisation.InternalIdentifier }, { "callOffId", order.CallOffId },
                    });
        }

        [Theory]
        [MockAutoData]
        public static async Task ConfirmSublocations_ReturnsSublocationsView(
            Organisation organisation,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService ordersService,
            [Frozen] IOrderSublocationService orderSublocationService,
            ServiceRecipientsController controller)
        {
            order.OrderingPartyId = organisation.Id;
            order.OrderingParty = organisation;
            ICollection<OrderSublocation> orderSublocations = order.OrderSublocations;

            ordersService.GetOrderWithSublocations(order.CallOffId, organisation.InternalIdentifier)
                .Returns(new OrderWrapper(order));
            orderSublocationService.GetCountForOrderSublocationRecipients(
                    organisation.ExternalIdentifier,
                    order.Id,
                    Arg.Any<string>())
                .Returns(call => orderSublocations.First(x => x.SublocationOdsCode == call.ArgAt<string>(2))
                    .SublocationRecipients.Count);

            var expectedModel = new SelectSublocationsOverviewModel
            {
                Title = "Confirm sublocations",
                Caption = order.CallOffId.ToString(),
                Advice = "Select a sublocation to amend the organisations in this order",
                ProcessType = "order",
                Sublocations = orderSublocations.Select(x => new SublocationModel
                    {
                        Name = x.SublocationOrganisation.Name,
                        ServiceRecipientCount = x.SublocationRecipients.Count,
                        OdsCode = x.SublocationOdsCode,
                    })
                    .ToList(),
                ParentName = organisation.Name,
            };

            var result =
                (await controller.ConfirmSublocations(organisation.InternalIdentifier, order.CallOffId))
                .As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should()
                .BeEquivalentTo(
                    expectedModel,
                    opt => opt.Excluding(model => model.BackLink)
                        .Excluding(model => model.AddOrChangeSublocationsLink)
                        .Excluding(model => model.Sublocations));

            IReadOnlyList<SublocationModel> sublocations =
                result.Model.As<SelectSublocationsOverviewModel>().Sublocations;

            sublocations.Should()
                .BeEquivalentTo(
                    expectedModel.Sublocations,
                    opt => opt.Excluding(slModel => slModel.RecipientLink).Excluding(slModel => slModel.TaskProgress));
        }

        [Theory]
        [MockAutoData]
        public static async Task ConfirmSublocations_Amendment_ReturnsSublocationsView_WithAmendedStatus(
            Organisation organisation,
            EntityFramework.Ordering.Models.Order previousOrder,
            EntityFramework.Ordering.Models.Order order,
            List<OrderSublocationRecipient> newSublocationRecipients,
            [Frozen] IOrderService ordersService,
            [Frozen] IOrderSublocationService orderSublocationService,
            ServiceRecipientsController controller)
        {
            previousOrder.OrderingPartyId = order.OrderingPartyId;
            previousOrder.OrderingParty = order.OrderingParty;
            previousOrder.Revision = 1;

            order.OrderingPartyId = organisation.Id;
            order.OrderingParty = organisation;
            order.OrderSublocations = previousOrder.OrderSublocations.Select(x => x.Clone()).ToList();
            order.Revision = 2;

            order.OrderSublocations.ForEach(x =>
                x.SublocationOrganisation = CommonEntityOdsOrganisationFactory(x.SublocationOdsCode));

            OrderSublocation sublocationForAmendment = order.OrderSublocations.First();

            sublocationForAmendment.SublocationRecipients.AddRange(newSublocationRecipients);

            var orderWrapper = new OrderWrapper(order, [previousOrder]);

            ordersService.GetOrderWithSublocations(order.CallOffId, organisation.InternalIdentifier)
                .Returns(orderWrapper);

            orderSublocationService.GetCountForOrderSublocationRecipients(
                    organisation.ExternalIdentifier,
                    order.Id,
                    Arg.Any<string>())
                .Returns(call => order.OrderSublocations.First(x => x.SublocationOdsCode == call.ArgAt<string>(2))
                    .SublocationRecipients.Count);

            orderSublocationService.GetCountForOrderSublocationRecipients(
                    organisation.ExternalIdentifier,
                    previousOrder.Id,
                    Arg.Any<string>())
                .Returns(call => previousOrder.OrderSublocations
                    .First(x => x.SublocationOdsCode == call.ArgAt<string>(2))
                    .SublocationRecipients.Count);

            var expectedModel = new SelectSublocationsOverviewModel
            {
                Title = "Confirm sublocations",
                Caption = order.CallOffId.ToString(),
                Advice = "Select a sublocation to amend the organisations in this order",
                ProcessType = "order",
                Sublocations = order.OrderSublocations.Select(x => new SublocationModel
                    {
                        Name = x.SublocationOrganisation.Name,
                        ServiceRecipientCount = x.SublocationRecipients.Count,
                        OdsCode = x.SublocationOdsCode,
                        TaskProgress = x.SublocationOdsCode == sublocationForAmendment.SublocationOdsCode
                            ? TaskProgress.Amended
                            : TaskProgress.Completed,
                    })
                    .ToList(),
                ParentName = organisation.Name,
            };

            var result =
                (await controller.ConfirmSublocations(organisation.InternalIdentifier, order.CallOffId))
                .As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should()
                .BeEquivalentTo(
                    expectedModel,
                    opt => opt.Excluding(model => model.BackLink)
                        .Excluding(model => model.AddOrChangeSublocationsLink)
                        .Excluding(model => model.Sublocations));

            IReadOnlyList<SublocationModel> sublocations =
                result.Model.As<SelectSublocationsOverviewModel>().Sublocations;

            sublocations.Should()
                .BeEquivalentTo(
                    expectedModel.Sublocations,
                    opt => opt.Excluding(slModel => slModel.RecipientLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task ConfirmSublocations_Post_ConditionalRedirect_RedirectToTasklistIfIncomplete(
            string internalOrganisationId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderService,
            ServiceRecipientsController controller)
        {
            order.OrderType = OrderTypeEnum.Solution;

            var callingModel =
                new SelectSublocationsOverviewModel
                {
                    Sublocations = [new SublocationModel { ServiceRecipientCount = 0 }],
                };

            orderService.GetOrderThin(order.CallOffId, internalOrganisationId).Returns(new OrderWrapper(order));

            var result =
                (await controller.ConfirmSublocations(callingModel, internalOrganisationId, order.CallOffId))
                .As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrderController.Order));
            result.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            result.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrganisationId }, { "callOffId", order.CallOffId } });
        }

        [Theory]
        [MockAutoData]
        public static async Task ConfirmSublocations_Post_ConditionalRedirect_RedirectToConfirmScreenIfComplete(
            string internalOrganisationId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderService,
            ServiceRecipientsController controller)
        {
            order.OrderType = OrderTypeEnum.Solution;

            var callingModel =
                new SelectSublocationsOverviewModel
                {
                    Sublocations = [new SublocationModel { ServiceRecipientCount = 1 }],
                };

            orderService.GetOrderThin(order.CallOffId, internalOrganisationId).Returns(new OrderWrapper(order));

            var result =
                (await controller.ConfirmSublocations(callingModel, internalOrganisationId, order.CallOffId))
                .As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.ConfirmSublocationRecipients));
            result.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrganisationId }, { "callOffId", order.CallOffId } });
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static async Task ConfirmSublocations_Post_MergerOrSplit_RedirectToTaskListIfIncomplete(
            OrderTypeEnum orderType,
            string internalOrganisationId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderService,
            ServiceRecipientsController controller)
        {
            order.OrderType = orderType;

            var callingModel =
                new SelectSublocationsOverviewModel
                {
                    Sublocations = [new SublocationModel { ServiceRecipientCount = 1 }],
                };

            orderService.GetOrderThin(order.CallOffId, internalOrganisationId).Returns(new OrderWrapper(order));

            var result =
                (await controller.ConfirmSublocations(callingModel, internalOrganisationId, order.CallOffId))
                .As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrderController.Order));
            result.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            result.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrganisationId }, { "callOffId", order.CallOffId } });
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        public static async Task ConfirmSublocations_Post_MergerOrSplit_RedirectToPracticeReorganisationSelection(
            OrderTypeEnum orderType,
            string internalOrganisationId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderService,
            ServiceRecipientsController controller)
        {
            order.OrderType = orderType;

            var callingModel =
                new SelectSublocationsOverviewModel
                {
                    Sublocations = [new SublocationModel { ServiceRecipientCount = 2 }],
                };

            orderService.GetOrderThin(order.CallOffId, internalOrganisationId).Returns(new OrderWrapper(order));

            var result =
                (await controller.ConfirmSublocations(callingModel, internalOrganisationId, order.CallOffId))
                .As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.SelectRecipientForPracticeReorganisation));
            result.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrganisationId }, { "callOffId", order.CallOffId } });
        }

        [Theory]
        [MockAutoData]
        public static async Task ConfirmSublocationsRecipients_ReturnsView(
            Organisation organisation,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService ordersService,
            ServiceRecipientsController controller)
        {
            var expectedModel = new ConfirmSublocationRecipientsModel(order, "testUrl", "testUrl");

            ordersService
                .GetOrderWithSublocationsAndSublocationRecipients(
                    order.CallOffId,
                    organisation.InternalIdentifier)
                .Returns(new OrderWrapper(order));

            var result = (await controller.ConfirmSublocationRecipients(
                organisation.InternalIdentifier,
                order.CallOffId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should()
                .BeEquivalentTo(
                    expectedModel,
                    opt => opt.Excluding(m => m.BackLink)
                        .Excluding(m => m.Title)
                        .Excluding(m => m.Caption)
                        .Excluding(m => m.Advice));
        }

        [Theory]
        [MockAutoData]
        public static async Task ConfirmSublocationsRecipients_ReturnsView_NoNewRecipients(
            Organisation organisation,
            EntityFramework.Ordering.Models.Order previousOrder,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService ordersService,
            ServiceRecipientsController controller)
        {
            previousOrder.Revision = 1;
            order.Revision = 2;

            order.OrderSublocations = previousOrder.OrderSublocations;

            var orderWrapper = new OrderWrapper(order, [previousOrder]);

            var expectedModel = new NoNewRecipientsForAmendmentModel(orderWrapper.Order, "testUrl", "testUrl");

            ordersService
                .GetOrderWithSublocationsAndSublocationRecipients(
                    order.CallOffId,
                    organisation.InternalIdentifier)
                .Returns(orderWrapper);

            var result = (await controller.ConfirmSublocationRecipients(
                organisation.InternalIdentifier,
                order.CallOffId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should()
                .BeEquivalentTo(expectedModel);
            result.ViewName.Should().Be("ServiceRecipients/NoNewRecipientsForAmendment");
        }

        [Theory]
        [MockAutoData]
        public static async Task ConfirmSublocationsRecipients_ReturnsView_WithAmendmentDetails(
            Organisation organisation,
            EntityFramework.Ordering.Models.Order previousOrder,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService ordersService,
            ServiceRecipientsController controller)
        {
            previousOrder.Revision = 1;
            order.Revision = 2;

            var orderWrapper = new OrderWrapper(order, [previousOrder]);

            var expectedModel = new ConfirmSublocationRecipientsModel(orderWrapper, "testUrl", "testUrl");

            ordersService
                .GetOrderWithSublocationsAndSublocationRecipients(
                    order.CallOffId,
                    organisation.InternalIdentifier)
                .Returns(orderWrapper);

            var result = (await controller.ConfirmSublocationRecipients(
                organisation.InternalIdentifier,
                order.CallOffId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should()
                .BeEquivalentTo(
                    expectedModel,
                    opt => opt.Excluding(m => m.BackLink)
                        .Excluding(m => m.Title)
                        .Excluding(m => m.Caption)
                        .Excluding(m => m.Advice));
            result.ViewName.Should().Be("ServiceRecipients/ConfirmSublocationRecipients");
        }

        private static Organisation CommonOrganisationFactory(int customId = 0)
        {
            return new Organisation
            {
                Id = customId == 0 ? CommonOrganisationId : customId,
                InternalIdentifier = CommonOrganisationInternalIdentifier,
                ExternalIdentifier = CommonOrganisationExternalIdentifier,
                Name = "A Local ICB",
            };
        }

        private static EntityFramework.Ordering.Models.Order CommonOrderFactory(
            int customId = 0,
            int customOrganisationId = 0,
            int customOrderNumber = 0,
            int customRevision = 0,
            ICollection<OrderSublocation> orderSublocations = null)
        {
            var random = new Random();
            return new EntityFramework.Ordering.Models.Order
            {
                Id = customId == 0 ? random.Next() : customId,
                OrderNumber = customOrderNumber == 0 ? CommonOrderNumber : customOrderNumber,
                Revision = customRevision,
                Description = $"An order {customId}",
                OrderingPartyId = customOrganisationId == 0 ? CommonOrganisationId : customOrganisationId,
                SelectedFramework = new EntityFramework.Catalogue.Models.Framework { Id = random.Next().ToString() },
                OrderSublocations = orderSublocations,
            };
        }

        private static OrderSublocation CommonOrderSublocationFactory(
            string sublocationOdsCode,
            List<OrderSublocationRecipient> sublocationRecipients = null,
            bool hasOrganisation = false)
        {
            return new OrderSublocation
            {
                OrderId = CommonOrderId,
                SublocationOdsCode = sublocationOdsCode,
                OwnerOdsCode = CommonOrganisationExternalIdentifier,
                SublocationRecipients = sublocationRecipients,
                SublocationOrganisation =
                    hasOrganisation ? CommonEntityOdsOrganisationFactory(sublocationOdsCode) : null,
            };
        }

        private static OrderSublocationRecipient CommonOrderSublocationRecipientFactory(
            string recipientOdsCode,
            string parentSublocationOdsCode,
            bool hasOrganisation = false)
        {
            return new OrderSublocationRecipient
            {
                OrderId = CommonOrderId,
                RecipientOdsCode = recipientOdsCode,
                ParentSublocationOdsCode = parentSublocationOdsCode,
                RecipientOdsOrganisation =
                    hasOrganisation ? CommonEntityOdsOrganisationFactory(recipientOdsCode) : null,
            };
        }

        private static ServiceRecipient CommonServiceRecipientFactory(string orgId, string locationOrgId)
        {
            return new ServiceRecipient { OrgId = orgId, LocationOrgId = locationOrgId };
        }

        private static EntityOdsOrganisation CommonEntityOdsOrganisationFactory(string id)
        {
            return new EntityOdsOrganisation { Id = id, Name = $"An organisation - {id}", IsActive = true };
        }

        private static bool AreStringHashSetsEquivalent(
            HashSet<string> actual,
            HashSet<string> expected)
        {
            try
            {
                actual.Should().BeEquivalentTo(expected);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
