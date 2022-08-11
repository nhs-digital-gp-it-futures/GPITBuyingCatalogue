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
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Controllers
{
    public static class FilterControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(FilterController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Solutions");
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
                .Setup(s => s.GetCapabilities())
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
                .Setup(s => s.GetCapabilities())
                .ReturnsAsync(capabilities);

            var selected = $"{capabilities.First().Id}{FilterCapabilitiesModel.FilterDelimiter}{capabilities.Last().Id}";
            var result = await controller.FilterCapabilities(selected);

            capabilitiesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new FilterCapabilitiesModel(capabilities, selected);

            actualResult.Model.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_FilterCapabilities_NoSelectedItems_ReturnsExpectedResult(
            FilterCapabilitiesModel model,
            FilterController controller)
        {
            model.Items.ForEach(x => x.Selected = false);

            var result = controller.FilterCapabilities(model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(FilterController.FilterCapabilities));
            actualResult.ControllerName.Should().Be(typeof(FilterController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "selectedIds", string.Empty },
            });
        }

        [Theory]
        [CommonAutoData]
        public static void Post_FilterCapabilities_WithSelectedItems_ReturnsExpectedResult(
            FilterCapabilitiesModel model,
            FilterController controller)
        {
            model.Items.ForEach(x => x.Selected = true);

            var expected = string.Join(
                FilterCapabilitiesModel.FilterDelimiter,
                model.Items.Select(x => x.Id));

            var result = controller.FilterCapabilities(model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(FilterController.FilterCapabilities));
            actualResult.ControllerName.Should().Be(typeof(FilterController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "selectedIds", expected },
            });
        }
    }
}
