using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.NUnit3;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile;
using NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.MappingProfiles
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class NativeMobileProfileTests
    {
        [Test]
        public static void Mappings_Configuration_Valid()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NativeMobileProfile>();
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
                cfg.AddProfile<NativeMobileProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
            
            var actual = mapper.Map<CatalogueItem, AdditionalInformationModel>(catalogueItem);

            actual.AdditionalInformation.Should().Be(clientApplication.NativeMobileAdditionalInformation);
            actual.BackLink.Should().Be(GetBackLink(catalogueItem));
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
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
                cfg.AddProfile<NativeMobileProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
            
            var actual = mapper.Map<CatalogueItem, ConnectivityModel>(catalogueItem);

            actual.BackLink.Should().Be(GetBackLink(catalogueItem));
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
            actual.ConnectionTypes.Should().BeEquivalentTo(new ConnectionTypeModel[]
            {
                new() {ConnectionType = "GPRS"},
                new() {ConnectionType = "3G"},
                new() {ConnectionType = "LTE"},
                new() {ConnectionType = "4G"},
                new() {ConnectionType = "5G"},
                new() {ConnectionType = "Bluetooth"},
                new() {ConnectionType = "Wifi"}
            });
            actual.Description.Should().Be(clientApplication.MobileConnectionDetails.Description);
            actual.SelectedConnectionSpeed.Should()
                .Be(clientApplication.MobileConnectionDetails.MinimumConnectionSpeed);
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
                cfg.AddProfile<NativeMobileProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
            
            var actual = mapper.Map<CatalogueItem, HardwareRequirementsModel>(catalogueItem);

            actual.BackLink.Should().Be(GetBackLink(catalogueItem));
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.Description.Should().Be(clientApplication.NativeMobileHardwareRequirements);
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
                cfg.AddProfile<NativeMobileProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
            
            var actual = mapper.Map<CatalogueItem, MemoryAndStorageModel>(catalogueItem);
            
            actual.BackLink.Should().Be(GetBackLink(catalogueItem));
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.Description.Should().Be(clientApplication.MobileMemoryAndStorage.Description);
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
            actual.SelectedMemorySize.Should()
                .Be(clientApplication.MobileMemoryAndStorage.MinimumMemoryRequirement);
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
                cfg.AddProfile<NativeMobileProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
            
            var actual = mapper.Map<CatalogueItem, OperatingSystemsModel>(catalogueItem);

            actual.BackLink.Should().Be(GetBackLink(catalogueItem));
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.Description.Should().Be(clientApplication.MobileOperatingSystems.OperatingSystemsDescription);
            actual.OperatingSystems.Should().BeEquivalentTo(new SupportedOperatingSystemModel[]
            {
                new() {OperatingSystemName = "Apple IOS", Checked = true},
                new() {OperatingSystemName = "Android", Checked = true},
                new() {OperatingSystemName = "Other"}
            });
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
                cfg.AddProfile<NativeMobileProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
            
            var actual = mapper.Map<CatalogueItem, ThirdPartyModel>(catalogueItem);

            actual.BackLink.Should().Be(GetBackLink(catalogueItem));
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.DeviceCapabilities.Should().Be(clientApplication.MobileThirdParty.DeviceCapabilities);
            actual.ThirdPartyComponents.Should().Be(clientApplication.MobileThirdParty.ThirdPartyComponents);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }

        [Test, AutoData]
        public static void Map_ConnectivityModelToMobileConnectionDetails_ResultAsExpected(
            ConnectivityModel connectivityModel)
        {
            var fixture = new Fixture();
            connectivityModel.ConnectionTypes = Enumerable.Range(1, 7).ToList()
                .Select(x => new ConnectionTypeModel
                {
                    Checked = DateTime.Now.Ticks % 2 == 0, 
                    ConnectionType= fixture.Create<string>(),
                }).ToArray();
            var expected = connectivityModel.ConnectionTypes.Where(o => o.Checked)
                .Select(o => o.ConnectionType);
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NativeMobileProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
            
            var actual = mapper.Map<ConnectivityModel, MobileConnectionDetails>(connectivityModel);

            actual.ConnectionType.Should().BeEquivalentTo(expected);
            actual.Description.Should().Be(connectivityModel.Description);
            actual.MinimumConnectionSpeed.Should().Be(connectivityModel.SelectedConnectionSpeed);
        }
        
        [Test, AutoData]
        public static void Map_MemoryAndStorageModelToMobileMemoryAndStorage_ResultAsExpected(
            MemoryAndStorageModel memoryAndStorageModel)
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NativeMobileProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();

            var actual = mapper.Map<MemoryAndStorageModel, MobileMemoryAndStorage>(memoryAndStorageModel);

            actual.Description.Should().Be(memoryAndStorageModel.Description);
            actual.MinimumMemoryRequirement.Should().Be(memoryAndStorageModel.SelectedMemorySize);
        }

        [Test, AutoData]
        public static void Map_OperatingSystemsModelToMobileOperatingSystems_ResultAsExpected(
            OperatingSystemsModel operatingSystemsModel)
        {
            var fixture = new Fixture();
            operatingSystemsModel.OperatingSystems = Enumerable.Range(1, 7).ToList()
                .Select(x => new SupportedOperatingSystemModel
                {
                    Checked = DateTime.Now.Ticks % 2 == 0,
                    OperatingSystemName = fixture.Create<string>(),
                }).ToArray();
            var expected = operatingSystemsModel.OperatingSystems.Where(o => o.Checked)
                .Select(o => o.OperatingSystemName);
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NativeMobileProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();

            var actual = mapper.Map<OperatingSystemsModel, MobileOperatingSystems>(operatingSystemsModel);

            actual.OperatingSystems.Should().BeEquivalentTo(expected);
            actual.OperatingSystemsDescription.Should().BeEquivalentTo(operatingSystemsModel.Description);
        }

        [Test, AutoData]
        public static void Map_ThirdPartyModelToMobileThirdParty_ResultAsExpected(
            ThirdPartyModel thirdPartyModel)
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NativeMobileProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
            
            var actual = mapper.Map<ThirdPartyModel, MobileThirdParty>(thirdPartyModel);

            actual.DeviceCapabilities.Should().Be(thirdPartyModel.DeviceCapabilities);
            actual.ThirdPartyComponents.Should().Be(thirdPartyModel.ThirdPartyComponents);
        }
        
        private static string GetBackLink(CatalogueItem catalogueItem) =>
            $"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}/section/native-mobile";
    }
}