﻿using System.Collections.Generic;
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
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;
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
        public static async Task Get_SelectCapabilities_ReturnsModel(
            SupplierDefinedEpicsController controller)
        {
            var result = (await controller.SelectCapabilities()).As<ViewResult>();

            result.Should().NotBeNull();

            var model = result.Model.As<FilterCapabilitiesModel>();

            model.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectCapabilities_InvalidModel_ReturnsView(
            FilterCapabilitiesModel model,
            SupplierDefinedEpicsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.SelectCapabilities(model)).As<ViewResult>();
            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectCapabilities_ValidModel_ReturnsView(
            FilterCapabilitiesModel model,
            SupplierDefinedEpicsController controller)
        {
            model.CapabilitySelectionItems = new SelectionModel[] { new SelectionModel { Id = "1", Selected = true } };
            var result = (await controller.SelectCapabilities(model)).As<RedirectToActionResult>();
            result.Should().NotBeNull();
            var expectedIds = model.CapabilitySelectionItems.Where(x => x.Selected).Select(x => x.Id).ToFilterString();
            result.RouteValues.Values.First().ToString().Should().BeEquivalentTo(expectedIds);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_AddEpic_ReturnsModel(
            SupplierDefinedEpicsController controller)
        {
            var result = controller.AddSupplierDefinedEpicDetails().As<ViewResult>();

            result.Should().NotBeNull();

            var model = result.Model.As<AddSupplierDefinedEpicDetailsModel>();

            model.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddEpic_InvalidModel_ReturnsView(
            AddSupplierDefinedEpicDetailsModel model,
            SupplierDefinedEpicsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.AddSupplierDefinedEpicDetails(model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.SelectedCapabilityIds));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddEpic_ValidModel_AddsSupplierDefinedEpic(
            AddSupplierDefinedEpicDetailsModel model,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            model.SelectedCapabilityIds = "1.2";
            _ = await controller.AddSupplierDefinedEpicDetails(model);

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
        public static async Task Post_AddEpic_ValidModel_RedirectsToReview(
            AddSupplierDefinedEpicDetailsModel model,
            SupplierDefinedEpicsController controller)
        {
            var result = (await controller.AddSupplierDefinedEpicDetails(model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.EditSupplierDefinedEpic));
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

            var result = (await controller.EditSupplierDefinedEpic(epicId)).As<BadRequestObjectResult>();

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

            var result = (await controller.EditSupplierDefinedEpic(epic.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditCapabilities_EpicNotFound_ReturnsBadRequestObjectResult(
            string epicId,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            supplierDefinedEpicsService.Setup(s => s.GetEpic(epicId))
                .ReturnsAsync((Epic)null);

            var result = (await controller.EditCapabilities(epicId)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
            result.Value.Should().Be($"No Supplier defined Epic found for Id: {epicId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditCapabilities_Valid_ReturnsModel(
            Epic epic,
            List<Capability> capabilities,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            epic.Capabilities = capabilities;

            capabilitiesService.Setup(s => s.GetCapabilities())
                .ReturnsAsync(capabilities);

            supplierDefinedEpicsService.Setup(s => s.GetEpic(epic.Id))
                .ReturnsAsync(epic);

            var expectedModel = new FilterCapabilitiesModel(capabilities, capabilities.Select(x => x.Id).ToList());

            var result = (await controller.EditCapabilities(epic.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(
                expectedModel,
                opt => opt.Excluding(m => m.BackLink)
                          .Excluding(m => m.NavModel));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditCapabilities_EpicNotFound_ReturnsBadRequestObjectResult(
            string epicId,
            FilterCapabilitiesModel model,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            supplierDefinedEpicsService.Setup(s => s.GetEpic(epicId))
                .ReturnsAsync((Epic)null);

            var result = (await controller.EditCapabilities(epicId, model)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
            result.Value.Should().Be($"No Supplier defined Epic found for Id: {epicId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditCapabilities_InvalidModel_ReturnsView(
            Epic epic,
            FilterCapabilitiesModel model,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            for (int i = 0; i < model.CapabilitySelectionItems.Length; i++)
            {
                model.CapabilitySelectionItems[i].Id = (i + 1).ToString();
            }

            supplierDefinedEpicsService.Setup(s => s.GetEpic(epic.Id))
                .ReturnsAsync(epic);

            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.EditCapabilities(epic.Id, model)).As<ViewResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditCapabilities_ValidModel_RedirectsToEditSupplierDefinedEpic(
            Epic epic,
            FilterCapabilitiesModel model,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            for (int i = 0; i < model.CapabilitySelectionItems.Length; i++)
            {
                model.CapabilitySelectionItems[i].Id = (i + 1).ToString();
            }

            supplierDefinedEpicsService.Setup(s => s.GetEpic(epic.Id))
                .ReturnsAsync(epic);

            var result = (await controller.EditCapabilities(epic.Id, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.EditSupplierDefinedEpic));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_EditEpic_InvalidModel_ReturnsView(
            EditSupplierDefinedEpicModel model,
            SupplierDefinedEpicsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = controller.EditSupplierDefinedEpic(model.Id, model).As<ViewResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditSupplierDefinedEpicDetails_EpicNotFound_ReturnsBadRequestObjectResult(
            string epicId,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            supplierDefinedEpicsService.Setup(s => s.GetEpic(epicId))
                .ReturnsAsync((Epic)null);

            var result = (await controller.EditSupplierDefinedEpicDetails(epicId)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
            result.Value.Should().Be($"No Supplier defined Epic found for Id: {epicId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditSupplierDefinedEpicDetails_Valid_ReturnsModel(
            Epic epic,
            List<CatalogueItem> items,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            supplierDefinedEpicsService.Setup(s => s.GetEpic(epic.Id))
                .ReturnsAsync(epic);

            supplierDefinedEpicsService.Setup(s => s.GetItemsReferencingEpic(epic.Id))
                .ReturnsAsync(items);

            var expectedModel = new EditSupplierDefinedEpicDetailsModel(epic, items)
            {
            };

            var result = (await controller.EditSupplierDefinedEpicDetails(epic.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSupplierDefinedEpicDetails_EpicNotFound_ReturnsBadRequestObjectResult(
            EditSupplierDefinedEpicDetailsModel model,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            supplierDefinedEpicsService.Setup(s => s.GetEpic(model.Id))
                .ReturnsAsync((Epic)null);

            var result = (await controller.EditSupplierDefinedEpicDetails(model.Id, model)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
            result.Value.Should().Be($"No Supplier defined Epic found for Id: {model.Id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditEpicDetails_InvalidModel_ReturnsView(
            EditSupplierDefinedEpicDetailsModel model,
            SupplierDefinedEpicsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.EditSupplierDefinedEpicDetails(model.Id, model)).As<ViewResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditEpicDetails_InvalidModel_RepopulatesRelatedItems(
            List<CatalogueItem> relatedItems,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            EditSupplierDefinedEpicDetailsModel model,
            Epic epic,
            SupplierDefinedEpicsController controller)
        {
            supplierDefinedEpicsService.Setup(s => s.GetEpic(model.Id))
                .ReturnsAsync(epic);
            supplierDefinedEpicsService.Setup(s => s.GetItemsReferencingEpic(epic.Id))
                .ReturnsAsync(relatedItems);
            model.RelatedItems = relatedItems;
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.EditSupplierDefinedEpicDetails(epic.Id, model)).As<ViewResult>();

            result.Should().NotBeNull();

            var viewModel = (EditSupplierDefinedEpicDetailsModel)result.Model;

            result.Model.As<EditSupplierDefinedEpicDetailsModel>().RelatedItems.Should().BeEquivalentTo(relatedItems);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditEpicDetails_ValidModel_EditsSupplierDefinedEpic(
            EditSupplierDefinedEpicDetailsModel model,
            List<Capability> capabilities,
            [Frozen] Mock<ISupplierDefinedEpicsService> supplierDefinedEpicsService,
            Epic epic,
            SupplierDefinedEpicsController controller)
        {
            epic.Capabilities = capabilities;

            supplierDefinedEpicsService.Setup(s => s.GetEpic(model.Id))
                .ReturnsAsync(epic);

            _ = await controller.EditSupplierDefinedEpicDetails(model.Id, model);

            supplierDefinedEpicsService.Verify(
                s => s.EditSupplierDefinedEpic(
                It.Is<AddEditSupplierDefinedEpic>(
                    m => m.Id == model.Id
                         && m.CapabilityIds.SequenceEqual(epic.Capabilities.Select(x => x.Id).ToList())
                         && m.Name == model.Name
                         && m.Description == model.Description
                         && m.IsActive == model.IsActive!.Value)),
                Times.Once);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_EditEpic_ValidModel_RedirectsToDashboard(
            EditSupplierDefinedEpicModel model,
            SupplierDefinedEpicsController controller)
        {
            var result = controller.EditSupplierDefinedEpic(model.Id, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Dashboard));
        }
    }
}
