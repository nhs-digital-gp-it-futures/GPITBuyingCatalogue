using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.MappingProfiles;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.MappingProfiles
{
    public sealed class CatalogueSolutionsProfileTests
    {
        private readonly IMapper mapper;
        private readonly MapperConfiguration mapperConfiguration;

        public CatalogueSolutionsProfileTests()
        {
            mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CatalogueSolutionsProfile>();
            });
            mapper = mapperConfiguration.CreateMapper();
        }
        
        [Fact]
        public void Mappings_Configuration_Valid()
        {
            mapperConfiguration.AssertConfigurationIsValid();
        }

        [Theory]
        [CommonAutoData]
        public void Map_CatalogueItemToCatalogueModel_ResultAsExpected(CatalogueItem catalogueItem)
        {
            var actual = mapper.Map<CatalogueItem, CatalogueModel>(catalogueItem);

            actual.Id.Should().Be(catalogueItem.CatalogueItemId.ToString());
            actual.Name.Should().Be(catalogueItem.Name);
            actual.LastUpdated.Should().Be(catalogueItem.Supplier.LastUpdated);
            actual.PublishedStatusId.Should().Be((int)catalogueItem.PublishedStatus);
            actual.Supplier.Should().Be(catalogueItem.Supplier.Name);
        }
        
        [Theory]
        [CommonAutoData]
        public void Map_CatalogueItemToCatalogueModel_SupplierIsNull_RelatedPropertiesToDefault(
            CatalogueItem catalogueItem)
        {
            catalogueItem.Supplier = null;
            
            var actual = mapper.Map<CatalogueItem, CatalogueModel>(catalogueItem);

            actual.LastUpdated.Should().Be(DateTime.MinValue);
            actual.Supplier.Should().Be(string.Empty);
        }

        [Theory]
        [InlineData(PublicationStatus.Draft, "Suspended")]
        [InlineData(PublicationStatus.Unpublished, "Unpublished")]
        [InlineData(PublicationStatus.Published, "Published")]
        [InlineData(PublicationStatus.Withdrawn, "Deleted")]
        public void Map_CatalogueItemToCatalogueModel_SetsPublishedStatusAsExpected(
            PublicationStatus status,
            string expected)
        {
            var catalogueItem = new CatalogueItem { PublishedStatus = status };
            
            var actual = mapper.Map<CatalogueItem, CatalogueModel>(catalogueItem);

            actual.PublishedStatus.Should().Be(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Map_CatalogueItemListToCatalogueSolutionsModel_ResultAsExpected(List<CatalogueItem> catalogueItems)
        {
            var actual = mapper.Map<IList<CatalogueItem>, CatalogueSolutionsModel>(catalogueItems);

            actual.AllPublicationStatuses.Should()
                .BeEquivalentTo(
                    new List<PublicationStatusModel>
                    {
                        new() { Id = 1, Display = "Suspended" },
                        new() { Id = 2, Display = PublicationStatus.Unpublished.ToString() },
                        new() { Id = 3, Display = PublicationStatus.Published.ToString() },
                        new() { Id = 4, Display = "Deleted" },
                    });
            actual.CatalogueItems.Count.Should().Be(catalogueItems.Count);
            actual.PublicationStatusId.Should().Be(0);
        }
    }
}
