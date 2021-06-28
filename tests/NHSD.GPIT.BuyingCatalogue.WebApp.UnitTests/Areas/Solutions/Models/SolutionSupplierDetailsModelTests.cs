using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
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
        [CommonAutoData]
        public static void HasContacts_ValidContacts_ReturnsTrue(SolutionSupplierDetailsModel model)
        {
            model.Contacts.Any(x => x != null).Should().BeTrue();

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
