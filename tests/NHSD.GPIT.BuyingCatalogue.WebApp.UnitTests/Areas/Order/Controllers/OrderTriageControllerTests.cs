using System;
using System.Collections.Generic;
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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    // TODO: MJK NSubstitue
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
        public static void Post_OrderItemType_Solution_Redirects(
            string internalOrgId,
            OrderItemTypeModel model,
            OrderTriageController controller)
        {
            model.SelectedOrderItemType = CatalogueItemType.Solution;
            var result = controller.OrderItemType(internalOrgId, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrderTriageController.SelectOrganisation));
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "orderType", OrderTypeEnum.Solution },
            });
        }

        [Theory]
        [CommonAutoData]
        public static void Post_OrderItemType_AssociatedService_Redirects(
            string internalOrgId,
            OrderItemTypeModel model,
            OrderTriageController controller)
        {
            model.SelectedOrderItemType = CatalogueItemType.AssociatedService;
            var result = controller.OrderItemType(internalOrgId, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrderTriageController.DetermineAssociatedServiceType));
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static void Post_OrderItemType_Throws(
            string internalOrgId,
            OrderItemTypeModel model,
            OrderTriageController controller)
        {
            model.SelectedOrderItemType = CatalogueItemType.AdditionalService;
            FluentActions.Invoking(() => controller.OrderItemType(internalOrgId, model))
                .Should().Throw<InvalidOperationException>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DetermineAssociatedServiceType_ReturnsView(
            Organisation organisation,
            bool mergerEnabled,
            bool splitEnabled,
            [Frozen] Mock<IOrganisationsService> service,
            [Frozen] Mock<ISupplierService> supplierService,
            OrderTriageController controller)
        {
            var expectedModel = new DetermineAssociatedServiceTypeModel(organisation.Name, mergerEnabled, splitEnabled)
            {
                InternalOrgId = organisation.InternalIdentifier,
            };

            service.Setup(s => s.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
                .ReturnsAsync(organisation);

            supplierService.Setup(s => s.HasActiveSuppliers(OrderTypeEnum.AssociatedServiceMerger))
                .ReturnsAsync(mergerEnabled);

            supplierService.Setup(s => s.HasActiveSuppliers(OrderTypeEnum.AssociatedServiceSplit))
                .ReturnsAsync(splitEnabled);

            var result = (await controller.DetermineAssociatedServiceType(organisation.InternalIdentifier)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_DetermineAssociatedServiceType_InvalidModel_ReturnsView(
            string internalOrgId,
            DetermineAssociatedServiceTypeModel model,
            OrderTriageController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = controller.DetermineAssociatedServiceType(internalOrgId, model).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        public static void Post_DetermineAssociatedServiceType_Solution_Redirects(
            OrderTypeEnum orderType,
            string internalOrgId,
            DetermineAssociatedServiceTypeModel model,
            OrderTriageController controller)
        {
            model.OrderType = orderType;
            var result = controller.DetermineAssociatedServiceType(internalOrgId, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrderTriageController.SelectOrganisation));
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "orderType", orderType },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectOrganisation_ReturnsView(
            OrderTypeEnum orderType,
            [Frozen] Mock<IOrganisationsService> organisationService,
            List<Organisation> organisations,
            OrderTriageController controller)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[]
                {
                    new(ClaimTypes.Role, "Buyer"),
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

            var result = (await controller.SelectOrganisation(organisations.First().InternalIdentifier, orderType)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectOrganisation_NoSecondaryOds_RedirectsToIndex(
            OrderTypeEnum orderType,
            Organisation organisation,
            OrderTriageController controller)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[]
                {
                    new(ClaimTypes.Role, "Buyer"),
                    new(CatalogueClaims.PrimaryOrganisationInternalIdentifier, organisation.InternalIdentifier),
                },
                "mock"));

            controller.ControllerContext =
                new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user },
                };

            var result = (await controller.SelectOrganisation(organisation.InternalIdentifier, orderType)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrderController.ReadyToStart));
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
                    new(ClaimTypes.Role, "Buyer"),
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
            result.ActionName.Should().Be(nameof(OrderController.ReadyToStart));
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
                    new(ClaimTypes.Role, "Buyer"),
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
            string internalOrgId,
            List<Organisation> organisations,
            SelectOrganisationModel model,
            OrderTriageController controller)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[]
                {
                    new(ClaimTypes.Role, "Buyer"),
                    new(CatalogueClaims.PrimaryOrganisationInternalIdentifier, organisations.First().InternalIdentifier),
                    new(CatalogueClaims.SecondaryOrganisationInternalIdentifier, organisations.Last().InternalIdentifier),
                },
                "mock"));

            controller.ControllerContext =
                new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user },
                };

            var result = controller.SelectOrganisation(internalOrgId, model).As<RedirectToActionResult>();

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
            result.ActionName.Should().Be(nameof(OrderController.ReadyToStart));
        }
    }
}
