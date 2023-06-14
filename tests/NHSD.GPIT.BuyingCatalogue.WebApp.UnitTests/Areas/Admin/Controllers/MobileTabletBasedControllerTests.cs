using System.Collections.Generic;
using System.Linq;
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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.MobileTabletBasedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class MobileTabletBasedControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(MobileTabletBasedController).Should()
                .BeDecoratedWith<AuthorizeAttribute>(a => a.Policy == "AdminOnly");
            typeof(MobileTabletBasedController).Should()
                .BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Admin");
            typeof(MobileTabletBasedController).Should()
                .BeDecoratedWith<RouteAttribute>(r => r.Template == "admin/catalogue-solutions");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(MobileTabletBasedController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_MobileTablet_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            List<Integration> integrations,
            [Frozen] Mock<ISolutionsService> mockService,
            MobileTabletBasedController controller)
        {
            var catalogueItem = solution.CatalogueItem;
            solution.Integrations = JsonSerializer.Serialize(integrations);

            mockService.Setup(s => s.GetSolutionThin(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.MobileTablet(catalogueItem.Id)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionThin(catalogueItem.Id));
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(new MobileTabletBasedModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_MobileTablet_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockService,
            MobileTabletBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.MobileTablet(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_OperatingSystems_ValidId_ReturnsViewWithExpectedModel(
           Solution solution,
           List<Integration> integrations,
           [Frozen] Mock<ISolutionsService> mockService,
           MobileTabletBasedController controller)
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
            MobileTabletBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.OperatingSystems(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OperatingSystems_Saves_And_RedirectsToMobileTablet(
            CatalogueItemId catalogueItemId,
            OperatingSystemsModel model,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            MobileTabletBasedController controller)
        {
            mockService.Setup(s => s.GetClientApplication(catalogueItemId)).ReturnsAsync(clientApplication);
            clientApplication.ClientApplicationTypes.Clear();

            var actual = (await controller.OperatingSystems(catalogueItemId, model)).As<RedirectToActionResult>();

            clientApplication.MobileOperatingSystems.OperatingSystems = model.OperatingSystems
                .Where(o => o.Checked)
                .Select(o => o.OperatingSystemName)
                .ToHashSet();

            clientApplication.ClientApplicationTypes.Add(ApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue));
            clientApplication.MobileOperatingSystems.OperatingSystemsDescription = model.Description;

            mockService.Verify(s => s.SaveClientApplication(catalogueItemId, clientApplication));
            actual.ActionName.Should().Be(nameof(MobileTabletBasedController.MobileTablet));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Connectivity_ValidId_ReturnsViewWithExpectedModel(
           Solution solution,
           List<Integration> integrations,
           [Frozen] Mock<ISolutionsService> mockService,
           MobileTabletBasedController controller)
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
            MobileTabletBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.Connectivity(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Connectivity_Saves_And_RedirectsToMobileTablet(
            CatalogueItemId catalogueItemId,
            ConnectivityModel model,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            MobileTabletBasedController controller)
        {
            mockService.Setup(s => s.GetClientApplication(catalogueItemId)).ReturnsAsync(clientApplication);
            clientApplication.ClientApplicationTypes.Clear();

            var actual = (await controller.Connectivity(catalogueItemId, model)).As<RedirectToActionResult>();

            clientApplication.MobileConnectionDetails.MinimumConnectionSpeed = model.SelectedConnectionSpeed;
            clientApplication.MobileConnectionDetails.Description = model.Description;

            clientApplication.MobileConnectionDetails.ConnectionType = model.ConnectionTypes
                .Where(o => o.Checked)
                .Select(o => o.ConnectionType)
                .ToHashSet();

            clientApplication.ClientApplicationTypes.Add(ApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue));

            mockService.Verify(s => s.SaveClientApplication(catalogueItemId, clientApplication));
            actual.ActionName.Should().Be(nameof(MobileTabletBasedController.MobileTablet));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_MemoryAndStorage_ValidId_ReturnsViewWithExpectedModel(
           Solution solution,
           List<Integration> integrations,
           [Frozen] Mock<ISolutionsService> mockService,
           MobileTabletBasedController controller)
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
            MobileTabletBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.MemoryAndStorage(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_MemoryAndStorage_Saves_And_RedirectsToMobileTablet(
            CatalogueItemId catalogueItemId,
            MemoryAndStorageModel model,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            MobileTabletBasedController controller)
        {
            mockService.Setup(s => s.GetClientApplication(catalogueItemId)).ReturnsAsync(clientApplication);
            clientApplication.ClientApplicationTypes.Clear();

            var actual = (await controller.MemoryAndStorage(catalogueItemId, model)).As<RedirectToActionResult>();

            clientApplication.MobileMemoryAndStorage.MinimumMemoryRequirement = model.SelectedMemorySize;
            clientApplication.MobileMemoryAndStorage.Description = model.Description;
            clientApplication.ClientApplicationTypes.Add(ApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue));

            mockService.Verify(s => s.SaveClientApplication(catalogueItemId, clientApplication));
            actual.ActionName.Should().Be(nameof(MobileTabletBasedController.MobileTablet));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ThirdPartyComponents_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            List<Integration> integrations,
            [Frozen] Mock<ISolutionsService> mockService,
            MobileTabletBasedController controller)
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
            MobileTabletBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.ThirdPartyComponents(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ThirdPartyComponents_Saves_And_RedirectsToMobileTablet(
            CatalogueItemId catalogueItemId,
            ThirdPartyComponentsModel model,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            MobileTabletBasedController controller)
        {
            mockService.Setup(s => s.GetClientApplication(catalogueItemId)).ReturnsAsync(clientApplication);
            clientApplication.ClientApplicationTypes.Clear();

            var actual = (await controller.ThirdPartyComponents(catalogueItemId, model)).As<RedirectToActionResult>();

            clientApplication.MobileThirdParty.ThirdPartyComponents = model.ThirdPartyComponents;
            clientApplication.MobileThirdParty.DeviceCapabilities = model.DeviceCapabilities;
            clientApplication.ClientApplicationTypes.Add(ApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue));

            mockService.Verify(s => s.SaveClientApplication(catalogueItemId, clientApplication));
            actual.ActionName.Should().Be(nameof(MobileTabletBasedController.MobileTablet));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HardwareRequirements_ValidId_ReturnsViewWithExpectedModel(
             Solution solution,
             List<Integration> integrations,
             [Frozen] Mock<ISolutionsService> mockService,
             MobileTabletBasedController controller)
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
            MobileTabletBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.HardwareRequirements(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_Saves_And_RedirectsToMobileTablet(
            CatalogueItemId catalogueItemId,
            HardwareRequirementsModel model,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            MobileTabletBasedController controller)
        {
            mockService.Setup(s => s.GetClientApplication(catalogueItemId)).ReturnsAsync(clientApplication);
            clientApplication.ClientApplicationTypes.Clear();

            var actual = (await controller.HardwareRequirements(catalogueItemId, model)).As<RedirectToActionResult>();

            clientApplication.NativeMobileHardwareRequirements = model.Description;
            clientApplication.ClientApplicationTypes.Add(ApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue));

            mockService.Verify(s => s.SaveClientApplication(catalogueItemId, clientApplication));
            actual.ActionName.Should().Be(nameof(MobileTabletBasedController.MobileTablet));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalInformation_ValidId_ReturnsViewWithExpectedModel(
            Solution solution,
            List<Integration> integrations,
            [Frozen] Mock<ISolutionsService> mockService,
            MobileTabletBasedController controller)
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
            MobileTabletBasedController controller)
        {
            mockService.Setup(s => s.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.AdditionalInformation(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_Saves_And_RedirectsToMobileTablet(
            CatalogueItemId catalogueItemId,
            AdditionalInformationModel model,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> mockService,
            MobileTabletBasedController controller)
        {
            mockService.Setup(s => s.GetClientApplication(catalogueItemId)).ReturnsAsync(clientApplication);
            clientApplication.ClientApplicationTypes.Clear();

            var actual = (await controller.AdditionalInformation(catalogueItemId, model)).As<RedirectToActionResult>();

            clientApplication.NativeMobileAdditionalInformation = model.AdditionalInformation;
            clientApplication.ClientApplicationTypes.Add(ApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue));

            mockService.Verify(s => s.SaveClientApplication(catalogueItemId, clientApplication));
            actual.ActionName.Should().Be(nameof(MobileTabletBasedController.MobileTablet));
            actual.ControllerName.Should().BeNull();
            actual.RouteValues["solutionId"].Should().Be(catalogueItemId);
        }
    }
}
