using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using EnumsNET;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.DesktopBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class DesktopBasedControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(DesktopBasedController).Should()
                .BeDecoratedWith<AuthorizeAttribute>(a => a.Policy == "AdminOnly");
            typeof(DesktopBasedController).Should()
                .BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Admin");
            typeof(DesktopBasedController).Should()
                .BeDecoratedWith<RouteAttribute>(r => r.Template == "admin/catalogue-solutions");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(DesktopBasedController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Desktop_ValidId_ReturnsViewWithExpectedModel(
         Solution solution,
         List<Integration> integrations,
         [Frozen] Mock<ISolutionsService> mockService,
         DesktopBasedController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            solution.Integrations = JsonSerializer.Serialize(integrations);

            mockService.Setup(s => s.GetSolutionThin(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.Desktop(catalogueItem.Id)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItem.Id));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new DesktopBasedModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Desktop_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            DesktopBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.Desktop(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_OperatingSystems_ValidId_ReturnsViewWithExpectedModel(
           Solution solution,
           List<Integration> integrations,
           [Frozen] Mock<ISolutionsService> mockService,
           DesktopBasedController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            solution.Integrations = JsonSerializer.Serialize(integrations);

            mockService.Setup(s => s.GetSolutionThin(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.OperatingSystems(catalogueItem.Id)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItem.Id));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new OperatingSystemsModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_OperatingSystems_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            DesktopBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.OperatingSystems(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OperatingSystems_Saves_And_RedirectsToDesktop(
            CatalogueItemId catalogueItemId,
            OperatingSystemsModel model,
            ApplicationTypes clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            DesktopBasedController controller)
        {
            mockService.Setup(s => s.GetApplicationTypes(catalogueItemId)).ReturnsAsync(clientApplication);
            clientApplication.ClientApplicationTypes.Clear();

            var actual = (await controller.OperatingSystems(catalogueItemId, model)).As<RedirectToActionResult>();

            clientApplication.ClientApplicationTypes.Add(ApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue));
            clientApplication.NativeDesktopOperatingSystemsDescription = model.Description;

            mockService.Verify(s => s.SaveApplicationType(catalogueItemId, clientApplication));
            actual.ActionName.Should().Be(nameof(DesktopBasedController.Desktop));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Connectivity_ValidId_ReturnsViewWithExpectedModel(
           Solution solution,
           List<Integration> integrations,
           [Frozen] Mock<ISolutionsService> mockService,
           DesktopBasedController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            solution.Integrations = JsonSerializer.Serialize(integrations);

            mockService.Setup(s => s.GetSolutionThin(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.Connectivity(catalogueItem.Id)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItem.Id));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ConnectivityModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Connectivity_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            DesktopBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.Connectivity(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Connectivity_Saves_And_RedirectsToDesktop(
            CatalogueItemId catalogueItemId,
            ConnectivityModel model,
            ApplicationTypes clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            DesktopBasedController controller)
        {
            mockService.Setup(s => s.GetApplicationTypes(catalogueItemId)).ReturnsAsync(clientApplication);
            clientApplication.ClientApplicationTypes.Clear();

            var actual = (await controller.Connectivity(catalogueItemId, model)).As<RedirectToActionResult>();

            clientApplication.NativeDesktopMinimumConnectionSpeed = model.SelectedConnectionSpeed;

            clientApplication.ClientApplicationTypes.Add(ApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue));

            mockService.Verify(s => s.SaveApplicationType(catalogueItemId, clientApplication));
            actual.ActionName.Should().Be(nameof(DesktopBasedController.Desktop));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_MemoryAndStorage_ValidId_ReturnsViewWithExpectedModel(
           Solution solution,
           List<Integration> integrations,
           [Frozen] Mock<ISolutionsService> mockService,
           DesktopBasedController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            solution.Integrations = JsonSerializer.Serialize(integrations);

            mockService.Setup(s => s.GetSolutionThin(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.MemoryAndStorage(catalogueItem.Id)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItem.Id));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new MemoryAndStorageModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_MemoryAndStorage_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            DesktopBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.MemoryAndStorage(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_MemoryAndStorage_Saves_And_RedirectsToDesktop(
            CatalogueItemId catalogueItemId,
            MemoryAndStorageModel model,
            ApplicationTypes clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            DesktopBasedController controller)
        {
            mockService.Setup(s => s.GetApplicationTypes(catalogueItemId)).ReturnsAsync(clientApplication);
            clientApplication.ClientApplicationTypes.Clear();

            var actual = (await controller.MemoryAndStorage(catalogueItemId, model)).As<RedirectToActionResult>();

            clientApplication.NativeDesktopMemoryAndStorage.MinimumMemoryRequirement = model.SelectedMemorySize;
            clientApplication.NativeDesktopMemoryAndStorage.StorageRequirementsDescription = model.StorageSpace;
            clientApplication.NativeDesktopMemoryAndStorage.MinimumCpu = model.ProcessingPower;
            clientApplication.NativeDesktopMemoryAndStorage.RecommendedResolution = model.SelectedResolution;
            clientApplication.ClientApplicationTypes.Add(ApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue));

            mockService.Verify(s => s.SaveApplicationType(catalogueItemId, clientApplication));
            actual.ActionName.Should().Be(nameof(DesktopBasedController.Desktop));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ThirdPartyComponents_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            List<Integration> integrations,
            [Frozen] Mock<ISolutionsService> mockService,
            DesktopBasedController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            solution.Integrations = JsonSerializer.Serialize(integrations);

            mockService.Setup(s => s.GetSolutionThin(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.ThirdPartyComponents(catalogueItem.Id)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItem.Id));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new ThirdPartyComponentsModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ThirdPartyComponents_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            DesktopBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.ThirdPartyComponents(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ThirdPartyComponents_Saves_And_RedirectsToDesktop(
            CatalogueItemId catalogueItemId,
            ThirdPartyComponentsModel model,
            ApplicationTypes clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            DesktopBasedController controller)
        {
            mockService.Setup(s => s.GetApplicationTypes(catalogueItemId)).ReturnsAsync(clientApplication);
            clientApplication.ClientApplicationTypes.Clear();

            var actual = (await controller.ThirdPartyComponents(catalogueItemId, model)).As<RedirectToActionResult>();

            clientApplication.NativeDesktopThirdParty.ThirdPartyComponents = model.ThirdPartyComponents;
            clientApplication.NativeDesktopThirdParty.DeviceCapabilities = model.DeviceCapabilities;
            clientApplication.ClientApplicationTypes.Add(ApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue));

            mockService.Verify(s => s.SaveApplicationType(catalogueItemId, clientApplication));
            actual.ActionName.Should().Be(nameof(DesktopBasedController.Desktop));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HardwareRequirements_ValidId_ReturnsViewWithExpectedModel(
             Solution solution,
             List<Integration> integrations,
             [Frozen] Mock<ISolutionsService> mockService,
             DesktopBasedController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            solution.Integrations = JsonSerializer.Serialize(integrations);

            mockService.Setup(s => s.GetSolutionThin(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.HardwareRequirements(catalogueItem.Id)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItem.Id));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new HardwareRequirementsModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HardwareRequirements_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            DesktopBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.HardwareRequirements(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_Saves_And_RedirectsToDesktop(
            CatalogueItemId catalogueItemId,
            HardwareRequirementsModel model,
            ApplicationTypes clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            DesktopBasedController controller)
        {
            mockService.Setup(s => s.GetApplicationTypes(catalogueItemId)).ReturnsAsync(clientApplication);
            clientApplication.ClientApplicationTypes.Clear();

            var actual = (await controller.HardwareRequirements(catalogueItemId, model)).As<RedirectToActionResult>();

            clientApplication.NativeDesktopHardwareRequirements = model.Description;
            clientApplication.ClientApplicationTypes.Add(ApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue));

            mockService.Verify(s => s.SaveApplicationType(catalogueItemId, clientApplication));
            actual.ActionName.Should().Be(nameof(DesktopBasedController.Desktop));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalInformation_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            List<Integration> integrations,
            [Frozen] Mock<ISolutionsService> mockService,
            DesktopBasedController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            solution.Integrations = JsonSerializer.Serialize(integrations);

            mockService.Setup(s => s.GetSolutionThin(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.AdditionalInformation(catalogueItem.Id)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItem.Id));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new AdditionalInformationModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalInformation_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            DesktopBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.AdditionalInformation(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_Saves_And_RedirectsToDesktop(
            CatalogueItemId catalogueItemId,
            AdditionalInformationModel model,
            ApplicationTypes clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            DesktopBasedController controller)
        {
            mockService.Setup(s => s.GetApplicationTypes(catalogueItemId)).ReturnsAsync(clientApplication);
            clientApplication.ClientApplicationTypes.Clear();

            var actual = (await controller.AdditionalInformation(catalogueItemId, model)).As<RedirectToActionResult>();

            clientApplication.NativeDesktopAdditionalInformation = model.AdditionalInformation;
            clientApplication.ClientApplicationTypes.Add(ApplicationType.Desktop.AsString(EnumFormat.EnumMemberValue));

            mockService.Verify(s => s.SaveApplicationType(catalogueItemId, clientApplication));
            actual.ActionName.Should().Be(nameof(DesktopBasedController.Desktop));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }
    }
}
