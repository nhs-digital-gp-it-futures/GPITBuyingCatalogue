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
        [MockAutoData]
        public static async Task Get_Dashboard_ReturnsViewWithExpectedViewModel(
            List<Epic> epics,
            [Frozen] ISupplierDefinedEpicsService mockEpicsService,
            SupplierDefinedEpicsController systemUnderTest)
        {
            var model = new SupplierDefinedEpicsDashboardModel(epics, null);

            mockEpicsService.GetSupplierDefinedEpics().Returns(epics);

            var actual = (await systemUnderTest.Dashboard()).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.As<SupplierDefinedEpicsDashboardModel>().Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Dashboard_WithSearchTerm_ReturnsViewWithExpectedViewModel(
            string searchTerm,
            List<Epic> epics,
            [Frozen] ISupplierDefinedEpicsService mockEpicsService,
            SupplierDefinedEpicsController systemUnderTest)
        {
            var model = new SupplierDefinedEpicsDashboardModel(epics, searchTerm);

            mockEpicsService.GetSupplierDefinedEpicsBySearchTerm(searchTerm).Returns(epics);

            var actual = (await systemUnderTest.Dashboard(searchTerm)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.As<SupplierDefinedEpicsDashboardModel>().Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static async Task Get_Dashboard_WithInvalidSearchTerm_ReturnsViewWithExpectedViewModel(
            string searchTerm,
            List<Epic> epics,
            [Frozen] ISupplierDefinedEpicsService mockEpicsService,
            SupplierDefinedEpicsController systemUnderTest)
        {
            mockEpicsService.GetSupplierDefinedEpics().Returns(epics);

            var model = new SupplierDefinedEpicsDashboardModel(epics, searchTerm);

            var actual = (await systemUnderTest.Dashboard(searchTerm)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.As<SupplierDefinedEpicsDashboardModel>().Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SearchResults_WithMatches_ReturnsExpectedResult(
            string searchTerm,
            Capability capability,
            List<Epic> epics,
            [Frozen] ISupplierDefinedEpicsService mockEpicsService,
            SupplierDefinedEpicsController systemUnderTest)
        {
            epics.ForEach(x => x.Capabilities = new List<Capability> { capability });

            mockEpicsService.GetSupplierDefinedEpicsBySearchTerm(searchTerm).Returns(epics);

            var result = await systemUnderTest.SearchResults(searchTerm);

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<HtmlEncodedSuggestionSearchResult>>()
                .ToList();

            foreach (var epic in epics)
            {
                actualResult.Should().Contain(x => x.Title == epic.Name && x.Category == capability.Name);
            }
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SearchResults_NoMatches_ReturnsExpectedResult(
            string searchTerm,
            [Frozen] ISupplierDefinedEpicsService mockEpicsService,
            SupplierDefinedEpicsController systemUnderTest)
        {
            mockEpicsService.GetSupplierDefinedEpicsBySearchTerm(searchTerm).Returns(new List<Epic>());

            var result = await systemUnderTest.SearchResults(searchTerm);

            var actualResult = result.As<JsonResult>()
                .Value.As<IEnumerable<HtmlEncodedSuggestionSearchResult>>()
                .ToList();

            actualResult.Should().BeEmpty();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SelectCapabilities_ReturnsModel(
            List<Capability> capabilities,
            [Frozen] ICapabilitiesService capabilitiesService,
            SupplierDefinedEpicsController controller)
        {
            capabilitiesService.GetCapabilities().Returns(capabilities);

            var result = (await controller.SelectCapabilities()).As<ViewResult>();

            result.Should().NotBeNull();

            var model = result.Model.As<FilterCapabilitiesModel>();

            model.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectCapabilities_InvalidModel_ReturnsView(
            List<Capability> capabilities,
            [Frozen] ICapabilitiesService capabilitiesService,
            FilterCapabilitiesModel model,
            SupplierDefinedEpicsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            capabilitiesService.GetCapabilities().Returns(capabilities);

            var result = (await controller.SelectCapabilities(model)).As<ViewResult>();
            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static void Get_AddEpic_ReturnsModel(
            SupplierDefinedEpicsController controller)
        {
            var result = controller.AddSupplierDefinedEpicDetails().As<ViewResult>();

            result.Should().NotBeNull();

            var model = result.Model.As<AddSupplierDefinedEpicDetailsModel>();

            model.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
        public static async Task Post_AddEpic_ValidModel_AddsSupplierDefinedEpic(
            AddSupplierDefinedEpicDetailsModel model,
            [Frozen] ISupplierDefinedEpicsService supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            model.SelectedCapabilityIds = "1.2";
            _ = await controller.AddSupplierDefinedEpicDetails(model);

            var capabilityIds = (List<int>)SolutionsFilterHelper.ParseCapabilityIds(model.SelectedCapabilityIds);

            await supplierDefinedEpicsService.Received().AddSupplierDefinedEpic(
                Arg.Is<AddEditSupplierDefinedEpic>(
                m => m.CapabilityIds.SequenceEqual(capabilityIds)
                         && m.Name == model.Name
                         && m.Description == model.Description
                         && m.IsActive == model.IsActive!.Value));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddEpic_ValidModel_RedirectsToReview(
            AddSupplierDefinedEpicDetailsModel model,
            SupplierDefinedEpicsController controller)
        {
            var result = (await controller.AddSupplierDefinedEpicDetails(model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.EditSupplierDefinedEpic));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditEpic_EpicNotFound_ReturnsBadRequestObjectResult(
            string epicId,
            [Frozen] ISupplierDefinedEpicsService supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            supplierDefinedEpicsService.GetEpic(epicId).Returns((Epic)null);

            var result = (await controller.EditSupplierDefinedEpic(epicId)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
            result.Value.Should().Be($"No Supplier defined Epic found for Id: {epicId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditEpic_Valid_ReturnsModel(
            Epic epic,
            List<Capability> capabilities,
            List<CatalogueItem> relatedItems,
            [Frozen] ICapabilitiesService capabilitiesService,
            [Frozen] ISupplierDefinedEpicsService supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            epic.Capabilities = capabilities;

            capabilitiesService.GetCapabilities().Returns(capabilities);

            supplierDefinedEpicsService.GetEpic(epic.Id).Returns(epic);

            supplierDefinedEpicsService.GetItemsReferencingEpic(epic.Id).Returns(relatedItems);

            var expectedModel = new EditSupplierDefinedEpicModel(epic, relatedItems);

            var result = (await controller.EditSupplierDefinedEpic(epic.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditCapabilities_EpicNotFound_ReturnsBadRequestObjectResult(
            string epicId,
            [Frozen] ISupplierDefinedEpicsService supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            supplierDefinedEpicsService.GetEpic(epicId).Returns((Epic)null);

            var result = (await controller.EditCapabilities(epicId)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
            result.Value.Should().Be($"No Supplier defined Epic found for Id: {epicId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditCapabilities_Valid_ReturnsModel(
            Epic epic,
            List<Capability> capabilities,
            [Frozen] ICapabilitiesService capabilitiesService,
            [Frozen] ISupplierDefinedEpicsService supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            epic.Capabilities = capabilities;

            capabilitiesService.GetCapabilities().Returns(capabilities);

            supplierDefinedEpicsService.GetEpic(epic.Id).Returns(epic);

            var expectedModel = new FilterCapabilitiesModel(capabilities, capabilities.Select(x => x.Id).ToList());

            var result = (await controller.EditCapabilities(epic.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(
                expectedModel,
                opt => opt.Excluding(m => m.BackLink)
                          .Excluding(m => m.NavModel));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditCapabilities_EpicNotFound_ReturnsBadRequestObjectResult(
            string epicId,
            FilterCapabilitiesModel model,
            [Frozen] ISupplierDefinedEpicsService supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            supplierDefinedEpicsService.GetEpic(epicId).Returns((Epic)null);

            var result = (await controller.EditCapabilities(epicId, model)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
            result.Value.Should().Be($"No Supplier defined Epic found for Id: {epicId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditCapabilities_InvalidModel_ReturnsView(
            Epic epic,
            FilterCapabilitiesModel model,
            List<Capability> capabilities,
            [Frozen] ICapabilitiesService capabilitiesService,
            [Frozen] ISupplierDefinedEpicsService supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            for (int i = 0; i < model.CapabilitySelectionItems.Length; i++)
            {
                model.CapabilitySelectionItems[i].Id = (i + 1).ToString();
            }

            supplierDefinedEpicsService.GetEpic(epic.Id).Returns(epic);
            capabilitiesService.GetCapabilities().Returns(capabilities);

            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.EditCapabilities(epic.Id, model)).As<ViewResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditCapabilities_ValidModel_RedirectsToEditSupplierDefinedEpic(
            Epic epic,
            FilterCapabilitiesModel model,
            [Frozen] ISupplierDefinedEpicsService supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            for (int i = 0; i < model.CapabilitySelectionItems.Length; i++)
            {
                model.CapabilitySelectionItems[i].Id = (i + 1).ToString();
            }

            supplierDefinedEpicsService.GetEpic(epic.Id).Returns(epic);

            var result = (await controller.EditCapabilities(epic.Id, model)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.EditSupplierDefinedEpic));
        }

        [Theory]
        [MockAutoData]
        public static void Post_EditEpic_InvalidModel_ReturnsView(
            EditSupplierDefinedEpicModel model,
            SupplierDefinedEpicsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = controller.EditSupplierDefinedEpic(model.Id, model).As<ViewResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditSupplierDefinedEpicDetails_EpicNotFound_ReturnsBadRequestObjectResult(
            string epicId,
            [Frozen] ISupplierDefinedEpicsService supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            supplierDefinedEpicsService.GetEpic(epicId).Returns((Epic)null);

            var result = (await controller.EditSupplierDefinedEpicDetails(epicId)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
            result.Value.Should().Be($"No Supplier defined Epic found for Id: {epicId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditSupplierDefinedEpicDetails_Valid_ReturnsModel(
            Epic epic,
            List<CatalogueItem> items,
            [Frozen] ISupplierDefinedEpicsService supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            supplierDefinedEpicsService.GetEpic(epic.Id).Returns(epic);

            supplierDefinedEpicsService.GetItemsReferencingEpic(epic.Id).Returns(items);

            var expectedModel = new EditSupplierDefinedEpicDetailsModel(epic, items)
            {
            };

            var result = (await controller.EditSupplierDefinedEpicDetails(epic.Id)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSupplierDefinedEpicDetails_EpicNotFound_ReturnsBadRequestObjectResult(
            EditSupplierDefinedEpicDetailsModel model,
            [Frozen] ISupplierDefinedEpicsService supplierDefinedEpicsService,
            SupplierDefinedEpicsController controller)
        {
            supplierDefinedEpicsService.GetEpic(model.Id).Returns((Epic)null);

            var result = (await controller.EditSupplierDefinedEpicDetails(model.Id, model)).As<BadRequestObjectResult>();

            result.Should().NotBeNull();
            result.Value.Should().Be($"No Supplier defined Epic found for Id: {model.Id}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditEpicDetails_InvalidModel_ReturnsView(
            EditSupplierDefinedEpicDetailsModel model,
            SupplierDefinedEpicsController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.EditSupplierDefinedEpicDetails(model.Id, model)).As<ViewResult>();

            result.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditEpicDetails_InvalidModel_RepopulatesRelatedItems(
            List<CatalogueItem> relatedItems,
            [Frozen] ISupplierDefinedEpicsService supplierDefinedEpicsService,
            EditSupplierDefinedEpicDetailsModel model,
            Epic epic,
            SupplierDefinedEpicsController controller)
        {
            supplierDefinedEpicsService.GetEpic(model.Id).Returns(epic);
            supplierDefinedEpicsService.GetItemsReferencingEpic(epic.Id).Returns(relatedItems);
            model.RelatedItems = relatedItems;
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.EditSupplierDefinedEpicDetails(epic.Id, model)).As<ViewResult>();

            result.Should().NotBeNull();

            var viewModel = (EditSupplierDefinedEpicDetailsModel)result.Model;

            result.Model.As<EditSupplierDefinedEpicDetailsModel>().RelatedItems.Should().BeEquivalentTo(relatedItems);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditEpicDetails_ValidModel_EditsSupplierDefinedEpic(
            EditSupplierDefinedEpicDetailsModel model,
            List<Capability> capabilities,
            [Frozen] ISupplierDefinedEpicsService supplierDefinedEpicsService,
            Epic epic,
            SupplierDefinedEpicsController controller)
        {
            epic.Capabilities = capabilities;

            supplierDefinedEpicsService.GetEpic(model.Id).Returns(epic);

            _ = await controller.EditSupplierDefinedEpicDetails(model.Id, model);

            await supplierDefinedEpicsService.Received().EditSupplierDefinedEpic(
                Arg.Is<AddEditSupplierDefinedEpic>(
                    m => m.Id == model.Id
                         && m.CapabilityIds.SequenceEqual(epic.Capabilities.Select(x => x.Id).ToList())
                         && m.Name == model.Name
                         && m.Description == model.Description
                         && m.IsActive == model.IsActive!.Value));
        }

        [Theory]
        [MockAutoData]
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
