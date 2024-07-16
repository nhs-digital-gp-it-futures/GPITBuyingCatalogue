using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class SolutionSupplierDetailsModelTests
    {
        [Fact]
        public static void Class_Inherits_SolutionDisplayBaseModel()
        {
            typeof(SolutionSupplierDetailsModel)
                .Should()
                .BeAssignableTo<SolutionDisplayBaseModel>();
        }

        [Theory]
        [MockAutoData]
        public static void Constructor_PopulatesAllProperties(Solution solution)
        {
            var catalogueItem = solution.CatalogueItem;
            var model = new SolutionSupplierDetailsModel(catalogueItem, new CatalogueItemContentStatus());

            model.Name.Should().Be(catalogueItem.Supplier.Name);
            model.Url.Should().Be(catalogueItem.Supplier.SupplierUrl);
            model.Summary.Should().Be(catalogueItem.Supplier.Summary);
            model.Contacts.Any().Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static void HasContacts_ValidContacts_ReturnsTrue(SolutionSupplierDetailsModel model)
        {
            model.Contacts.Any(c => c != null).Should().BeTrue();

            model.HasContacts().Should().BeTrue();
        }

        [Fact]
        public static void HasContacts_NullContacts_ReturnsFalse()
        {
            var model = new SolutionSupplierDetailsModel
            {
                Contacts = new List<SupplierContactViewModel> { null, null, },
            };

            model.HasContacts().Should().BeFalse();
        }

        [Fact]
        public static void HasContacts_EmptyContacts_ReturnsFalse()
        {
            var model = new SolutionSupplierDetailsModel
            {
                Contacts = new List<SupplierContactViewModel>(),
            };

            model.HasContacts().Should().BeFalse();
        }

        [Fact]
        public static void HasContacts_NullCollection_ReturnsFalse()
        {
            var model = new SolutionSupplierDetailsModel
            {
                Contacts = null,
            };

            model.HasContacts().Should().BeFalse();
        }
    }
}
