using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.AboutOrganisation
{
    public static class ContactDetailsModelTests
    {
        [Fact]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new ContactDetailsModel(null));
        }

        [Fact]
        public static void WithCatalogueItem_AndNoContacts_PropertiesCorrectlySet_AndIncomplete()
        {
            var catalogueItem = new CatalogueItem
            {
                Id = new CatalogueItemId(1, "123"),
                Solution = new Solution
                {
                    MarketingContacts = Array.Empty<MarketingContact>(),
                },
            };

            var model = new ContactDetailsModel(catalogueItem);

            Assert.Equal("/marketing/supplier/solution/1-123", model.BackLink);
            Assert.False(model.IsComplete);
            model.Contact1.Should().BeEquivalentTo(new MarketingContact());
            model.Contact2.Should().BeEquivalentTo(new MarketingContact());
        }

        [Fact]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new ContactDetailsModel();

            Assert.Equal("./", model.BackLink);
            Assert.False(model.IsComplete);
            model.Contact1.Should().BeEquivalentTo(new MarketingContact());
            model.Contact2.Should().BeEquivalentTo(new MarketingContact());
        }

        [Fact]
        public static void WithCatalogueItem_AndOneContact_PropertiesCorrectlySet_AndComplete()
        {
            var catalogueItem = new CatalogueItem
            {
                Id = new CatalogueItemId(1, "123"),
                Solution = new Solution
                {
                    MarketingContacts = new MarketingContact[] { new() { FirstName = "Fred" } },
                },
            };

            var model = new ContactDetailsModel(catalogueItem);

            Assert.Equal("/marketing/supplier/solution/1-123", model.BackLink);
            Assert.True(model.IsComplete);
            model.Contact1.Should().BeEquivalentTo(catalogueItem.Solution.MarketingContacts.Single());
            model.Contact2.Should().BeEquivalentTo(new MarketingContact());
        }

        [Fact]
        public static void WithCatalogueItem_AndTwoContacts_PropertiesCorrectlySet_AndComplete()
        {
            var catalogueItem = new CatalogueItem
            {
                Id = new CatalogueItemId(1, "123"),
                Solution = new Solution
                {
                    MarketingContacts = new MarketingContact[]
                    {
                        new() { FirstName = "Fred" },
                        new() { FirstName = "Bill" },
                    },
                },
            };

            var model = new ContactDetailsModel(catalogueItem);

            Assert.Equal("/marketing/supplier/solution/1-123", model.BackLink);
            Assert.True(model.IsComplete);
            model.Contact1.Should().BeEquivalentTo(catalogueItem.Solution.MarketingContacts.First());
            model.Contact2.Should().BeEquivalentTo(catalogueItem.Solution.MarketingContacts.Skip(1).Single());
        }
    }
}
