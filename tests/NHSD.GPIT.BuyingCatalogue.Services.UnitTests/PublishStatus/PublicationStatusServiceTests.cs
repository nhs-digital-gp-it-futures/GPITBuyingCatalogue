using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.PublishStatus;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.PublishStatus
{
    public static class PublicationStatusServiceTests
    {
        [Theory]
        [InMemoryDbAutoData]
        public static async void SetPublicationStatus_SameStatus_DoesNotUpdatePublicationStatus(
            CatalogueItem catalogueItem,
            [Frozen] BuyingCatalogueDbContext context,
            PublicationStatusService service)
        {
            var expectedPublishedStatus = catalogueItem.PublishedStatus;

            context.CatalogueItems.Add(catalogueItem);
            context.SaveChanges();

            await service.SetPublicationStatus(catalogueItem.Id, catalogueItem.PublishedStatus);

            catalogueItem.PublishedStatus.Should().Be(expectedPublishedStatus);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async void SetPublicationStatus_InRemediationStatus_DoesNotSetPublishedDate(
            CatalogueItem catalogueItem,
            [Frozen] BuyingCatalogueDbContext context,
            PublicationStatusService service)
        {
            catalogueItem.LastPublished = null;
            catalogueItem.PublishedStatus = PublicationStatus.Published;

            context.CatalogueItems.Add(catalogueItem);
            context.SaveChanges();

            await service.SetPublicationStatus(catalogueItem.Id, PublicationStatus.InRemediation);

            catalogueItem.PublishedStatus.Should().Be(PublicationStatus.InRemediation);
            catalogueItem.LastPublished.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async void SetPublicationStatus_DraftToPublished_SetsPublishedDate(
            CatalogueItem catalogueItem,
            [Frozen] BuyingCatalogueDbContext context,
            PublicationStatusService service)
        {
            catalogueItem.LastPublished = null;
            catalogueItem.PublishedStatus = PublicationStatus.Draft;

            context.CatalogueItems.Add(catalogueItem);
            context.SaveChanges();

            await service.SetPublicationStatus(catalogueItem.Id, PublicationStatus.Published);

            catalogueItem.PublishedStatus.Should().Be(PublicationStatus.Published);
            catalogueItem.LastPublished.Should().NotBeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async void SetPublicationStatus_PublishedToPublished_DoesNotSetPublishedDate(
            CatalogueItem catalogueItem,
            [Frozen] BuyingCatalogueDbContext context,
            PublicationStatusService service)
        {
            catalogueItem.LastPublished = null;
            catalogueItem.PublishedStatus = PublicationStatus.Published;

            context.CatalogueItems.Add(catalogueItem);
            context.SaveChanges();

            await service.SetPublicationStatus(catalogueItem.Id, PublicationStatus.Published);

            catalogueItem.PublishedStatus.Should().Be(PublicationStatus.Published);
            catalogueItem.LastPublished.Should().BeNull();
        }
    }
}
