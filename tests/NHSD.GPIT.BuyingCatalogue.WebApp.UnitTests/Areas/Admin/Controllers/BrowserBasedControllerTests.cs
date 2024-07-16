using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using EnumsNET;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.BrowserBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class BrowserBasedControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(BrowserBasedController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task BrowserBased_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.BrowserBased(solutionId)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task BrowserBased_WithBrowserClientApplication_SetsCorrectBacklink(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IUrlHelper urlHelper,
            BrowserBasedController controller)
        {
            solution.ApplicationTypeDetail.ApplicationTypes.Add(
                ApplicationType.BrowserBased.AsString(EnumFormat.EnumMemberValue));

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            _ = await controller.BrowserBased(solution.CatalogueItemId);

            urlHelper.Received().Action(
                    Arg.Is<UrlActionContext>(y => y.Action == nameof(CatalogueSolutionsController.ApplicationType)));
        }

        [Theory]
        [MockAutoData]
        public static async Task BrowserBased_WithoutBrowserClientApplication_SetsCorrectBacklink(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IUrlHelper urlHelper,
            BrowserBasedController controller)
        {
            solution.ApplicationTypeDetail.ApplicationTypes = new HashSet<string>();

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            _ = await controller.BrowserBased(solution.CatalogueItemId);

            urlHelper.Received().Action(
                    Arg.Is<UrlActionContext>(y => y.Action == nameof(CatalogueSolutionsController.AddApplicationType)));
        }

        [Theory]
        [MockAutoData]
        public static async Task BrowserBased_Valid_ReturnsViewWithModel(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solution.ApplicationTypeDetail.ApplicationTypes = new HashSet<string>();

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var expectedModel = new BrowserBasedModel(solution.CatalogueItem);

            var result = (await controller.BrowserBased(solution.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task SupportedBrowsers_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.SupportedBrowsers(solutionId)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SupportedBrowsers_ValidSolutionId_ReturnsViewWithModel(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var expectedModel = new SupportedBrowsersModel(solution.CatalogueItem);

            var result = (await controller.SupportedBrowsers(solution.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SupportedBrowsers_InvalidModel_ReturnsViewWithModel(
            CatalogueItemId solutionId,
            SupportedBrowsersModel model,
            BrowserBasedController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-value");

            var result = (await controller.SupportedBrowsers(solutionId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().Be(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SupportedBrowsers_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            SupportedBrowsersModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.SupportedBrowsers(solutionId, model)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SupportedBrowsers_NullApplicationType_ReturnsBadRequestResult(
            Solution solution,
            SupportedBrowsersModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns((ApplicationTypeDetail)null);

            var result = (await controller.SupportedBrowsers(solution.CatalogueItemId, model)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SupportedBrowsers_NullBrowsers_SetsBrowsersSupported(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            SupportedBrowsersModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            model.Browsers = null;

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            _ = await controller.SupportedBrowsers(solution.CatalogueItemId, model);

            applicationTypeDetails.BrowsersSupported.Should().BeEquivalentTo(new HashSet<SupportedBrowser>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SupportedBrowsers_WithBrowsers_SetsBrowsersSupported(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            SupportedBrowsersModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            _ = await controller.SupportedBrowsers(solution.CatalogueItemId, model);

            applicationTypeDetails.BrowsersSupported.Should()
                .BeEquivalentTo(
                    model.Browsers.Where(b => b.Checked)
                        .Select(
                            b =>
                                new SupportedBrowser
                                {
                                    BrowserName = b.BrowserName, MinimumBrowserVersion = b.MinimumBrowserVersion,
                                })
                        .ToHashSet());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SupportedBrowsers_IsNotMobileResponsive_SetsMobileResponsiveNull(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            SupportedBrowsersModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            model.MobileResponsive = null;

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            _ = await controller.SupportedBrowsers(solution.CatalogueItemId, model);

            applicationTypeDetails.MobileResponsive.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SupportedBrowsers_IsMobileResponsive_SetsMobileResponsiveNull(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            SupportedBrowsersModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            model.MobileResponsive = true.ToYesNo();

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            _ = await controller.SupportedBrowsers(solution.CatalogueItemId, model);

            applicationTypeDetails.MobileResponsive.Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SupportedBrowsers_Valid_Redirects(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            SupportedBrowsersModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            model.MobileResponsive = true.ToYesNo();

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            var result = (await controller.SupportedBrowsers(solution.CatalogueItemId, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.BrowserBased));
        }

        [Theory]
        [MockAutoData]
        public static async Task PluginsOrExtensions_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.PlugInsOrExtensions(solutionId)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task PluginsOrExtensions_ValidSolutionId_ReturnsViewWithModel(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var expectedModel = new PlugInsOrExtensionsModel(solution.CatalogueItem);

            var result = (await controller.PlugInsOrExtensions(solution.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_PluginsOrExtensions_InvalidModel_ReturnsViewWithModel(
            CatalogueItemId solutionId,
            PlugInsOrExtensionsModel model,
            BrowserBasedController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-value");

            var result = (await controller.PlugInsOrExtensions(solutionId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().Be(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_PluginsOrExtensions_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            PlugInsOrExtensionsModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.PlugInsOrExtensions(solutionId, model)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_PluginsOrExtensions_NullApplicationType_ReturnsBadRequestResult(
            Solution solution,
            PlugInsOrExtensionsModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns((ApplicationTypeDetail)null);

            var result = (await controller.PlugInsOrExtensions(solution.CatalogueItemId, model)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_PluginsOrExtensions_PluginsRequired_SetsApplicationPlugins(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            PlugInsOrExtensionsModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            model.PlugInsRequired = true.ToYesNo();

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            _ = await controller.PlugInsOrExtensions(solution.CatalogueItemId, model);

            applicationTypeDetails.Plugins.Should().NotBeNull();
            applicationTypeDetails.Plugins.Required.Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_PluginsOrExtensions_PluginsNotRequired_SetsApplicationPlugins(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            PlugInsOrExtensionsModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            model.PlugInsRequired = null;

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            _ = await controller.PlugInsOrExtensions(solution.CatalogueItemId, model);

            applicationTypeDetails.Plugins.Should().NotBeNull();
            applicationTypeDetails.Plugins.Required.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_PluginsOrExtensions_Valid_Redirects(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            PlugInsOrExtensionsModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            model.PlugInsRequired = null;

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            var result = (await controller.PlugInsOrExtensions(solution.CatalogueItemId, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.BrowserBased));
        }

        [Theory]
        [MockAutoData]
        public static async Task ConnectivityAndResolution_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.ConnectivityAndResolution(solutionId)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ConnectivityAndResolution_NullApplicationType_ReturnsBadRequestResult(
            Solution solution,
            ConnectivityAndResolutionModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns((ApplicationTypeDetail)null);

            var result = (await controller.ConnectivityAndResolution(solution.CatalogueItemId, model)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task ConnectivityAndResolution_ValidSolutionId_ReturnsViewWithModel(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var expectedModel = new ConnectivityAndResolutionModel(solution.CatalogueItem);

            var result = (await controller.ConnectivityAndResolution(solution.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ConnectivityAndResolution_InvalidModel_ReturnsView(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            ConnectivityAndResolutionModel model,
            BrowserBasedController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-value");

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = (await controller.ConnectivityAndResolution(solution.CatalogueItemId, model)).As<ViewResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ConnectivityAndResolution_Valid_SetsApplication(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            ConnectivityAndResolutionModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            _ = await controller.ConnectivityAndResolution(solution.CatalogueItemId, model);

            applicationTypeDetails.MinimumConnectionSpeed.Should().Be(model.SelectedConnectionSpeed);
            applicationTypeDetails.MinimumDesktopResolution.Should().Be(model.SelectedScreenResolution);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ConnectivityAndResolution_Valid_Redirects(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            ConnectivityAndResolutionModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            var result = (await controller.ConnectivityAndResolution(solution.CatalogueItemId, model))
                .As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.BrowserBased));
        }

        [Theory]
        [MockAutoData]
        public static async Task HardwareRequirements_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.HardwareRequirements(solutionId)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task HardwareRequirements_ValidSolutionId_ReturnsViewWithModel(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var expectedModel = new HardwareRequirementsModel(solution.CatalogueItem);

            var result = (await controller.HardwareRequirements(solution.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_HardwareRequirements_InvalidModel_ReturnsViewWithModel(
            CatalogueItemId solutionId,
            HardwareRequirementsModel model,
            BrowserBasedController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-value");

            var result = (await controller.HardwareRequirements(solutionId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().Be(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_HardwareRequirements_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            HardwareRequirementsModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.HardwareRequirements(solutionId, model)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_HardwareRequirements_NullApplicationType_ReturnsBadRequestResult(
            Solution solution,
            HardwareRequirementsModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns((ApplicationTypeDetail)null);

            var result = (await controller.HardwareRequirements(solution.CatalogueItemId, model)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_HardwareRequirements_Valid_SetsApplication(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            HardwareRequirementsModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            _ = await controller.HardwareRequirements(solution.CatalogueItemId, model);

            applicationTypeDetails.HardwareRequirements.Should().Be(model.Description);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_HardwareRequirements_Valid_Redirects(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            HardwareRequirementsModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            var result = (await controller.HardwareRequirements(solution.CatalogueItemId, model))
                .As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.BrowserBased));
        }

        [Theory]
        [MockAutoData]
        public static async Task AdditionalInformation_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.AdditionalInformation(solutionId)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task AdditionalInformation_ValidSolutionId_ReturnsViewWithModel(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var expectedModel = new AdditionalInformationModel(solution.CatalogueItem);

            var result = (await controller.AdditionalInformation(solution.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AdditionalInformation_InvalidModel_ReturnsViewWithModel(
            CatalogueItemId solutionId,
            AdditionalInformationModel model,
            BrowserBasedController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-value");

            var result = (await controller.AdditionalInformation(solutionId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().Be(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AdditionalInformation_NullApplicationType_ReturnsBadRequestResult(
            Solution solution,
            AdditionalInformationModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns((ApplicationTypeDetail)null);

            var result = (await controller.AdditionalInformation(solution.CatalogueItemId, model)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AdditionalInformation_Valid_SetsApplication(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            AdditionalInformationModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            _ = await controller.AdditionalInformation(solution.CatalogueItemId, model);

            applicationTypeDetails.AdditionalInformation.Should().Be(model.AdditionalInformation);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AdditionalInformation_Valid_Redirects(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            AdditionalInformationModel model,
            [Frozen] ISolutionsService solutionsService,
            BrowserBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            var result = (await controller.AdditionalInformation(solution.CatalogueItemId, model))
                .As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.BrowserBased));
        }
    }
}
