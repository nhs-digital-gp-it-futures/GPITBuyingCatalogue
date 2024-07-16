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
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(MobileTabletBasedController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task MobileTablet_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.MobileTablet(solutionId)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task MobileTablet_WithBrowserClientApplication_SetsCorrectBacklink(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IUrlHelper urlHelper,
            MobileTabletBasedController controller)
        {
            solution.ApplicationTypeDetail.ApplicationTypes.Add(
                ApplicationType.MobileTablet.AsString(EnumFormat.EnumMemberValue));

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            _ = await controller.MobileTablet(solution.CatalogueItemId);

            urlHelper.Received().Action(
                    Arg.Is<UrlActionContext>(y => y.Action == nameof(CatalogueSolutionsController.ApplicationType)));
        }

        [Theory]
        [MockAutoData]
        public static async Task MobileTablet_WithoutBrowserClientApplication_SetsCorrectBacklink(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IUrlHelper urlHelper,
            MobileTabletBasedController controller)
        {
            solution.ApplicationTypeDetail.ApplicationTypes = new HashSet<string>();

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            _ = await controller.MobileTablet(solution.CatalogueItemId);

            urlHelper.Received().Action(
                    Arg.Is<UrlActionContext>(y => y.Action == nameof(CatalogueSolutionsController.AddApplicationType)));
        }

        [Theory]
        [MockAutoData]
        public static async Task MobileTablet_Valid_ReturnsViewWithModel(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solution.ApplicationTypeDetail.ApplicationTypes = new HashSet<string>();

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var expectedModel = new MobileTabletBasedModel(solution.CatalogueItem);

            var result = (await controller.MobileTablet(solution.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task OperatingSystems_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.OperatingSystems(solutionId)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_OperatingSystems_ValidSolutionId_ReturnsViewWithModel(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var expectedModel = new OperatingSystemsModel(solution.CatalogueItem);

            var result = (await controller.OperatingSystems(solution.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_OperatingSystems_InvalidModel_ReturnsViewWithModel(
            CatalogueItemId solutionId,
            OperatingSystemsModel model,
            MobileTabletBasedController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-value");

            var result = (await controller.OperatingSystems(solutionId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().Be(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_OperatingSystems_NullApplicationType_ReturnsBadRequestResult(
            Solution solution,
            OperatingSystemsModel model,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns((ApplicationTypeDetail)null);

            var result = (await controller.OperatingSystems(solution.CatalogueItemId, model)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_OperatingSystems_Valid_SetsApplication(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            OperatingSystemsModel model,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            applicationTypeDetails.MobileOperatingSystems = null;

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            _ = await controller.OperatingSystems(solution.CatalogueItemId, model);

            applicationTypeDetails.MobileOperatingSystems!.OperatingSystemsDescription.Should().Be(model.Description);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_OperatingSystems_Existing_SetsApplication(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            OperatingSystemsModel model,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            applicationTypeDetails.MobileOperatingSystems = new() { OperatingSystems = null };

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            _ = await controller.OperatingSystems(solution.CatalogueItemId, model);

            applicationTypeDetails.MobileOperatingSystems!.OperatingSystems.Should()
                .BeEquivalentTo(
                    model.OperatingSystems
                        .Where(o => o.Checked)
                        .Select(o => o.OperatingSystemName)
                        .ToHashSet());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_OperatingSystems_Valid_Redirects(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            OperatingSystemsModel model,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            var result = (await controller.OperatingSystems(solution.CatalogueItemId, model))
                .As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.MobileTablet));
        }

        [Theory]
        [MockAutoData]
        public static async Task Connectivity_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.Connectivity(solutionId)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Connectivity_ValidSolutionId_ReturnsViewWithModel(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var expectedModel = new ConnectivityModel(solution.CatalogueItem);

            var result = (await controller.Connectivity(solution.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Connectivity_InvalidModel_ReturnsViewWithModel(
            CatalogueItemId solutionId,
            ConnectivityModel model,
            MobileTabletBasedController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-value");

            var result = (await controller.Connectivity(solutionId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().Be(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Connectivity_NullApplicationType_ReturnsBadRequestResult(
            Solution solution,
            ConnectivityModel model,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns((ApplicationTypeDetail)null);

            var result = (await controller.Connectivity(solution.CatalogueItemId, model)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Connectivity_Valid_SetsApplication(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            ConnectivityModel model,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            applicationTypeDetails.MobileConnectionDetails = null;

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            _ = await controller.Connectivity(solution.CatalogueItemId, model);

            applicationTypeDetails.MobileConnectionDetails!.MinimumConnectionSpeed.Should().Be(model.SelectedConnectionSpeed);
            applicationTypeDetails.MobileConnectionDetails!.Description.Should().Be(model.Description);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Connectivity_Existing_SetsApplication(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            ConnectivityModel model,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            applicationTypeDetails.MobileConnectionDetails = new() { ConnectionType = null };

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            _ = await controller.Connectivity(solution.CatalogueItemId, model);

            applicationTypeDetails.MobileConnectionDetails.ConnectionType.Should()
                .BeEquivalentTo(
                    model.ConnectionTypes
                        .Where(o => o.Checked)
                        .Select(o => o.ConnectionType)
                        .ToHashSet());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Connectivity_Valid_Redirects(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            ConnectivityModel model,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            var result = (await controller.Connectivity(solution.CatalogueItemId, model))
                .As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.MobileTablet));
        }

        [Theory]
        [MockAutoData]
        public static async Task MemoryAndStorage_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.MemoryAndStorage(solutionId)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_MemoryAndStorage_ValidSolutionId_ReturnsViewWithModel(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var expectedModel = new MemoryAndStorageModel(solution.CatalogueItem);

            var result = (await controller.MemoryAndStorage(solution.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_MemoryAndStorage_InvalidModel_ReturnsViewWithModel(
            CatalogueItemId solutionId,
            MemoryAndStorageModel model,
            MobileTabletBasedController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-value");

            var result = (await controller.MemoryAndStorage(solutionId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().Be(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_MemoryAndStorage_NullApplicationType_ReturnsBadRequestResult(
            Solution solution,
            MemoryAndStorageModel model,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns((ApplicationTypeDetail)null);

            var result = (await controller.MemoryAndStorage(solution.CatalogueItemId, model)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_MemoryAndStorage_Valid_SetsApplication(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            MemoryAndStorageModel model,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            applicationTypeDetails.MobileMemoryAndStorage = null;

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            _ = await controller.MemoryAndStorage(solution.CatalogueItemId, model);

            applicationTypeDetails.MobileMemoryAndStorage!.MinimumMemoryRequirement.Should().Be(model.SelectedMemorySize);
            applicationTypeDetails.MobileMemoryAndStorage!.Description.Should().Be(model.Description);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_MemoryAndStorage_Valid_Redirects(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            MemoryAndStorageModel model,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            var result = (await controller.MemoryAndStorage(solution.CatalogueItemId, model))
                .As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.MobileTablet));
        }

        [Theory]
        [MockAutoData]
        public static async Task ThirdPartyComponents_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.ThirdPartyComponents(solutionId)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ThirdPartyComponents_ValidSolutionId_ReturnsViewWithModel(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var expectedModel = new ThirdPartyComponentsModel(solution.CatalogueItem);

            var result = (await controller.ThirdPartyComponents(solution.CatalogueItemId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ThirdPartyComponents_InvalidModel_ReturnsViewWithModel(
            CatalogueItemId solutionId,
            ThirdPartyComponentsModel model,
            MobileTabletBasedController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-value");

            var result = (await controller.ThirdPartyComponents(solutionId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().Be(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ThirdPartyComponents_NullApplicationType_ReturnsBadRequestResult(
            Solution solution,
            ThirdPartyComponentsModel model,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns((ApplicationTypeDetail)null);

            var result = (await controller.ThirdPartyComponents(solution.CatalogueItemId, model)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ThirdPartyComponents_Valid_SetsApplication(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            ThirdPartyComponentsModel model,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            applicationTypeDetails.MobileThirdParty = null;

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            _ = await controller.ThirdPartyComponents(solution.CatalogueItemId, model);

            applicationTypeDetails.MobileThirdParty!.ThirdPartyComponents.Should().Be(model.ThirdPartyComponents);
            applicationTypeDetails.MobileThirdParty!.DeviceCapabilities.Should().Be(model.DeviceCapabilities);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ThirdPartyComponents_Valid_Redirects(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            ThirdPartyComponentsModel model,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            var result = (await controller.ThirdPartyComponents(solution.CatalogueItemId, model))
                .As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.MobileTablet));
        }

        [Theory]
        [MockAutoData]
        public static async Task HardwareRequirements_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.HardwareRequirements(solutionId)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_HardwareRequirements_ValidSolutionId_ReturnsViewWithModel(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
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
            MobileTabletBasedController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-value");

            var result = (await controller.HardwareRequirements(solutionId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().Be(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_HardwareRequirements_NullApplicationType_ReturnsBadRequestResult(
            Solution solution,
            HardwareRequirementsModel model,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
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
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            _ = await controller.HardwareRequirements(solution.CatalogueItemId, model);

            applicationTypeDetails.NativeMobileHardwareRequirements.Should().Be(model.Description);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_HardwareRequirements_Valid_Redirects(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            HardwareRequirementsModel model,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            var result = (await controller.HardwareRequirements(solution.CatalogueItemId, model))
                .As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.MobileTablet));
        }

        [Theory]
        [MockAutoData]
        public static async Task AdditionalInformation_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solutionId).Returns((CatalogueItem)null);

            var result = (await controller.AdditionalInformation(solutionId)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AdditionalInformation_ValidSolutionId_ReturnsViewWithModel(
            Solution solution,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
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
            MobileTabletBasedController controller)
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
            MobileTabletBasedController controller)
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
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            _ = await controller.AdditionalInformation(solution.CatalogueItemId, model);

            applicationTypeDetails.NativeMobileAdditionalInformation.Should().Be(model.AdditionalInformation);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AdditionalInformation_Valid_Redirects(
            Solution solution,
            ApplicationTypeDetail applicationTypeDetails,
            AdditionalInformationModel model,
            [Frozen] ISolutionsService solutionsService,
            MobileTabletBasedController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            solutionsService.GetApplicationType(solution.CatalogueItemId).Returns(applicationTypeDetails);

            var result = (await controller.AdditionalInformation(solution.CatalogueItemId, model))
                .As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.MobileTablet));
        }
    }
}
