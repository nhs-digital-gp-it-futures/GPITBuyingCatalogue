using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderTriage;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Shared;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class OrderTriageControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderTriageController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_OrderItemType_ReturnsView(
            Organisation organisation,
            [Frozen] IOrganisationsService service,
            OrderTriageController controller)
        {
            var expectedModel = new OrderItemTypeModel(organisation.Name);

            service
                .GetOrganisationByInternalIdentifier(organisation.InternalIdentifier)
                .Returns(organisation);

            var result = (await controller.OrderItemType(organisation.InternalIdentifier)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static void Post_OrderItemType_Solution_Redirects(
            string internalOrgId,
            OrderItemTypeModel model,
            OrderTriageController controller)
        {
            model.SelectedOrderItemType = CatalogueItemType.Solution;
            var result = controller.OrderItemType(internalOrgId, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrderTriageController.SelectFramework));
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "orderType", OrderTypeEnum.Solution },
            });
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Get_DetermineAssociatedServiceType_ReturnsView(
            Organisation organisation,
            bool mergerEnabled,
            bool splitEnabled,
            [Frozen] IOrganisationsService service,
            [Frozen] ISupplierService supplierService,
            OrderTriageController controller)
        {
            var expectedModel = new DetermineAssociatedServiceTypeModel(organisation.Name, mergerEnabled, splitEnabled)
            {
                InternalOrgId = organisation.InternalIdentifier,
            };

            service
                .GetOrganisationByInternalIdentifier(organisation.InternalIdentifier)
                .Returns(organisation);

            supplierService
                .HasActiveSuppliers(OrderTypeEnum.AssociatedServiceMerger)
                .Returns(mergerEnabled);

            supplierService.HasActiveSuppliers(OrderTypeEnum.AssociatedServiceSplit)
                .Returns(splitEnabled);

            var result = (await controller.DetermineAssociatedServiceType(organisation.InternalIdentifier)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
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
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        public static void Post_DetermineAssociatedServiceType_Solution_Redirects(
            OrderTypeEnum orderType,
            string internalOrgId,
            DetermineAssociatedServiceTypeModel model,
            OrderTriageController controller)
        {
            model.OrderType = orderType;
            var result = controller.DetermineAssociatedServiceType(internalOrgId, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrderTriageController.SelectFramework));
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "orderType", orderType },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SelectFramework_ReturnsView(
            Organisation organisation,
            [Frozen] IOrganisationsService service,
            [Frozen] IFrameworkService frameworkService,
            EntityFramework.Catalogue.Models.Framework framework,
            OrderTriageController controller)
        {
            framework.IsExpired = false;
            var frameworks = new List<EntityFramework.Catalogue.Models.Framework>()
            {
                framework,
            };

            var expectedModel = new SelectFrameworkModel(organisation.Name, frameworks, null);

            service
                .GetOrganisationByInternalIdentifier(organisation.InternalIdentifier)
                .Returns(organisation);

            frameworkService
                .GetFrameworks()
                .Returns(frameworks);

            var result = (await controller.SelectFramework(organisation.InternalIdentifier, OrderTypeEnum.Solution)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static void Post_SelectFramework_InvalidModel_ReturnsView(
            string internalOrgId,
            SelectFrameworkModel model,
            OrderTriageController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = controller.SelectFramework(internalOrgId, model).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static void Post_SelectFramework_ReturnsView(
            string internalOrgId,
            SelectFrameworkModel model,
            OrderTriageController controller)
        {
            var result = controller.SelectFramework(internalOrgId, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrderController.NewOrder));
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "orderType", model.OrderType },
                { "selectedFrameworkId", model.SelectedFrameworkId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SelectOrganisation_ReturnsView(
            [Frozen] IOrganisationsService organisationService,
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

            organisationService
                .GetOrganisationsByInternalIdentifiers(Arg.Any<string[]>())
                .Returns(organisations);

            var expected = new SelectOrganisationModel(organisations.First().InternalIdentifier, organisations)
            {
                Title = "Which organisation are you ordering for?",
            };

            var result = (await controller.SelectOrganisation(organisations.First().InternalIdentifier)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SelectOrganisation_NoSecondaryOds_RedirectsToOrderItemType(
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

            var result = (await controller.SelectOrganisation(organisation.InternalIdentifier)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrderTriageController.OrderItemType));
        }

        [Theory]
        [MockAutoData]
        public static void Post_SelectOrganisation_NoSecondaryOds_RedirectsToOrderItemType(
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
            result.ActionName.Should().Be(nameof(OrderTriageController.OrderItemType));
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static void Post_SelectOrganisation_RedirectsToOrderItemType(
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

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrderTriageController.OrderItemType));
        }
    }
}
