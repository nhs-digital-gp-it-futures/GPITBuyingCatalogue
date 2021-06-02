using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.MappingProfiles
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal class BrowserBasedProfileTests
    {
        private IMapper mapper;

        [OneTimeSetUp]
        public void SetUp()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x =>
                    x.GetService(typeof(IMemberValueResolver<object, object, string, bool?>)))
                .Returns(new Mock<IMemberValueResolver<object, object, string, bool?>>().Object);
            
            mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BrowserBasedProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper(serviceProvider.Object.GetService);
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            mapper = null;
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var browser in ProfileDefaults.SupportedBrowsers)
            {
                browser.Checked = false;
            }
        }

        [Test]
        public static void Mappings_Configuration_Valid()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BrowserBasedProfile>();
                cfg.AddProfile<OrganisationProfile>();
            });

            mapperConfiguration.AssertConfigurationIsValid();
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToAdditionalInformationModel_ResultAsExpected(
            CatalogueItem catalogueItem)
        {
            var clientApplication =
                JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);

            var actual = mapper.Map<CatalogueItem, AdditionalInformationModel>(catalogueItem);

            actual.AdditionalInformation.Should().Be(clientApplication.AdditionalInformation);
            actual.BackLink.Should()
                .Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}/section/browser-based");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToAdditionalInformationModel_NoClientApplication_AdditionalInfoNotSet(
            CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.ClientApplication = null;

            var actual = mapper.Map<CatalogueItem, AdditionalInformationModel>(catalogueItem);

            actual.AdditionalInformation.Should().BeNull();
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToConnectivityAndResolutionModel_ResultAsExpected(
            CatalogueItem catalogueItem)
        {
            var actual = mapper.Map<CatalogueItem, ConnectivityAndResolutionModel>(catalogueItem);

            actual.BackLink.Should()
                .Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}/section/browser-based");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ConnectionSpeeds.Should().BeEquivalentTo(new List<SelectListItem>
            {
                new() {Text = "0.5Mbps", Value = "0.5Mbps"},
                new() {Text = "1Mbps", Value = "1Mbps"},
                new() {Text = "1.5Mbps", Value = "1.5Mbps"},
                new() {Text = "2Mbps", Value = "2Mbps"},
                new() {Text = "3Mbps", Value = "3Mbps"},
                new() {Text = "5Mbps", Value = "5Mbps"},
                new() {Text = "8Mbps", Value = "8Mbps"},
                new() {Text = "10Mbps", Value = "10Mbps"},
                new() {Text = "15Mbps", Value = "15Mbps"},
                new() {Text = "20Mbps", Value = "20Mbps"},
                new() {Text = "30Mbps", Value = "30Mbps"},
                new() {Text = "Higher than 30Mbps", Value = "Higher than 30Mbps"}
            });
            actual.ScreenResolutions.Should().BeEquivalentTo(new List<SelectListItem>
            {
                new() {Text = "16:9 - 640 x 360", Value = "16:9 - 640 x 360"},
                new() {Text = "4:3 - 800 x 600", Value = "4:3 - 800 x 600"},
                new() {Text = "4:3 - 1024 x 768", Value = "4:3 - 1024 x 768"},
                new() {Text = "16:9 - 1280 x 720", Value = "16:9 - 1280 x 720"},
                new() {Text = "16:10 - 1280 x 800", Value = "16:10 - 1280 x 800"},
                new() {Text = "5:4 - 1280 x 1024", Value = "5:4 - 1280 x 1024"},
                new() {Text = "16:9 - 1360 x 768", Value = "16:9 - 1360 x 768"},
                new() {Text = "16:9 - 1366 x 768", Value = "16:9 - 1366 x 768"},
                new() {Text = "16:10 - 1440 x 900", Value = "16:10 - 1440 x 900"},
                new() {Text = "16:9 - 1536 x 864", Value = "16:9 - 1536 x 864"},
                new() {Text = "16:9 - 1600 x 900", Value = "16:9 - 1600 x 900"},
                new() {Text = "16:10 - 1680 x 1050", Value = "16:10 - 1680 x 1050"},
                new() {Text = "16:9 - 1920 x 1080", Value = "16:9 - 1920 x 1080"},
                new() {Text = "16:10 - 1920 x 1200", Value = "16:10 - 1920 x 1200"},
                new() {Text = "16:9 - 2048 x 1152", Value = "16:9 - 2048 x 1152"},
                new() {Text = "21:9 - 2560 x 1080", Value = "21:9 - 2560 x 1080"},
                new() {Text = "16:9 - 2560 x 1440", Value = "16:9 - 2560 x 1440"},
                new() {Text = "21:9 - 3440 x 1440", Value = "21:9 - 3440 x 1440"},
                new() {Text = "16:9 - 3840 x 2160", Value = "16:9 - 3840 x 2160"}
            });
            actual.SelectedConnectionSpeed.Should().Be(actual.ClientApplication.MinimumConnectionSpeed);
            actual.SelectedScreenResolution.Should().Be(actual.ClientApplication.MinimumDesktopResolution);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToHardwareRequirementsModel_ResultAsExpected(
            CatalogueItem catalogueItem)
        {
            var clientApplication =
                JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);

            var actual = mapper.Map<CatalogueItem, HardwareRequirementsModel>(catalogueItem);

            actual.BackLink.Should()
                .Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}/section/browser-based");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.Description.Should().Be(clientApplication.HardwareRequirements);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToHardwareRequirementsModel_NoClientApplication_DescriptionNotSet(
            CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.ClientApplication = null;

            var actual = mapper.Map<CatalogueItem, HardwareRequirementsModel>(catalogueItem);

            actual.Description.Should().BeNull();
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToMobileFirstApproachModel_ResultAsExpected(
            CatalogueItem catalogueItem)
        {
            var clientApplication =
                JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);

            var actual = mapper.Map<CatalogueItem, MobileFirstApproachModel>(catalogueItem);

            actual.BackLink.Should()
                .Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}/section/browser-based");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToMobileFirstApproachModel_MobileFirstDesignHasValue_MobileFirstApproachSet(
            CatalogueItem catalogueItem, ClientApplication clientApplication)
        {
            clientApplication.MobileFirstDesign = DateTime.Now.Ticks % 2 == 0;
            var expected = clientApplication.MobileFirstDesign.ToYesNo();
            catalogueItem.Solution.ClientApplication = JsonConvert.SerializeObject(clientApplication);

            var actual = mapper.Map<CatalogueItem, MobileFirstApproachModel>(catalogueItem);

            actual.MobileFirstApproach.Should().Be(expected);
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToMobileFirstApproachModel_MobileFirstDesignHasNoValue_MobileFirstApproachNotSet(
            CatalogueItem catalogueItem, ClientApplication clientApplication)
        {
            clientApplication.MobileFirstDesign = null;
            catalogueItem.Solution.ClientApplication = JsonConvert.SerializeObject(clientApplication);

            var actual = mapper.Map<CatalogueItem, MobileFirstApproachModel>(catalogueItem);

            actual.MobileFirstApproach.Should().BeNull();
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToPlugInsOrExtensionsModel_ResultAsExpected(
            CatalogueItem catalogueItem)
        {
            var clientApplication =
                JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);

            var actual = mapper.Map<CatalogueItem, PlugInsOrExtensionsModel>(catalogueItem);

            actual.AdditionalInformation.Should().Be(clientApplication.Plugins.AdditionalInformation);
            actual.BackLink.Should()
                .Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}/section/browser-based");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.PlugInsRequired.Should().Be(clientApplication.Plugins.Required.ToYesNo());
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToPlugInsOrExtensionsModel_PluginsRequiredIsNull_PluginsRequiredAndAdditionalInfoNotSet(
            CatalogueItem catalogueItem, ClientApplication clientApplication)
        {
            clientApplication.Plugins.Required = null;
            catalogueItem.Solution.ClientApplication = JsonConvert.SerializeObject(clientApplication);

            var actual = mapper.Map<CatalogueItem, PlugInsOrExtensionsModel>(catalogueItem);

            actual.AdditionalInformation.Should().BeNull();
            actual.PlugInsRequired.Should().BeNull();
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToSupportedBrowsersModel_ResultAsExpected(
            CatalogueItem catalogueItem)
        {
            var clientApplication =
                JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);

            var actual = mapper.Map<CatalogueItem, SupportedBrowsersModel>(catalogueItem);

            actual.BackLink.Should()
                .Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}/section/browser-based");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.Browsers.Should().BeEquivalentTo(new SupportedBrowserModel[]
            {
                new() {BrowserName = "Google Chrome", Checked = true, },
                new() {BrowserName = "Microsoft Edge", Checked = false, },
                new() {BrowserName = "Mozilla Firefox", Checked = true, },
                new() {BrowserName = "Opera", Checked = true, },
                new() {BrowserName = "Safari", Checked = true, },
                new() {BrowserName = "Chromium", Checked = false, },
                new() {BrowserName = "Internet Explorer 11", Checked = true, },
                new() {BrowserName = "Internet Explorer 10", Checked = false, }
            });
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.MobileResponsive.Should().Be(clientApplication.MobileResponsive.ToYesNo());
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToSupportedBrowsersModel_NoClientApplication_DependentValuesNotSet(
            CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.ClientApplication = null;

            var actual = mapper.Map<CatalogueItem, SupportedBrowsersModel>(catalogueItem);
            
            actual.Browsers.ToList().ForEach(b => b.Checked.Should().BeFalse());
            actual.MobileResponsive.Should().BeNullOrEmpty();
        }

        [Test, CommonAutoData]
        public void Map_ConnectivityAndResolutionToClientApplication_ResultAsExpected(
            ConnectivityAndResolutionModel model,
            ClientApplication clientApplication)
        {
            var original = clientApplication.CopyObjectToNew();

            var actual = mapper.Map(model, clientApplication);

            actual.MinimumConnectionSpeed.Should().Be(model.SelectedConnectionSpeed);
            actual.MinimumDesktopResolution.Should().Be(model.SelectedScreenResolution);
            actual.ValidateAllPropertiesExcept(original, new[]
            {
                nameof(ClientApplication.MinimumConnectionSpeed),
                nameof(ClientApplication.MinimumDesktopResolution),
            });
        }

        [Test, AutoData]
        public void Map_PlugInsOrExtensionsModelToPlugins_ResultAsExpected(
            PlugInsOrExtensionsModel model)
        {
            var expected = DateTime.Now.Ticks % 2 == 0;
            var requiredResolver = new Mock<IMemberValueResolver<object, object, string, bool?>>();
            requiredResolver.Setup(x => x.Resolve(It.IsAny<object>(), It.IsAny<object>(),
                    model.PlugInsRequired, It.IsAny<bool?>(), It.IsAny<ResolutionContext>()))
                .Returns(expected);
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x =>
                    x.GetService(typeof(IMemberValueResolver<object, object, string, bool?>)))
                .Returns(requiredResolver.Object);
            mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BrowserBasedProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper(serviceProvider.Object.GetService);

            var actual = mapper.Map<PlugInsOrExtensionsModel, Plugins>(model);

            actual.AdditionalInformation.Should().Be(model.AdditionalInformation);
            actual.Required.Should().Be(expected);
        }

        [Test, CommonAutoData]
        public void Map_SupportedBrowsersModelToClientApplication_ResultAsExpected(
            SupportedBrowsersModel model,
            ClientApplication clientApplication)
        {
            var original = clientApplication.CopyObjectToNew();

            var actual = mapper.Map(model, clientApplication);

            actual.BrowsersSupported.Should()
                .BeEquivalentTo(model.Browsers.Where(x => x.Checked).Select(x => x.BrowserName));
            actual.ValidateAllPropertiesExcept(original, new[]
            {
                nameof(ClientApplication.BrowsersSupported),
                nameof(ClientApplication.MobileResponsive),
            });
        }

        [Test, AutoData]
        public void Map_SupportedBrowsersModelToClientApplication_SetsMobileResponsiveFromResolver(
            SupportedBrowsersModel model)
        {
            var clientApplication = new ClientApplication();
            var expected = DateTime.Now.Ticks % 2 == 0;
            var mobileResponsiveResolver = new Mock<IMemberValueResolver<object, object, string, bool?>>();
            mobileResponsiveResolver.Setup(x => x.Resolve(It.IsAny<object>(), It.IsAny<object>(),
                    model.MobileResponsive, It.IsAny<bool?>(), It.IsAny<ResolutionContext>()))
                .Returns(expected);
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x =>
                    x.GetService(typeof(IMemberValueResolver<object, object, string, bool?>)))
                .Returns(mobileResponsiveResolver.Object);
            var autoMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BrowserBasedProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper(serviceProvider.Object.GetService);
            
            autoMapper.Map(model, clientApplication);

            mobileResponsiveResolver.Verify(x => x.Resolve(It.IsAny<object>(), It.IsAny<object>(),
                model.MobileResponsive, It.IsAny<bool?>(), It.IsAny<ResolutionContext>()));
            clientApplication.MobileResponsive.Should().Be(expected);
        }
    }
}
