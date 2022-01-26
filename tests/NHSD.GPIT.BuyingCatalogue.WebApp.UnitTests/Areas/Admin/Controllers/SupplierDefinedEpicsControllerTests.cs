using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class SupplierDefinedEpicsControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SupplierDefinedEpicsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Dashboard_ReturnsView(
            SupplierDefinedEpicsController controller)
        {
            var result = (await controller.Dashboard()).As<ViewResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Dashboard_ReturnsModelWithEpics(
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            var epics = new List<Epic>
            {
                new Epic
                {
                    Capability = new Capability
                    {
                        Name = "Test Capability",
                    },
                    Name = "Test Epic",
                    Id = "S00001",
                },
            };

            var expectedModel = new SupplierDefinedEpicsDashboardModel(epics);

            supplierDefinedEpicsService.Setup(s => s.GetSupplierDefinedEpics())
                .ReturnsAsync(epics);

            var result = (await controller.Dashboard()).As<ViewResult>();

            result.Should().NotBeNull();

            var model = result.Model.As<SupplierDefinedEpicsDashboardModel>();
            model.Should().NotBeNull();
            model.Should().BeEquivalentTo(expectedModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddEpic_ReturnsView(
            SupplierDefinedEpicsController controller)
        {
            var result = (await controller.AddEpic()).As<ViewResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddEpic_ReturnsModelWithCapabilities(
            List<Capability> capabilities,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            SupplierDefinedEpicsController controller)
        {
            var expectedCapabilitiesSelectList = new SelectList(
                capabilities
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem(c.Name, c.Id.ToString())),
                "Value",
                "Text");

            capabilitiesService.Setup(s => s.GetCapabilities())
                .ReturnsAsync(capabilities);

            var result = (await controller.AddEpic()).As<ViewResult>();

            result.Should().NotBeNull();

            var model = result.Model.As<AddEditSupplierDefinedEpicModel>();

            model.Should().NotBeNull();

            model.Capabilities.Should().BeEquivalentTo(expectedCapabilitiesSelectList);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddEpic_InvalidModel_ReturnsView(
            AddEditSupplierDefinedEpicModel model,
            SupplierDefinedEpicsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.AddEpic(model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.Capabilities));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddEpic_InvalidModel_RepopulatesCapabilities(
            List<Capability> capabilities,
            AddEditSupplierDefinedEpicModel model,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            SupplierDefinedEpicsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var expectedCapabilitiesSelectList = new SelectList(
                capabilities
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem(c.Name, c.Id.ToString())),
                "Value",
                "Text");

            capabilitiesService.Setup(s => s.GetCapabilities())
                .ReturnsAsync(capabilities);

            var result = (await controller.AddEpic(model)).As<ViewResult>();

            result.Should().NotBeNull();

            var viewModel = result.Model.As<AddEditSupplierDefinedEpicModel>();

            viewModel.Should().NotBeNull();

            viewModel.Capabilities.Should().BeEquivalentTo(expectedCapabilitiesSelectList);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddEpic_ValidModel_AddsSupplierDefinedEpic(
            AddEditSupplierDefinedEpicModel model,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            await controller.AddEpic(model);

            supplierDefinedEpicsService.Verify(
                s => s.AddSupplierDefinedEpic(
                It.Is<AddEditSupplierDefinedEpic>(
                    m => m.CapabilityId == model.SelectedCapabilityId!.Value
                         && m.Name == model.Name
                         && m.Description == model.Description
                         && m.IsActive == model.IsActive!.Value)),
                Times.Once);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddEpic_ValidModel_RedirectsToDashboard(
            AddEditSupplierDefinedEpicModel model,
            SupplierDefinedEpicsController controller)
        {
            var result = (await controller.AddEpic(model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Dashboard));
        }
    }
}
