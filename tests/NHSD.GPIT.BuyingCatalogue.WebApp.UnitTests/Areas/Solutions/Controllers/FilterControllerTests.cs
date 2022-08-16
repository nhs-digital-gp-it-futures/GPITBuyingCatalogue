using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using Xunit;

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
                .Setup(x => x.GetCapabilities())
                .ReturnsAsync(capabilities);

            var result = await controller.FilterCapabilities();

            capabilitiesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new FilterCapabilitiesModel(capabilities);

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
                .Setup(x => x.GetCapabilities())
                .ReturnsAsync(capabilities);

            var selected = $"{capabilities.First().Id}{FilterConstants.Delimiter}{capabilities.Last().Id}";
            var result = await controller.FilterCapabilities(selected);

            capabilitiesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new FilterCapabilitiesModel(capabilities, selected);

            actualResult.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_FilterCapabilities_WithModelErrors_ReturnsExpectedResult(
            FilterCapabilitiesModel model,
            List<Capability> capabilities,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            FilterController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            capabilitiesService
                .Setup(x => x.GetCapabilities())
                .ReturnsAsync(capabilities);

            var result = await controller.FilterCapabilities(model);

            capabilitiesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new FilterCapabilitiesModel(capabilities);

            actualResult.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_FilterCapabilities_ReturnsExpectedResult(
            FilterCapabilitiesModel model,
            FilterController controller)
        {
            model.SelectedItems.ForEach(x => x.Selected = true);

            var expected = string.Join(
                FilterConstants.Delimiter,
                model.SelectedItems.Select(x => x.Id));

            var result = await controller.FilterCapabilities(model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(FilterController.IncludeEpics));
            actualResult.ControllerName.Should().Be(typeof(FilterController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "selectedCapabilityIds", expected },
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
                .Setup(x => x.GetActiveEpicsByCapabilityIds(new[] { capabilityId }))
                .ReturnsAsync(new List<Epic>());

            var result = await controller.IncludeEpics(selectedIds);

            epicsService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(FilterController.FilterCapabilities));
            actualResult.ControllerName.Should().Be(typeof(FilterController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "selectedIds", selectedIds },
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
                .Setup(x => x.GetActiveEpicsByCapabilityIds(new[] { capabilityId }))
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

            actualResult.ActionName.Should().Be(nameof(FilterController.FilterCapabilities));
            actualResult.ControllerName.Should().Be(typeof(FilterController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "selectedIds", model.SelectedCapabilityIds },
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
            var selectedCapabilityIds = string.Join(FilterConstants.Delimiter, capabilityIds);

            capabilitiesService
                .Setup(x => x.GetCapabilitiesByIds(capabilityIds))
                .ReturnsAsync(capabilities);

            epicsService
                .Setup(x => x.GetActiveEpicsByCapabilityIds(capabilityIds))
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
            var selectedCapabilityIds = string.Join(FilterConstants.Delimiter, capabilityIds);
            var selectedEpicIds = $"{epics.First().Id}{FilterConstants.Delimiter}{epics.Last().Id}";

            capabilitiesService
                .Setup(x => x.GetCapabilitiesByIds(capabilityIds))
                .ReturnsAsync(capabilities);

            epicsService
                .Setup(x => x.GetActiveEpicsByCapabilityIds(capabilityIds))
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

            model.CapabilityIds = string.Join(FilterConstants.Delimiter, capabilityIds);

            capabilitiesService
                .Setup(x => x.GetCapabilitiesByIds(capabilityIds))
                .ReturnsAsync(capabilities);

            epicsService
                .Setup(x => x.GetActiveEpicsByCapabilityIds(capabilityIds))
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

            var expected = string.Join(FilterConstants.Delimiter, model.SelectedItems.Select(x => x.Id));

            var result = await controller.FilterEpics(model);
            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(FilterController.FilterEpics));
            actualResult.ControllerName.Should().Be(typeof(FilterController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "selectedCapabilityIds", model.CapabilityIds },
                { "selectedIds", expected },
            });
        }
    }
}
