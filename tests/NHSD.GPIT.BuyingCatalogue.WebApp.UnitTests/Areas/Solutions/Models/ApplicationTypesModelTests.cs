using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class ApplicationTypesModelTests
    {
        [Fact]
        public static void ApplicationTypesModel_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new ApplicationTypesModel(null, new CatalogueItemContentStatus()));
            actual.ParamName.Should().Be("catalogueItem");
        }

        [Fact]
        public static void ApplicationTypesModel_NullSolution_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new ApplicationTypesModel(new CatalogueItem() { Solution = null }, new CatalogueItemContentStatus()));
            actual.ParamName.Should().Be("catalogueItem");
        }

        [Fact]
        public static void ApplicationTypes_UIHintAttribute_ExpectedHint()
        {
            typeof(ApplicationTypesModel)
                .GetProperty(nameof(ApplicationTypesModel.ApplicationTypes))
                .GetCustomAttribute<UIHintAttribute>()
                .UIHint.Should()
                .Be("DescriptionList");
        }

        [Fact]
        public static void BrowserBasedApplication_UIHintAttribute_ExpectedHint()
        {
            typeof(ApplicationTypesModel)
                .GetProperty(nameof(ApplicationTypesModel.BrowserBasedApplication))
                .GetCustomAttribute<UIHintAttribute>()
                .UIHint.Should()
                .Be("DescriptionList");
        }

        [Fact]
        public static void NativeMobileApplication_UIHintAttribute_ExpectedHint()
        {
            typeof(ApplicationTypesModel)
                .GetProperty(nameof(ApplicationTypesModel.NativeMobileApplication))
                .GetCustomAttribute<UIHintAttribute>()
                .UIHint.Should()
                .Be("DescriptionList");
        }

        [Fact]
        public static void NativeDesktopApplication_UIHintAttribute_ExpectedHint()
        {
            typeof(ApplicationTypesModel)
                .GetProperty(nameof(ApplicationTypesModel.NativeDesktopApplication))
                .GetCustomAttribute<UIHintAttribute>()
                .UIHint.Should()
                .Be("DescriptionList");
        }

        [Theory]
        [CommonAutoData]
        public static void HasApplicationType_ValueValid_ReturnsYes(
            ApplicationType clientApplicationType,
            [Frozen] CatalogueItem catalogueItem,
            [Frozen] Solution solution,
            [Frozen] ClientApplication clientApplication,
            ApplicationTypesModel model)
        {
            // CatalogueItem and Solution must be frozen so that a catalogue item instance with solution is passed
            // to the ClientApplicationTypesModel constructor
            _ = catalogueItem;
            _ = solution;

            clientApplication.ClientApplicationTypes = new HashSet<string> { clientApplicationType.EnumMemberName() };

            var actual = model.HasApplicationType(clientApplicationType);

            actual.Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void HasApplicationType_ValueNotValid_ReturnsNo(
            [Frozen] CatalogueItem catalogueItem,
            [Frozen] Solution solution,
            [Frozen] ClientApplication clientApplication,
            ApplicationTypesModel model)
        {
            // CatalogueItem and Solution must be frozen so that a catalogue item instance with solution is passed
            // to the ClientApplicationTypesModel constructor
            _ = catalogueItem;
            _ = solution;

            clientApplication.ClientApplicationTypes =
                new HashSet<string> { ApplicationType.Desktop.EnumMemberName() };

            var actual = model.HasApplicationType(ApplicationType.MobileTablet);

            actual.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void Construct_BrowserBasedApplication_CreatesDescriptionListViewModel(
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var clientApplication = new ClientApplication
            {
                ClientApplicationTypes = new() { ApplicationType.BrowserBased.EnumMemberName() },
                BrowsersSupported = new() { new() { BrowserName = "Chrome" } },
                MobileResponsive = true,
                Plugins = new() { Required = true, AdditionalInformation = "AdditionalInformation" },
                MinimumConnectionSpeed = "1Gbit",
                MinimumDesktopResolution = "3440x1440",
                HardwareRequirements = "RTX 4090, i9 13900KF",
                AdditionalInformation = "Test",
            };

            solution.ApplicationType = JsonSerializer.Serialize(clientApplication);

            var model = new ApplicationTypesModel(solution.CatalogueItem, contentStatus);

            model.BrowserBasedApplication.Should().NotBeNull();
            model.BrowserBasedApplication.Items.Should().HaveCount(8);
            model.BrowserBasedApplication.Items.Should()
                .ContainKeys(
                    "Supported browser types",
                    "Mobile responsive",
                    "Plug-ins or extensions required",
                    "Additional information about plug-ins or extensions",
                    "Minimum connection speed",
                    "Screen resolution and aspect ratio",
                    "Hardware requirements",
                    "Additional information");
        }

        [Theory]
        [CommonAutoData]
        public static void Construct_NativeDesktopApplication_CreatesDescriptionListViewModel(
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var clientApplication = new ClientApplication
            {
                ClientApplicationTypes = new() { ApplicationType.Desktop.EnumMemberName() },
                NativeDesktopOperatingSystemsDescription = "Windows 95",
                NativeDesktopMinimumConnectionSpeed = "10Gbps",
                NativeDesktopMemoryAndStorage = new()
                {
                    MinimumCpu = "i9 13900KF",
                    MinimumMemoryRequirement = "64GB DDR5 5600Mhz",
                    RecommendedResolution = "3440x1440",
                    StorageRequirementsDescription = "2TB NVMe",
                },
                NativeDesktopThirdParty =
                    new() { ThirdPartyComponents = "Component", DeviceCapabilities = "Device Capability", },
                NativeDesktopHardwareRequirements = "Water cooling",
                NativeDesktopAdditionalInformation = "Monitor",
            };

            solution.ApplicationType = JsonSerializer.Serialize(clientApplication);

            var model = new ApplicationTypesModel(solution.CatalogueItem, contentStatus);

            model.NativeDesktopApplication.Should().NotBeNull();
            model.NativeDesktopApplication.Items.Should().HaveCount(10);
            model.NativeDesktopApplication.Items.Should()
                .ContainKeys(
                    "Description of supported operating systems",
                    "Minimum connection speed",
                    "Screen resolution and aspect ratio",
                    "Memory size",
                    "Storage space",
                    "Processing power",
                    "Third-party components",
                    "Device capabilities",
                    "Hardware requirements",
                    "Additional information");
        }

        [Theory]
        [CommonAutoData]
        public static void Construct_NativeMobileApplication_CreatesDescriptionListViewModel(
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var clientApplication = new ClientApplication
            {
                ClientApplicationTypes = new() { ApplicationType.MobileTablet.EnumMemberName() },
                MobileOperatingSystems =
                    new() { OperatingSystems = new() { "MS-DOS" }, OperatingSystemsDescription = "256MB DDR", },
                MobileConnectionDetails =
                    new()
                    {
                        MinimumConnectionSpeed = "500Mbit",
                        ConnectionType = new() { "10BASE5" },
                        Description = "Description",
                    },
                MobileMemoryAndStorage = new() { Description = "Description", MinimumMemoryRequirement = "256GB", },
                MobileThirdParty = new() { DeviceCapabilities = "Capabilities", ThirdPartyComponents = "Components", },
                NativeMobileHardwareRequirements = "Requirements",
                NativeMobileAdditionalInformation = "Information",
            };

            solution.ApplicationType = JsonSerializer.Serialize(clientApplication);

            var model = new ApplicationTypesModel(solution.CatalogueItem, contentStatus);

            model.NativeMobileApplication.Should().NotBeNull();
            model.NativeMobileApplication.Items.Should().HaveCount(11);
            model.NativeMobileApplication.Items.Should()
                .ContainKeys(
                    "Supported operating systems",
                    "Description of supported operating systems",
                    "Minimum connection speed",
                    "Connection types supported",
                    "Connection requirements",
                    "Memory size",
                    "Storage space",
                    "Third-party components",
                    "Device capabilities",
                    "Hardware requirements",
                    "Additional information");
        }

        [Theory]
        [CommonAutoData]
        public static void Construct_AllApplicationTypes(
            Solution solution,
            CatalogueItemContentStatus contentStatus)
        {
            var clientApplication = new ClientApplication
            {
                ClientApplicationTypes = new()
                {
                    ApplicationType.Desktop.EnumMemberName(),
                    ApplicationType.BrowserBased.EnumMemberName(),
                    ApplicationType.MobileTablet.EnumMemberName(),
                },
            };

            solution.ApplicationType = JsonSerializer.Serialize(clientApplication);

            var model = new ApplicationTypesModel(solution.CatalogueItem, contentStatus);

            model.ApplicationTypes.Should().NotBeNull();
            model.ApplicationTypes.Items.Should()
                .ContainKeys(
                    "Browser-based application",
                    "Desktop application",
                    "Mobile or tablet application");
            model.ApplicationTypes.Items.Values.Should().OnlyContain(x => x.Text == "Yes");
        }
    }
}
