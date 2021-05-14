using System.Collections.Generic;
using AutoFixture.NUnit3;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop;
using NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.MappingProfiles
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class NativeDesktopProfileTests
    {
        [Test]
        public static void Mappings_Configuration_Valid()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NativeDesktopProfile>();
                cfg.AddProfile<OrganisationProfile>();
            });

            mapperConfiguration.AssertConfigurationIsValid();
        }
        
        [Test, CommonAutoData]
        public static void Map_CatalogueItemToAdditionalInformationModel_ResultAsExpected(
            CatalogueItem catalogueItem)
        {
            var clientApplication = 
                JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NativeDesktopProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
            
            var actual = mapper.Map<CatalogueItem, AdditionalInformationModel>(catalogueItem);

            actual.BackLink.Should()
                .Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}/section/native-desktop");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.AdditionalInformation.Should().Be(clientApplication.NativeDesktopAdditionalInformation);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }

        [Test, CommonAutoData]
        public static void Map_CatalogueItemToConnectivityModel_ResultAsExpected(
            CatalogueItem catalogueItem)
        {
            var clientApplication = 
                JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NativeDesktopProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
            
            var actual = mapper.Map<CatalogueItem, ConnectivityModel>(catalogueItem);

            actual.BackLink.Should()
                .Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}/section/native-desktop");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.ConnectionSpeeds.Should().BeEquivalentTo(new List<SelectListItem>
            {
                new() {Text = "Please select"},
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
            actual.SelectedConnectionSpeed.Should().Be(clientApplication.NativeDesktopMinimumConnectionSpeed);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }
        
        [Test, CommonAutoData]
        public static void Map_CatalogueItemToHardwareRequirementsModel_ResultAsExpected(
            CatalogueItem catalogueItem)
        {
            var clientApplication = 
                JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NativeDesktopProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
            
            var actual = mapper.Map<CatalogueItem, HardwareRequirementsModel>(catalogueItem);

            actual.BackLink.Should()
                .Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}/section/native-desktop");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.Description.Should().Be(clientApplication.NativeDesktopHardwareRequirements);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }

        [Test, CommonAutoData]
        public static void Map_CatalogueItemToMemoryAndStorageModel_ResultAsExpected(
            CatalogueItem catalogueItem)
        {
            var clientApplication = 
                JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NativeDesktopProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
            
            var actual = mapper.Map<CatalogueItem, MemoryAndStorageModel>(catalogueItem);
            
            actual.BackLink.Should()
                .Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}/section/native-desktop");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.MemorySizes.Should().BeEquivalentTo(new List<SelectListItem>
            {
                new() { Text = "Please select"},
                new() { Text = "256MB", Value = "256MB"},
                new() { Text = "512MB", Value = "512MB"},
                new() { Text = "1GB", Value = "1GB"},
                new() { Text = "2GB", Value = "2GB"},
                new() { Text = "4GB", Value = "4GB"},
                new() { Text = "8GB", Value = "8GB"},
                new() { Text = "16GB or higher", Value = "16GB or higher"}
            });
            actual.MinimumCpu.Should().Be(clientApplication.NativeDesktopMemoryAndStorage.MinimumCpu);
            actual.SelectedScreenResolution.Should()
                .Be(clientApplication.NativeDesktopMemoryAndStorage.RecommendedResolution);
            actual.SelectedMemorySize.Should()
                .Be(clientApplication.NativeDesktopMemoryAndStorage.MinimumMemoryRequirement);
            actual.ScreenResolutions.Should().BeEquivalentTo(new List<SelectListItem>
            {
                new() {Text = "Please select", Value = ""},
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
            actual.StorageDescription.Should()
                .Be(clientApplication.NativeDesktopMemoryAndStorage.StorageRequirementsDescription);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }

        [Test, CommonAutoData]
        public static void Map_CatalogueItemToOperatingSystemsModel_ResultAsExpected(
            CatalogueItem catalogueItem)
        {
            var clientApplication = 
                JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NativeDesktopProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
            
            var actual = mapper.Map<CatalogueItem, OperatingSystemsModel>(catalogueItem);

            actual.BackLink.Should()
                .Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}/section/native-desktop");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.OperatingSystemsDescription.Should().Be(clientApplication.NativeDesktopOperatingSystemsDescription);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }
        
        [Test, CommonAutoData]
        public static void Map_CatalogueItemToThirdPartyModel_ResultAsExpected(
            CatalogueItem catalogueItem)
        {
            var clientApplication = 
                JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NativeDesktopProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
            
            var actual = mapper.Map<CatalogueItem, ThirdPartyModel>(catalogueItem);

            actual.BackLink.Should()
                .Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}/section/native-desktop");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.DeviceCapabilities.Should().Be(clientApplication.NativeDesktopThirdParty.DeviceCapabilities);
            actual.ThirdPartyComponents.Should().Be(clientApplication.NativeDesktopThirdParty.ThirdPartyComponents);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }

        [Test, AutoData]
        public static void Map_MemoryAndStorageModelToNativeDesktopMemoryAndStorage_ResultAsExpected(
            MemoryAndStorageModel memoryAndStorageModel)
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NativeDesktopProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
            
            var actual = mapper.Map<MemoryAndStorageModel, NativeDesktopMemoryAndStorage>(memoryAndStorageModel);
            
            actual.MinimumMemoryRequirement.Should().Be(memoryAndStorageModel.SelectedMemorySize);
            actual.StorageRequirementsDescription.Should().Be(memoryAndStorageModel.StorageDescription);
            actual.MinimumCpu.Should().Be(memoryAndStorageModel.MinimumCpu);
            actual.RecommendedResolution.Should().Be(memoryAndStorageModel.SelectedScreenResolution);
        }
    }
}