using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.SupplierModels
{
    public static class EditContactModelTests
    {
        [Fact]
        public static void Constructor_NullSupplier_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new EditContactModel(null));

            actual.ParamName.Should().Be("supplier");
        }

        [Fact]
        public static void ConstructorOverload_NullContact_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new EditContactModel(null, new Supplier(), new List<CatalogueItem>()));

            actual.ParamName.Should().Be("contact");
        }

        [Fact]
        public static void ConstructorOverload_NullSupplier_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new EditContactModel(new SupplierContact(), null, new List<CatalogueItem>()));

            actual.ParamName.Should().Be("supplier");
        }

        [Fact]
        public static void ConstructorOverload_NullReferencingSolutions_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new EditContactModel(new SupplierContact(), new Supplier(), null));
            actual.ParamName.Should().Be("solutionsReferencing");
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            Supplier supplier)
        {
            var actual = new EditContactModel(supplier);

            actual.Title.Should().Be("Add a contact");
            actual.SupplierName.Should().Be(supplier.Name);
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidConstructionOverload_PropertiesSetAsExpected(
            SupplierContact contact,
            Supplier supplier,
            List<CatalogueItem> solutions)
        {
            var actual = new EditContactModel(contact, supplier, solutions);

            actual.ContactId.Should().Be(contact.Id);
            actual.SupplierId.Should().Be(contact.SupplierId);
            actual.FirstName.Should().Be(contact.FirstName);
            actual.LastName.Should().Be(contact.LastName);
            actual.Email.Should().Be(contact.Email);
            actual.PhoneNumber.Should().Be(contact.PhoneNumber);
            actual.Department.Should().Be(contact.Department);
            actual.Title.Should().Be($"{contact.FirstName} {contact.LastName} contact details");
            actual.SupplierName.Should().Be(supplier.Name);
            actual.SolutionsReferencingThisContact.Should().NotBeEmpty().And.HaveCount(solutions.Count);
        }
    }
}
