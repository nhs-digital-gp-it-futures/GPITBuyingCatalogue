using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using Xunit;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Controllers
{
    public static class FilterControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(FilterController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Solutions");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(FilterController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_FilterCapabilities_NoSelectedItems_ReturnsExpectedResult(
            List<Capability> capabilities,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            FilterController controller)
        {
            capabilitiesService
                .Setup(x => x.GetReferencedCapabilities())
                .ReturnsAsync(capabilities);

            var result = await controller.FilterCapabilities();

            capabilitiesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new SelectCapabilitiesModel(capabilities, string.Empty, true);

            actualResult.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_FilterCapabilities_WithSelectedItems_ReturnsExpectedResult(
            List<Capability> capabilities,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            FilterController controller)
        {
            capabilitiesService
                .Setup(x => x.GetReferencedCapabilities())
                .ReturnsAsync(capabilities);

            var selected = new[] { capabilities.First().Id, capabilities.Last().Id }.ToFilterString();
            var result = await controller.FilterCapabilities(selected);

            capabilitiesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new SelectCapabilitiesModel(capabilities, selected, true);

            actualResult.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_FilterCapabilities_WithModelErrors_ReturnsExpectedResult(
            SelectCapabilitiesModel model,
            List<Capability> capabilities,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            FilterController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            capabilitiesService
                .Setup(x => x.GetReferencedCapabilities())
                .ReturnsAsync(capabilities);

            var result = await controller.FilterCapabilities(model);

            capabilitiesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new SelectCapabilitiesModel(capabilities, string.Empty, true);

            actualResult.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_FilterCapabilities_ReturnsExpectedResult(
            SelectCapabilitiesModel model,
            FilterController controller)
        {
            model.SelectedItems.ForEach(x => x.Selected = true);

            var expected = model.SelectedItems.Select(x => x.Id).ToFilterString();

            var result = await controller.FilterCapabilities(model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(FilterController.IncludeEpics));
            actualResult.ControllerName.Should().Be(typeof(FilterController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "selectedCapabilityIds", expected },
                { "selectedEpicIds", null },
                { "search", null },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_FilterCapabilities_HasSelectedEpics_PassesEpicsThrough(
            [Frozen] Mock<IEpicsService> epicsService,
            SelectCapabilitiesModel model,
            FilterController controller)
        {
            model.SelectedItems.ForEach(x => x.Selected = true);

            var expectedEpics = "C5E1.C5E2";

            epicsService.Setup(e => e.GetEpicsForSelectedCapabilities(
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(expectedEpics);

            var expected = model.SelectedItems.Select(x => x.Id).ToFilterString();

            var result = await controller.FilterCapabilities(model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(FilterController.IncludeEpics));
            actualResult.ControllerName.Should().Be(typeof(FilterController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "selectedCapabilityIds", expected },
                { "selectedEpicIds", expectedEpics },
                { "search", null },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_IncludeEpics_NoEpicsAvailable_ReturnsExpectedResult(
            [Frozen] Mock<IEpicsService> epicsService,
            FilterController controller)
        {
            const int capabilityId = 1;
            var selectedIds = $"{capabilityId}";

            epicsService
                .Setup(x => x.GetReferencedEpicsByCapabilityIds(new[] { capabilityId }))
                .ReturnsAsync(new List<Epic>());

            var result = await controller.IncludeEpics(selectedIds);

            epicsService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(SolutionsController.Index));
            actualResult.ControllerName.Should().Be(typeof(SolutionsController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "SelectedCapabilityIds", selectedIds },
                { "search", null },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_IncludeEpics_EpicsAvailable_ReturnsExpectedResult(
            List<Epic> epics,
            [Frozen] Mock<IEpicsService> epicsService,
            FilterController controller)
        {
            const int capabilityId = 1;
            var selectedIds = $"{capabilityId}";

            epicsService
                .Setup(x => x.GetReferencedEpicsByCapabilityIds(new[] { capabilityId }))
                .ReturnsAsync(epics);

            var result = await controller.IncludeEpics(selectedIds);

            epicsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new IncludeEpicsModel
            {
                SelectedCapabilityIds = selectedIds,
            };

            actualResult.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_IncludeEpics_WithModelErrors_ReturnsExpectedResult(
            IncludeEpicsModel model,
            FilterController controller)
        {
            model.IncludeEpics = null;
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = controller.IncludeEpics(model);
            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new IncludeEpicsModel
            {
                SelectedCapabilityIds = model.SelectedCapabilityIds,
            };

            actualResult.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_IncludeEpics_IncludeEpics_ReturnsExpectedResult(
            IncludeEpicsModel model,
            FilterController controller)
        {
            model.IncludeEpics = true;

            var result = controller.IncludeEpics(model);
            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(FilterController.FilterEpics));
            actualResult.ControllerName.Should().Be(typeof(FilterController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "selectedCapabilityIds", model.SelectedCapabilityIds },
                { "selectedEpicIds", null },
                { "search", null },
            });
        }

        [Theory]
        [CommonAutoData]
        public static void Post_IncludeEpics_DoNotIncludeEpics_ReturnsExpectedResult(
            IncludeEpicsModel model,
            FilterController controller)
        {
            model.IncludeEpics = false;

            var result = controller.IncludeEpics(model);
            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(SolutionsController.Index));
            actualResult.ControllerName.Should().Be(typeof(SolutionsController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "selectedCapabilityIds", model.SelectedCapabilityIds },
                { "selectedEpicIds", null },
                { "search", null },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_FilterEpics_NoSelectedItems_ReturnsExpectedResult(
            List<Capability> capabilities,
            List<Epic> epics,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            [Frozen] Mock<IEpicsService> epicsService,
            FilterController controller)
        {
            var capabilityIds = capabilities.Select(x => x.Id).ToList();
            var selectedCapabilityIds = capabilityIds.ToFilterString();

            capabilitiesService
                .Setup(x => x.GetCapabilitiesByIds(capabilityIds))
                .ReturnsAsync(capabilities);

            epicsService
                .Setup(x => x.GetReferencedEpicsByCapabilityIds(capabilityIds))
                .ReturnsAsync(epics);

            var result = await controller.FilterEpics(selectedCapabilityIds);

            capabilitiesService.VerifyAll();
            epicsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new FilterEpicsModel(capabilities, epics);

            actualResult.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_FilterEpics_WithSelectedItems_ReturnsExpectedResult(
            List<Capability> capabilities,
            List<Epic> epics,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            [Frozen] Mock<IEpicsService> epicsService,
            FilterController controller)
        {
            var capabilityIds = capabilities.Select(x => x.Id).ToList();
            var selectedCapabilityIds = capabilityIds.ToFilterString();
            var selectedEpicIds = new[] { epics.First().Id, epics.Last().Id }.ToFilterString();

            capabilitiesService
                .Setup(x => x.GetCapabilitiesByIds(capabilityIds))
                .ReturnsAsync(capabilities);

            epicsService
                .Setup(x => x.GetReferencedEpicsByCapabilityIds(capabilityIds))
                .ReturnsAsync(epics);

            var result = await controller.FilterEpics(selectedCapabilityIds, selectedEpicIds);

            capabilitiesService.VerifyAll();
            epicsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new FilterEpicsModel(capabilities, epics, selectedEpicIds);

            actualResult.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_FilterEpics_WithModelErrors_ReturnsExpectedResult(
            List<Capability> capabilities,
            List<Epic> epics,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            [Frozen] Mock<IEpicsService> epicsService,
            FilterEpicsModel model,
            FilterController controller)
        {
            var capabilityIds = capabilities.Select(x => x.Id).ToList();

            model.CapabilityIds = capabilityIds.ToFilterString();

            capabilitiesService
                .Setup(x => x.GetCapabilitiesByIds(capabilityIds))
                .ReturnsAsync(capabilities);

            epicsService
                .Setup(x => x.GetReferencedEpicsByCapabilityIds(capabilityIds))
                .ReturnsAsync(epics);

            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.FilterEpics(model);

            capabilitiesService.VerifyAll();
            epicsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new FilterEpicsModel(capabilities, epics);

            actualResult.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_FilterEpics_ReturnsExpectedResult(
            FilterEpicsModel model,
            FilterController controller)
        {
            model.SelectedItems.ForEach(x => x.Selected = true);

            var expected = model.SelectedItems.Select(x => x.Id).ToFilterString();

            var result = await controller.FilterEpics(model);
            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(SolutionsController.Index));
            actualResult.ControllerName.Should().Be(typeof(SolutionsController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "selectedCapabilityIds", model.CapabilityIds },
                { "selectedEpicIds", expected },
                { "search", null },
            });
        }
    }
}
