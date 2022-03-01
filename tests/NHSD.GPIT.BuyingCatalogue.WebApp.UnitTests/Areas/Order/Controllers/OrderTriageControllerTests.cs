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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSource;
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
        public static void Get_Index_ReturnsModel(
            string internalOrgId,
            OrderTriageController controller)
        {
            var result = controller.Index(internalOrgId);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().NotBeNull();
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
        public static void Post_Index_NotSureTriageOption_RedirectsToView(
            string internalOrgId,
            OrderTriageModel model,
            OrderTriageController controller)
        {
            model.SelectedTriageOption = TriageOption.NotSure;

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
            model.SelectedTriageOption = TriageOption.Under40k;

            var result = controller.Index(internalOrgId, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.TriageSelection));
        }

        [Theory]
        [CommonAutoData]
        public static void Get_NotReady_ReturnsView(
            string internalOrgId,
            OrderTriageController controller)
        {
            var result = controller.NotSure(internalOrgId);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeOfType(typeof(GenericOrderTriageModel));
        }

        [Theory]
        [CommonAutoData]
        public static void Get_TriageSelection_InvalidOption_Redirects(
            string internalOrgId,
            OrderTriageController controller)
        {
            var result = controller.TriageSelection(internalOrgId, null);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.Index));
        }

        [Theory]
        [CommonAutoData]
        public static void Get_TriageSelection_ReturnsView(
            string internalOrgId,
            TriageOption option,
            OrderTriageController controller)
        {
            var result = controller.TriageSelection(internalOrgId, option);

            result.As<ViewResult>().Should().NotBeNull();
        }

        [Theory]
        [CommonInlineAutoData(TriageOption.Under40k, "Select yes if you’ve identified what you want to order")]
        [CommonInlineAutoData(TriageOption.Between40kTo250k, "Select yes if you’ve carried out a competition on the Buying Catalogue")]
        [CommonInlineAutoData(TriageOption.Over250k, "Select yes if you’ve carried out an Off-Catalogue Competition with suppliers")]
        public static void Post_TriageSelection_NoSelection_AddsModelError(
            TriageOption option,
            string expectedErrorMessage,
            string internalOrgId,
            TriageDueDiligenceModel model,
            OrderTriageController controller)
        {
            model.Selected = null;

            _ = controller.TriageSelection(internalOrgId, option, model);

            var modelStateErrors = controller.ModelState.Values.SelectMany(mse => mse.Errors.Select(e => e.ErrorMessage));

            modelStateErrors.Should().Contain(expectedErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_TriageSelection_InvalidModelState_ReturnsView(
            string internalOrgId,
            TriageOption option,
            TriageDueDiligenceModel model,
            OrderTriageController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = controller.TriageSelection(internalOrgId, option, model);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_TriageSelection_NotSelected_RedirectsToStepsNotCompleted(
            string internalOrgId,
            TriageOption option,
            TriageDueDiligenceModel model,
            OrderTriageController controller)
        {
            model.Selected = false;

            var result = controller.TriageSelection(internalOrgId, option, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(controller.StepsNotCompleted));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_TriageSelection_Selected_Redirects(
            string internalOrgId,
            TriageOption option,
            TriageDueDiligenceModel model,
            OrderTriageController controller)
        {
            model.Selected = true;

            var result = controller.TriageSelection(internalOrgId, option, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(OrderTriageController.TriageFunding));
        }

        [Theory]
        [CommonInlineAutoData(TriageOption.Under40k, "Incomplete40k")]
        [CommonInlineAutoData(TriageOption.Between40kTo250k, "Incomplete40kTo250k")]
        [CommonInlineAutoData(TriageOption.Over250k, "IncompleteOver250k")]
        public static void Get_StepsNotCompleted_ReturnsView(
            TriageOption option,
            string expectedViewName,
            string internalOrgId,
            OrderTriageController controller)
        {
            var result = controller.StepsNotCompleted(internalOrgId, option);

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
            TriageOption? option,
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

            result.RouteValues["option"].As<TriageOption?>().Should().BeNull();
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

        [Theory]
        [CommonAutoData]
        public static void Get_TriageFunding_ReturnsViewWithModel(
            string internalOrgId,
            TriageOption option,
            OrderTriageController controller)
        {
            var model = new FundingSourceModel();

            var result = controller.TriageFunding(internalOrgId, option).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Get_TriageFunding_PreselectsFundingSource(
            string internalOrgId,
            TriageOption option,
            FundingSource fundingSource,
            OrderTriageController controller)
        {
            var result = controller.TriageFunding(internalOrgId, option, fundingSource).As<ViewResult>();

            result.Model.As<FundingSourceModel>().SelectedFundingSource.Should().Be(fundingSource);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_TriageFunding_InvalidModel_ReturnsView(
            string internalOrgId,
            TriageOption option,
            FundingSourceModel model,
            OrderTriageController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = controller.TriageFunding(internalOrgId, option, model).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_TriageFunding_ValidModel_Redirects(
            string internalOrgId,
            TriageOption option,
            FundingSourceModel model,
            OrderTriageController controller)
        {
            var result = controller.TriageFunding(internalOrgId, option, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrderController.ReadyToStart));
            result.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { nameof(internalOrgId), internalOrgId },
                { nameof(option), option },
                { "fundingSource", model.SelectedFundingSource!.Value },
            });
        }
    }
}
