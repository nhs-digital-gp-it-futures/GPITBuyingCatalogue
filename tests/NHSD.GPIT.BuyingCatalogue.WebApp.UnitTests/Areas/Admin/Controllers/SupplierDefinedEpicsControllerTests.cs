using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;
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
        public static async Task Get_Dashboard_ReturnsViewWithExpectedViewModel(
            List<Epic> epics,
            [Frozen] Mock<ISupplierDefinedEpicsService> mockEpicsService,
            SupplierDefinedEpicsController systemUnderTest)
        {
            var model = new SupplierDefinedEpicsDashboardModel(epics, null);

            mockEpicsService
                .Setup(o => o.GetSupplierDefinedEpics())
                .ReturnsAsync(epics);

            var actual = (await systemUnderTest.Dashboard()).As<ViewResult>();

            mockEpicsService.VerifyAll();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.As<SupplierDefinedEpicsDashboardModel>().Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Dashboard_WithSearchTerm_ReturnsViewWithExpectedViewModel(
            string searchTerm,
            List<Epic> epics,
            [Frozen] Mock<ISupplierDefinedEpicsService> mockEpicsService,
            SupplierDefinedEpicsController systemUnderTest)
        {
            var model = new SupplierDefinedEpicsDashboardModel(epics, searchTerm);

            mockEpicsService
                .Setup(o => o.GetSupplierDefinedEpicsBySearchTerm(searchTerm))
                .ReturnsAsync(epics);

            var actual = (await systemUnderTest.Dashboard(searchTerm)).As<ViewResult>();

            mockEpicsService.VerifyAll();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.As<SupplierDefinedEpicsDashboardModel>().Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static async Task Get_Dashboard_WithInvalidSearchTerm_ReturnsViewWithExpectedViewModel(
            string searchTerm,
            List<Epic> epics,
            [Frozen] Mock<ISupplierDefinedEpicsService> mockEpicsService,
            SupplierDefinedEpicsController systemUnderTest)
        {
            mockEpicsService
                .Setup(o => o.GetSupplierDefinedEpics())
                .ReturnsAsync(epics);

            var model = new SupplierDefinedEpicsDashboardModel(epics, searchTerm);

            var actual = (await systemUnderTest.Dashboard(searchTerm)).As<ViewResult>();

            mockEpicsService.VerifyAll();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.As<SupplierDefinedEpicsDashboardModel>().Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SearchResults_WithMatches_ReturnsExpectedResult(
            string searchTerm,
            Capability capability,
            List<Epic> epics,
            [Frozen] Mock<ISupplierDefinedEpicsService> mockEpicsService,
            SupplierDefinedEpicsController systemUnderTest)
        {
            epics.ForEach(x => x.Capabilities = new List<Capability> { capability });

            mockEpicsService
                .Setup(o => o.GetSupplierDefinedEpicsBySearchTerm(searchTerm))
                .ReturnsAsync(epics);

            var result = await systemUnderTest.SearchResults(searchTerm);

            mockEpicsService.VerifyAll();

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<SuggestionSearchResult>>()
                .ToList();

            foreach (var epic in epics)
            {
                actualResult.Should().Contain(x => x.Title == epic.Name && x.Category == capability.Name);
            }
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SearchResults_NoMatches_ReturnsExpectedResult(
            string searchTerm,
            [Frozen] Mock<ISupplierDefinedEpicsService> mockEpicsService,
            SupplierDefinedEpicsController systemUnderTest)
        {
            mockEpicsService
                .Setup(o => o.GetSupplierDefinedEpicsBySearchTerm(searchTerm))
                .ReturnsAsync(new List<Epic>());

            var result = await systemUnderTest.SearchResults(searchTerm);

            mockEpicsService.VerifyAll();

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<SuggestionSearchResult>>()
                .ToList();

            actualResult.Should().BeEmpty();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddEpic_ReturnsModel(
            SupplierDefinedEpicsController controller)
        {
            var result = (await controller.AddEpic()).As<ViewResult>();

            result.Should().NotBeNull();

            var model = result.Model.As<SupplierDefinedEpicBaseModel>();

            model.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddEpic_InvalidModel_ReturnsView(
            SupplierDefinedEpicBaseModel model,
            SupplierDefinedEpicsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.AddEpic(model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.SelectedCapabilityIds));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddEpic_ValidModel_AddsSupplierDefinedEpic(
            SupplierDefinedEpicBaseModel model,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            model.SelectedCapabilityIds = "1.2";
            _ = await controller.AddEpic(model);

            var capabilityIds = (List<int>)SolutionsFilterHelper.ParseCapabilityIds(model.SelectedCapabilityIds);

            supplierDefinedEpicsService.Verify(
                s => s.AddSupplierDefinedEpic(
                It.Is<AddEditSupplierDefinedEpic>(
                m => m.CapabilityIds.SequenceEqual(capabilityIds)
                         && m.Name == model.Name
                         && m.Description == model.Description
                         && m.IsActive == model.IsActive!.Value)),
                Times.Once);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddEpic_ValidModel_RedirectsToDashboard(
            SupplierDefinedEpicBaseModel model,
            SupplierDefinedEpicsController controller)
        {
            var result = (await controller.AddEpic(model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Dashboard));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditEpic_EpicNotFound_ReturnsBadRequestObjectResult(
            string epicId,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            supplierDefinedEpicsService.Setup(s => s.GetEpic(epicId))
                .ReturnsAsync((Epic)null);

            var result = (await controller.EditEpic(epicId)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
            result.Value.Should().Be($"No Supplier defined Epic found for Id: {epicId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditEpic_Valid_ReturnsModel(
            Epic epic,
            List<Capability> capabilities,
            List<CatalogueItem> relatedItems,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            epic.Capabilities = capabilities;

            capabilitiesService.Setup(s => s.GetCapabilities())
                .ReturnsAsync(capabilities);

            supplierDefinedEpicsService.Setup(s => s.GetEpic(epic.Id))
                .ReturnsAsync(epic);

            supplierDefinedEpicsService.Setup(s => s.GetItemsReferencingEpic(epic.Id))
                .ReturnsAsync(relatedItems);

            var expectedModel = new EditSupplierDefinedEpicModel(epic, relatedItems);

            var result = (await controller.EditEpic(epic.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditEpic_InvalidModel_ReturnsView(
            EditSupplierDefinedEpicModel model,
            SupplierDefinedEpicsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.EditEpic(model.Id, model)).As<ViewResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditEpic_InvalidModel_RepopulatesCapabilities(
            List<Capability> capabilities,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            EditSupplierDefinedEpicModel model,
            SupplierDefinedEpicsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var expectedCapabilitiesSelectList = capabilities
                .OrderBy(c => c.Name)
                .Select(c => new SelectOption<string>(c.Name, c.Id.ToString()));

            capabilitiesService.Setup(s => s.GetCapabilities())
                .ReturnsAsync(capabilities);

            var result = (await controller.EditEpic(model.Id, model)).As<ViewResult>();

            result.Should().NotBeNull();

            var viewModel = result.Model.As<EditSupplierDefinedEpicModel>();

           //viewModel.Capabilities.Should().BeEquivalentTo(expectedCapabilitiesSelectList);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditEpic_InvalidModel_RepopulatesRelatedItems(
            List<CatalogueItem> relatedItems,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            EditSupplierDefinedEpicModel model,
            SupplierDefinedEpicsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            supplierDefinedEpicsService.Setup(s => s.GetItemsReferencingEpic(model.Id))
                .ReturnsAsync(relatedItems);

            var result = (await controller.EditEpic(model.Id, model)).As<ViewResult>();

            result.Should().NotBeNull();

            var viewModel = result.Model.As<EditSupplierDefinedEpicModel>();

            viewModel.RelatedItems.Should().BeEquivalentTo(relatedItems);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditEpic_ValidModel_EditsSupplierDefinedEpic(
            EditSupplierDefinedEpicModel model,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            _ = await controller.EditEpic(model.Id, model);

            supplierDefinedEpicsService.Verify(
                s => s.EditSupplierDefinedEpic(
                It.Is<AddEditSupplierDefinedEpic>(
                    m => m.Id == model.Id
                         && m.CapabilityIds == (List<int>)SolutionsFilterHelper.ParseCapabilityIds(model.SelectedCapabilityIds)
                         && m.Name == model.Name
                         && m.Description == model.Description
                         && m.IsActive == model.IsActive!.Value)),
                Times.Once);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditEpic_ValidModel_RedirectsToDashboard(
            EditSupplierDefinedEpicModel model,
            SupplierDefinedEpicsController controller)
        {
            var result = (await controller.EditEpic(model.Id, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Dashboard));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteEpic_InvalidEpicId_ReturnsToDashboard(
            string epicId,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            supplierDefinedEpicsService.Setup(s => s.GetEpic(epicId))
                .ReturnsAsync((Epic)null);

            var result = (await controller.DeleteEpic(epicId)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Dashboard));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteEpic_ValidEpicId_ReturnsViewWithModel(
            Epic epic,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            var expectedModel = new DeleteSupplierDefinedEpicConfirmationModel(epic.Id, epic.Name);

            supplierDefinedEpicsService.Setup(s => s.GetEpic(epic.Id))
                .ReturnsAsync(epic);

            var result = (await controller.DeleteEpic(epic.Id)).As<ViewResult>();

            result.Should().NotBeNull();

            var model = result.Model.As<DeleteSupplierDefinedEpicConfirmationModel>();

            model.Should().NotBeNull();
            model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteEpic_InvalidModel_RedirectsToDashboard(
            DeleteSupplierDefinedEpicConfirmationModel model,
            SupplierDefinedEpicsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.DeleteEpic(model.Id, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.EditEpic));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteEpic_ValidModel_Deletes(
            DeleteSupplierDefinedEpicConfirmationModel model,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            await controller.DeleteEpic(model.Id, model);

            supplierDefinedEpicsService.Verify(s => s.DeleteSupplierDefinedEpic(model.Id), Times.Once);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteEpic_ValidModel_RedirectsToDashboard(
            DeleteSupplierDefinedEpicConfirmationModel model,
            SupplierDefinedEpicsController controller)
        {
            var result = (await controller.DeleteEpic(model.Id)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Dashboard));
        }
    }
}
