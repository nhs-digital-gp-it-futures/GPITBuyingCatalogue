using System;
using AutoMapper;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.MappingProfiles
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal class HostingTypeProfileTests
    {
        private IMapper mapper;

        [OneTimeSetUp]
        public void SetUp()
        {
            mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<HostingTypeProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            mapper = null;
        }

        [Test]
        public static void Mappings_Configuration_Valid()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<OrganisationProfile>();
                cfg.AddProfile<HostingTypeProfile>();
            });

            mapperConfiguration.AssertConfigurationIsValid();
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToHybridModel_ResultAsExpected(
            CatalogueItem catalogueItem)
        {
            var clientApplication =
                JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);
            var hosting = JsonConvert.DeserializeObject<Hosting>(catalogueItem.Solution.Hosting);

            var actual = mapper.Map<CatalogueItem, HybridModel>(catalogueItem);

            actual.BackLink.Should().Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.HybridHostingType.Should().BeEquivalentTo(hosting.HybridHostingType);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToOnPremiseModel_ResultAsExpected(
            CatalogueItem catalogueItem)
        {
            var clientApplication =
                JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);
            var hosting = JsonConvert.DeserializeObject<Hosting>(catalogueItem.Solution.Hosting);

            var actual = mapper.Map<CatalogueItem, OnPremiseModel>(catalogueItem);

            actual.BackLink.Should().Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.OnPremise.Should().BeEquivalentTo(hosting.OnPremise);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToPrivateCloudModel_ResultAsExpected(
            CatalogueItem catalogueItem)
        {
            var clientApplication =
                JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);
            var hosting = JsonConvert.DeserializeObject<Hosting>(catalogueItem.Solution.Hosting);

            var actual = mapper.Map<CatalogueItem, PrivateCloudModel>(catalogueItem);

            actual.BackLink.Should().Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.PrivateCloud.Should().BeEquivalentTo(hosting.PrivateCloud);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToPublicCloudModel_ResultAsExpected(
            CatalogueItem catalogueItem)
        {
            var clientApplication =
                JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);
            var hosting = JsonConvert.DeserializeObject<Hosting>(catalogueItem.Solution.Hosting);

            var actual = mapper.Map<CatalogueItem, PublicCloudModel>(catalogueItem);

            actual.BackLink.Should().Be($"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}");
            actual.BackLinkText.Should().Be("Return to all sections");
            actual.ClientApplication.Should().BeEquivalentTo(clientApplication);
            actual.PublicCloud.Should().BeEquivalentTo(hosting.PublicCloud);
            actual.SolutionId.Should().Be(catalogueItem.CatalogueItemId);
            actual.SupplierId.Should().Be(catalogueItem.Supplier.Id);
        }

        [Test, CommonAutoData]
        public void Map_CatalogueItemToSolutionStatusModel_MapsFromConverter(
            CatalogueItem catalogueItem)
        {
            var mockSolutionStatusModel = new Mock<SolutionStatusModel>().Object;
            var mockConverter = new Mock<ITypeConverter<CatalogueItem, SolutionStatusModel>>();
            mockConverter.Setup(c =>
                    c.Convert(catalogueItem, It.IsAny<SolutionStatusModel>(), It.IsAny<ResolutionContext>()))
                .Returns(mockSolutionStatusModel);
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x =>
                    x.GetService(typeof(ITypeConverter<CatalogueItem, SolutionStatusModel>)))
                .Returns(mockConverter.Object);
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<OrganisationProfile>();
                cfg.AddProfile<HostingTypeProfile>();
            }).CreateMapper(serviceProvider.Object.GetService);

            var actual = mapper.Map<CatalogueItem, SolutionStatusModel>(catalogueItem);

            mockConverter.Verify(c =>
                c.Convert(catalogueItem, It.IsAny<SolutionStatusModel>(), It.IsAny<ResolutionContext>()));
            actual.Should().Be(mockSolutionStatusModel);
        }
    }
}
