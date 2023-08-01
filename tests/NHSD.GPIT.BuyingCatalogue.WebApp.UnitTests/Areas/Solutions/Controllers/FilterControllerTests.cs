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
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;
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
                .Setup(x => x.GetReferencedCapabilities())
                .ReturnsAsync(capabilities);

            var result = await controller.FilterCapabilities();

            capabilitiesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new FilterCapabilitiesModel(capabilities, true, null);

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

            var selected = new Dictionary<int, string[]>(new[]
            {
                new KeyValuePair<int, string[]>(capabilities.First().Id, System.Array.Empty<string>()),
                new KeyValuePair<int, string[]>(capabilities.Last().Id, System.Array.Empty<string>()),
            });

            var result = await controller.FilterCapabilities(selected.ToFilterString());

            capabilitiesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new FilterCapabilitiesModel(capabilities, true, selected.Keys);

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
                .Setup(x => x.GetReferencedCapabilities())
                .ReturnsAsync(capabilities);

            var result = await controller.FilterCapabilities(model);

            capabilitiesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new FilterCapabilitiesModel(capabilities, true, null);

            actualResult.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_FilterCapabilities_ReturnsExpectedResult(
            FilterCapabilitiesModel model,
            FilterController controller)
        {
            model.SelectedItems.ForEach((x, i) =>
            {
                x.Selected = true;
                x.Id = i.ToString();
            });

            var expected = new Dictionary<int, string[]>(model.SelectedItems
                  .Where(x => x.Selected)
                  .Select(x => int.Parse(x.Id))
                  .Select(x => new KeyValuePair<int, string[]>(x, null))).ToFilterString();

            var result = await controller.FilterCapabilities(model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(FilterController.IncludeEpics));
            actualResult.ControllerName.Should().Be(typeof(FilterController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "selected", expected },
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
            var selected = new Dictionary<int, string[]>(new[] { new KeyValuePair<int, string[]>(capabilityId, null) })
                .ToFilterString();

            epicsService
                .Setup(x => x.GetReferencedEpicsByCapabilityIds(new[] { capabilityId }))
                .ReturnsAsync(new List<Epic>());

            var result = await controller.IncludeEpics(selected);

            epicsService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(SolutionsController.Index));
            actualResult.ControllerName.Should().Be(typeof(SolutionsController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "selected", selected },
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
            var selected = new Dictionary<int, string[]>(new[] { new KeyValuePair<int, string[]>(capabilityId, null) })
                .ToFilterString();

            epicsService
                .Setup(x => x.GetReferencedEpicsByCapabilityIds(new[] { capabilityId }))
                .ReturnsAsync(epics);

            var result = await controller.IncludeEpics(selected);

            epicsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new IncludeEpicsModel
            {
                Selected = selected,
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
                Selected = model.Selected,
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
                { "selected", model.Selected },
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
                { "selected", model.Selected },
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
            var selected = new Dictionary<int, string[]>(capabilities.Select(x => new KeyValuePair<int, string[]>(x.Id, null)));
            var selectedIdsFilterString = selected.ToFilterString();

            capabilitiesService
                .Setup(x => x.GetCapabilitiesByIds(selected.Keys))
                .ReturnsAsync(capabilities);

            epicsService
                .Setup(x => x.GetReferencedEpicsByCapabilityIds(selected.Keys))
                .ReturnsAsync(epics);

            var result = await controller.FilterEpics(selectedIdsFilterString);

            capabilitiesService.VerifyAll();
            epicsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new FilterEpicsModel(capabilities, epics, selected);

            actualResult.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_FilterEpics_WithSelectedItems_ReturnsExpectedResult(
            List<Capability> capabilities,
            Epic epic,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            [Frozen] Mock<IEpicsService> epicsService,
            FilterController controller)
        {
            capabilities.ForEach(c =>
            {
                c.Epics.Clear();
                c.Epics.Add(epic);
            });

            var selected = new Dictionary<int, string[]>(capabilities.Select(x => new KeyValuePair<int, string[]>(x.Id, new string[] { epic.Id })));
            var selectedIdsFilterString = selected.ToFilterString();

            capabilitiesService
                .Setup(x => x.GetCapabilitiesByIds(selected.Keys))
                .ReturnsAsync(capabilities);

            epicsService
                .Setup(x => x.GetReferencedEpicsByCapabilityIds(selected.Keys))
                .ReturnsAsync(new List<Epic> { epic });

            var result = await controller.FilterEpics(selectedIdsFilterString);

            capabilitiesService.VerifyAll();
            epicsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new FilterEpicsModel(capabilities, new List<Epic> { epic }, selected);

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
            var selected = new Dictionary<int, string[]>(capabilities.Select(x => new KeyValuePair<int, string[]>(x.Id, null)));

            model.Selected = selected.ToFilterString();

            capabilitiesService
                .Setup(x => x.GetCapabilitiesByIds(selected.Keys))
                .ReturnsAsync(capabilities);

            epicsService
                .Setup(x => x.GetReferencedEpicsByCapabilityIds(selected.Keys))
                .ReturnsAsync(epics);

            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.FilterEpics(model);

            capabilitiesService.VerifyAll();
            epicsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new FilterEpicsModel(capabilities, epics, selected);

            actualResult.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_FilterEpics_ReturnsExpectedResult(
            FilterEpicsModel model,
            FilterController controller)
        {
            model.SelectedItems.ForEach((x, i) =>
            {
                x.Id = $"{i},{i}";
                x.Selected = true;
            });

            model.Selected = new Dictionary<int, string[]>(model.SelectedItems
                .Select(x => new KeyValuePair<int, string[]>(int.Parse(x.Id.Split(",")[0]), null)))
                .ToFilterString();

            var expected = new Dictionary<int, string[]>(model.SelectedItems
                .Select(x => new KeyValuePair<int, string[]>(int.Parse(x.Id.Split(",")[0]), new string[] { x.Id.Split(",")[1] })))
                .ToFilterString();

            var result = await controller.FilterEpics(model);
            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(SolutionsController.Index));
            actualResult.ControllerName.Should().Be(typeof(SolutionsController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "selected", expected },
                { "search", null },
            });
        }
    }
}
