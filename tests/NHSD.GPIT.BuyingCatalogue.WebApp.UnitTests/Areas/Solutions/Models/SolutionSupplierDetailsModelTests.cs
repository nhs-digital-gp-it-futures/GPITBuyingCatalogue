using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class SolutionSupplierDetailsModelTests
    {
        [Test]
        public static void Class_Inherits_SolutionDisplayBaseModel()
        {
            typeof(SolutionSupplierDetailsModel)
                .Should()
                .BeAssignableTo<SolutionDisplayBaseModel>();
        }

        [Test]
        [CommonAutoData]
        public static void HasContacts_ValidContacts_ReturnsTrue(SolutionSupplierDetailsModel model)
        {
            model.Contacts.Any(x => x != null).Should().BeTrue();

            model.HasContacts().Should().BeTrue();
        }

        [Test]
        public static void HasContacts_NullContacts_ReturnsFalse()
        {
            var model = new SolutionSupplierDetailsModel
            {
                Contacts = new List<SupplierContactViewModel> { null, null, },
            };

            model.HasContacts().Should().BeFalse();
        }

        [Test]
        public static void HasContacts_EmptyContacts_ReturnsFalse()
        {
            var model = new SolutionSupplierDetailsModel
            {
                Contacts = new List<SupplierContactViewModel>(),
            };

            model.HasContacts().Should().BeFalse();
        }

        [Test]
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
