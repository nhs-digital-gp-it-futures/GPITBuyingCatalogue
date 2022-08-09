﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class OrderTriageControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderTriageController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_OrderItemType_ReturnsView(
            Organisation organisation,
            [Frozen] Mock<IOrganisationsService> service,
            OrderTriageController controller)
        {
            var expectedModel = new OrderItemTypeModel(organisation.Name);

            service.Setup(s => s.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
                .ReturnsAsync(organisation);

            var result = (await controller.OrderItemType(organisation.InternalIdentifier)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_OrderItemType_InvalidModel_ReturnsView(
            string internalOrgId,
            OrderItemTypeModel model,
            OrderTriageController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = controller.OrderItemType(internalOrgId, model).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_OrderItemType_Redirects(
            string internalOrgId,
            OrderItemTypeModel model,
            OrderTriageController controller)
        {
            var result = controller.OrderItemType(internalOrgId, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrderTriageController.SelectOrganisation));
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "orderType", model.SelectedOrderItemType!.Value },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ReturnsModel(
            Organisation organisation,
            [Frozen] Mock<IOrganisationsService> service,
            OrderTriageController controller)
        {
            service.Setup(s => s.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
                .ReturnsAsync(organisation);

            var result = await controller.Index(organisation.InternalIdentifier);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_WithAssociatedServiceOrderType_Redirects(
            string internalOrgId,
            OrderTriageController controller)
        {
            var result = (await controller.Index(
                internalOrgId,
                orderType: CatalogueItemType.AssociatedService)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrderController.ReadyToStart));
            result.ControllerName.Should().Be(typeof(OrderController).ControllerName());
        }

        [Theory]
        [CommonAutoData]
        public static void Post_Index_InvalidModelState_ReturnsView(
            string internalOrgId,
            OrderTriageModel model,
            OrderTriageController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = controller.Index(internalOrgId, model);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_Index_NotSureOrderTriageValue_RedirectsToView(
            string internalOrgId,
            OrderTriageModel model,
            OrderTriageController controller)
        {
            model.SelectedOrderTriageValue = OrderTriageValue.NotSure;

            var result = controller.Index(internalOrgId, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.NotSure));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_Index_Valid_RedirectsToTriageSelection(
            string internalOrgId,
            OrderTriageModel model,
            OrderTriageController controller)
        {
            model.SelectedOrderTriageValue = OrderTriageValue.Under40K;

            var result = controller.Index(internalOrgId, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.TriageSelection));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_NotReady_ReturnsView(
            Organisation organisation,
            [Frozen] Mock<IOrganisationsService> service,
            OrderTriageController controller)
        {
            service.Setup(s => s.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
                .ReturnsAsync(organisation);

            var result = await controller.NotSure(organisation.InternalIdentifier);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeOfType(typeof(GenericOrderTriageModel));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_TriageSelection_InvalidOption_Redirects(
            string internalOrgId,
            OrderTriageController controller)
        {
            var result = await controller.TriageSelection(internalOrgId, null);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.Index));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_TriageSelection_ReturnsView(
            Organisation organisation,
            OrderTriageValue option,
            [Frozen] Mock<IOrganisationsService> service,
            OrderTriageController controller)
        {
            service.Setup(s => s.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
                .ReturnsAsync(organisation);

            var result = await controller.TriageSelection(organisation.InternalIdentifier, option);

            result.As<ViewResult>().Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Post_TriageSelection_InvalidModelState_ReturnsView(
            string internalOrgId,
            OrderTriageValue option,
            TriageDueDiligenceModel model,
            OrderTriageController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = controller.TriageSelection(internalOrgId, model, option);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_TriageSelection_NotSelected_RedirectsToStepsNotCompleted(
            string internalOrgId,
            OrderTriageValue option,
            TriageDueDiligenceModel model,
            OrderTriageController controller)
        {
            model.Selected = false;

            var result = controller.TriageSelection(internalOrgId, model, option);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.StepsNotCompleted));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_TriageSelection_Selected_Redirects(
            string internalOrgId,
            OrderTriageValue option,
            TriageDueDiligenceModel model,
            OrderTriageController controller)
        {
            model.Selected = true;

            var result = controller.TriageSelection(internalOrgId, model, option);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(OrderController.ReadyToStart));
        }

        [Theory]
        [CommonInlineAutoData(OrderTriageValue.Under40K, "Incomplete40k")]
        [CommonInlineAutoData(OrderTriageValue.Between40KTo250K, "Incomplete40kTo250k")]
        [CommonInlineAutoData(OrderTriageValue.Over250K, "IncompleteOver250k")]
        public static async Task Get_StepsNotCompleted_ReturnsView(
            OrderTriageValue option,
            string expectedViewName,
            Organisation organisation,
            [Frozen] Mock<IOrganisationsService> service,
            OrderTriageController controller)
        {
            service.Setup(s => s.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
                .ReturnsAsync(organisation);

            var result = await controller.StepsNotCompleted(organisation.InternalIdentifier, option);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be(expectedViewName);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectOrganisation_ReturnsView(
            [Frozen] Mock<IOrganisationsService> organisationService,
            List<Organisation> organisations,
            OrderTriageController controller)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[]
                {
                    new(CatalogueClaims.OrganisationFunction, "Buyer"),
                    new(CatalogueClaims.PrimaryOrganisationInternalIdentifier, organisations.First().InternalIdentifier),
                    new(CatalogueClaims.SecondaryOrganisationInternalIdentifier, organisations.Last().InternalIdentifier),
                },
                "mock"));

            controller.ControllerContext =
                new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user },
                };

            organisationService.Setup(s => s.GetOrganisationsByInternalIdentifiers(It.IsAny<string[]>())).ReturnsAsync(organisations);

            var expected = new SelectOrganisationModel(organisations.First().InternalIdentifier, organisations)
            {
                Title = "Which organisation are you ordering for?",
            };

            var result = (await controller.SelectOrganisation(organisations.First().InternalIdentifier)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectOrganisation_NoSecondaryOds_RedirectsToIndex(
            Organisation organisation,
            OrderTriageController controller)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[]
                {
                    new(CatalogueClaims.OrganisationFunction, "Buyer"),
                    new(CatalogueClaims.PrimaryOrganisationInternalIdentifier, organisation.InternalIdentifier),
                },
                "mock"));

            controller.ControllerContext =
                new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user },
                };

            var result = (await controller.SelectOrganisation(organisation.InternalIdentifier)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrderTriageController.Index));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SelectOrganisation_NoSecondaryOds_RedirectsToIndex(
            Organisation organisation,
            SelectOrganisationModel model,
            OrderTriageController controller)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[]
                {
                    new(CatalogueClaims.OrganisationFunction, "Buyer"),
                    new(CatalogueClaims.PrimaryOrganisationInternalIdentifier, organisation.InternalIdentifier),
                },
                "mock"));

            controller.ControllerContext =
                new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user },
                };

            var result = controller.SelectOrganisation(organisation.InternalIdentifier, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SelectOrganisation_InvalidModel_ReturnsView(
            string internalOrgId,
            List<Organisation> organisations,
            SelectOrganisationModel model,
            OrderTriageController controller)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[]
                {
                    new(CatalogueClaims.OrganisationFunction, "Buyer"),
                    new(CatalogueClaims.PrimaryOrganisationInternalIdentifier, organisations.First().InternalIdentifier),
                    new(CatalogueClaims.SecondaryOrganisationInternalIdentifier, organisations.Last().InternalIdentifier),
                },
                "mock"));

            controller.ControllerContext =
                new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user },
                };

            controller.ModelState.AddModelError("some-key", "some-error");

            var result = controller.SelectOrganisation(internalOrgId, model).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SelectOrganisation_SelectedDifferent_ResetsOption(
            OrderTriageValue? option,
            string internalOrgId,
            List<Organisation> organisations,
            SelectOrganisationModel model,
            OrderTriageController controller)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[]
                {
                    new(CatalogueClaims.OrganisationFunction, "Buyer"),
                    new(CatalogueClaims.PrimaryOrganisationInternalIdentifier, organisations.First().InternalIdentifier),
                    new(CatalogueClaims.SecondaryOrganisationInternalIdentifier, organisations.Last().InternalIdentifier),
                },
                "mock"));

            controller.ControllerContext =
                new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user },
                };

            var result = controller.SelectOrganisation(internalOrgId, model, option).As<RedirectToActionResult>();

            result.RouteValues["option"].As<OrderTriageValue?>().Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SelectOrganisation_RedirectsToNewOrder(
            string internalOrgId,
            SelectOrganisationModel model,
            OrderTriageController controller)
        {
            var result = controller.SelectOrganisation(internalOrgId, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrderTriageController.Index));
        }
    }
}
