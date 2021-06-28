using System;
using AutoMapper;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.MappingProfiles
{
    public sealed class HostingTypeProfileTests : IDisposable
    {
        private IMapper mapper;

        public HostingTypeProfileTests()
        {
            mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<HostingTypeProfile>();
                cfg.AddProfile<OrganisationProfile>();
            }).CreateMapper();
        }

        public void Dispose()
        {
            mapper = null;
        }

        [Fact]
        public static void Mappings_Configuration_Valid()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<OrganisationProfile>();
                cfg.AddProfile<HostingTypeProfile>();
            });

            mapperConfiguration.AssertConfigurationIsValid();
        }

        [Fact]
        public void Map_CatalogueItemToHybridModel_ResultAsExpected()
        {
            var catalogueItem = Fakers.CatalogueItem.Generate();
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

        [Fact]
        public void Map_CatalogueItemToOnPremiseModel_ResultAsExpected()
        {
            var catalogueItem = Fakers.CatalogueItem.Generate();
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

        [Fact]
        public void Map_CatalogueItemToPrivateCloudModel_ResultAsExpected()
        {
            var catalogueItem = Fakers.CatalogueItem.Generate();
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

        [Fact]
        public void Map_CatalogueItemToPublicCloudModel_ResultAsExpected()
        {
            var catalogueItem = Fakers.CatalogueItem.Generate();
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

        [Fact]
        public void Map_CatalogueItemToSolutionStatusModel_MapsFromConverter()
        {
            var catalogueItem = Fakers.CatalogueItem.Generate();
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
