using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class ClientApplicationTypesModelTests
    {
        [Fact]
        public static void ApplicationTypes_UIHintAttribute_ExpectedHint()
        {
            typeof(ClientApplicationTypesModel)
                .GetProperty(nameof(ClientApplicationTypesModel.ApplicationTypes))
                .GetCustomAttribute<UIHintAttribute>()
                .UIHint.Should()
                .Be("DescriptionList");
        }

        [Fact]
        public static void BrowserBasedApplication_UIHintAttribute_ExpectedHint()
        {
            typeof(ClientApplicationTypesModel)
                .GetProperty(nameof(ClientApplicationTypesModel.BrowserBasedApplication))
                .GetCustomAttribute<UIHintAttribute>()
                .UIHint.Should()
                .Be("DescriptionList");
        }

        [Fact]
        public static void NativeMobileApplication_UIHintAttribute_ExpectedHint()
        {
            typeof(ClientApplicationTypesModel)
                .GetProperty(nameof(ClientApplicationTypesModel.NativeMobileApplication))
                .GetCustomAttribute<UIHintAttribute>()
                .UIHint.Should()
                .Be("DescriptionList");
        }

        [Fact]
        public static void NativeDesktopApplication_UIHintAttribute_ExpectedHint()
        {
            typeof(ClientApplicationTypesModel)
                .GetProperty(nameof(ClientApplicationTypesModel.NativeDesktopApplication))
                .GetCustomAttribute<UIHintAttribute>()
                .UIHint.Should()
                .Be("DescriptionList");
        }

        [Theory]
        [CommonInlineAutoData("some-value")]
        [CommonInlineAutoData("some-VALUE")]
        public static void HasApplicationType_ValueValid_ReturnsYes(
            string valid,
            [Frozen] CatalogueItem catalogueItem,
            [Frozen] Solution solution,
            [Frozen] ClientApplication clientApplication,
            ClientApplicationTypesModel model)
        {
            // CatalogueItem and Solution must be frozen so that a catalogue item instance with solution is passed
            // to the ClientApplicationTypesModel constructor
            _ = catalogueItem;
            _ = solution;

            clientApplication.ClientApplicationTypes = new HashSet<string> { valid };

            var actual = model.HasApplicationType("SOME-value");

            actual.Should().Be("Yes");
        }

        [Theory]
        [CommonAutoData]
        public static void HasApplicationType_ValueNotValid_ReturnsNo(
            [Frozen] CatalogueItem catalogueItem,
            [Frozen] Solution solution,
            [Frozen] ClientApplication clientApplication,
            ClientApplicationTypesModel model)
        {
            // CatalogueItem and Solution must be frozen so that a catalogue item instance with solution is passed
            // to the ClientApplicationTypesModel constructor
            _ = catalogueItem;
            _ = solution;

            clientApplication.ClientApplicationTypes = new HashSet<string> { "valid" };

            var actual = model.HasApplicationType("SOME-value");

            actual.Should().Be("No");
        }
    }
}
