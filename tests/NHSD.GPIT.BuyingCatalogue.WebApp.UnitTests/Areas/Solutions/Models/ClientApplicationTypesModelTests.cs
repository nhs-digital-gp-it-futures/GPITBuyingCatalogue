using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
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
        [InlineData("some-value")]
        [InlineData("some-VALUE")]
        public static void HasApplicationType_ValueValid_ReturnsYes(string valid)
        {
            var model = new ClientApplicationTypesModel
            {
                ClientApplication = new ClientApplication
                {
                    ClientApplicationTypes = new HashSet<string> { valid },
                }
            };

            var actual = model.HasApplicationType("SOME-value");

            actual.Should().Be("Yes");
        }

        [Fact]
        public static void HasApplicationType_ValueNotValid_ReturnsNo()
        {
            var model = new ClientApplicationTypesModel
            {
                ClientApplication = new ClientApplication
                {
                    ClientApplicationTypes = new HashSet<string> { "valid" },
                }
            };

            var actual = model.HasApplicationType("SOME-value");

            actual.Should().Be("No");
        }
    }
}
