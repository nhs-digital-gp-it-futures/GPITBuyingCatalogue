using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.BrowserBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class BrowserBasedControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(BrowserBasedController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_BrowserBased_GetsSolutionFromService(
            Solution solution,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(solution.CatalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            await controller.BrowserBased(solution.CatalogueItemId);

            mockService.Verify(s => s.GetSolutionThin(solution.CatalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_BrowserBased_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.BrowserBased(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_BrowserBased_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(solution.CatalogueItem);

            var actual = (await controller.BrowserBased(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new BrowserBasedModel(solution.CatalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupportedBrowsers_GetsSolutionFromService(
           CatalogueItemId catalogueItemId,
           CatalogueItem catalogueItem,
           [Frozen] Mock<ISolutionsService> mockService,
           BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            await controller.SupportedBrowsers(catalogueItemId);

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupportedBrowsers_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.SupportedBrowsers(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupportedBrowsers_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.SupportedBrowsers(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new SupportedBrowsersModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SupportedBrowsers_CallsSaveClientApplication(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            SupportedBrowsersModel model,
            ApplicationTypeDetail clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetApplicationType(catalogueItemId))
                .ReturnsAsync(clientApplication);

            await controller.SupportedBrowsers(catalogueItemId, model);

            mockService.Verify(s => s.SaveApplicationType(catalogueItemId, clientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SupportedBrowsers_RedirectsToBrowserBased(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            SupportedBrowsersModel model,
            ApplicationTypeDetail clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetApplicationType(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var actual = (await controller.SupportedBrowsers(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(BrowserBasedController.BrowserBased));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SupportedBrowsers_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.SupportedBrowsers(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new SupportedBrowsersModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PlugInsOrExtensions_GetsSolutionFromService(
           CatalogueItemId catalogueItemId,
           CatalogueItem catalogueItem,
           [Frozen] Mock<ISolutionsService> mockService,
           BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            await controller.PlugInsOrExtensions(catalogueItemId);

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PlugInsOrExtensions_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.PlugInsOrExtensions(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PlugInsOrExtensions_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.PlugInsOrExtensions(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new PlugInsOrExtensionsModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PlugInsOrExtensions_CallsSaveClientApplication(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            PlugInsOrExtensionsModel model,
            ApplicationTypeDetail clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetApplicationType(catalogueItemId))
                .ReturnsAsync(clientApplication);

            await controller.PlugInsOrExtensions(catalogueItemId, model);

            mockService.Verify(s => s.SaveApplicationType(catalogueItemId, clientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PlugInsOrExtensions_RedirectsToBrowserBased(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            PlugInsOrExtensionsModel model,
            ApplicationTypeDetail clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetApplicationType(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var actual = (await controller.PlugInsOrExtensions(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(BrowserBasedController.BrowserBased));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PlugInsOrExtensions_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.PlugInsOrExtensions(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new PlugInsOrExtensionsModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConnectivityAndResolution_GetsSolutionFromService(
           CatalogueItemId catalogueItemId,
           CatalogueItem catalogueItem,
           [Frozen] Mock<ISolutionsService> mockService,
           BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            await controller.ConnectivityAndResolution(catalogueItemId);

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConnectivityAndResolution_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.ConnectivityAndResolution(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConnectivityAndResolution_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.ConnectivityAndResolution(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ConnectivityAndResolutionModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConnectivityAndResolution_CallsSaveClientApplication(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            ConnectivityAndResolutionModel model,
            ApplicationTypeDetail clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetApplicationType(catalogueItemId))
                .ReturnsAsync(clientApplication);

            await controller.ConnectivityAndResolution(catalogueItemId, model);

            mockService.Verify(s => s.SaveApplicationType(catalogueItemId, clientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConnectivityAndResolution_RedirectsToBrowserBased(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            ConnectivityAndResolutionModel model,
            ApplicationTypeDetail clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetApplicationType(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var actual = (await controller.ConnectivityAndResolution(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(BrowserBasedController.BrowserBased));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConnectivityAndResolution_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.ConnectivityAndResolution(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ConnectivityAndResolutionModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HardwareRequirements_GetsSolutionFromService(
           CatalogueItemId catalogueItemId,
           CatalogueItem catalogueItem,
           [Frozen] Mock<ISolutionsService> mockService,
           BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            await controller.HardwareRequirements(catalogueItemId);

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HardwareRequirements_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.HardwareRequirements(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HardwareRequirements_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.HardwareRequirements(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new HardwareRequirementsModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_CallsSaveClientApplication(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            HardwareRequirementsModel model,
            ApplicationTypeDetail clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetApplicationType(catalogueItemId))
                .ReturnsAsync(clientApplication);

            await controller.HardwareRequirements(catalogueItemId, model);

            mockService.Verify(s => s.SaveApplicationType(catalogueItemId, clientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_RedirectsToBrowserBased(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            HardwareRequirementsModel model,
            ApplicationTypeDetail clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetApplicationType(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var actual = (await controller.HardwareRequirements(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(BrowserBasedController.BrowserBased));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.HardwareRequirements(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new HardwareRequirementsModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalInformation_GetsSolutionFromService(
           CatalogueItemId catalogueItemId,
           CatalogueItem catalogueItem,
           [Frozen] Mock<ISolutionsService> mockService,
           BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            await controller.AdditionalInformation(catalogueItemId);

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalInformation_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.AdditionalInformation(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalInformation_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.AdditionalInformation(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new AdditionalInformationModel(catalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_CallsSaveClientApplication(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            AdditionalInformationModel model,
            ApplicationTypeDetail clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetApplicationType(catalogueItemId))
                .ReturnsAsync(clientApplication);

            await controller.AdditionalInformation(catalogueItemId, model);

            mockService.Verify(s => s.SaveApplicationType(catalogueItemId, clientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_RedirectsToBrowserBased(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            AdditionalInformationModel model,
            ApplicationTypeDetail clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockService.Setup(s => s.GetApplicationType(catalogueItemId))
                .ReturnsAsync(clientApplication);

            var actual = (await controller.AdditionalInformation(catalogueItemId, model)).As<RedirectToActionResult>();

            actual.ActionName.Should().Be(nameof(BrowserBasedController.BrowserBased));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_InvalidId_ReturnsBadRequestResult(
            CatalogueItem catalogueItem,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            BrowserBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.AdditionalInformation(catalogueItemId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItemId));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new AdditionalInformationModel(catalogueItem));
        }
    }
}
