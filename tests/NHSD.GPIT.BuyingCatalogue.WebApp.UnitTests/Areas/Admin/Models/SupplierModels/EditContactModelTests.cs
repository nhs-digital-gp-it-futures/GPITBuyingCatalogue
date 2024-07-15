using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
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
        [MockAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            Supplier supplier)
        {
            var actual = new EditContactModel(supplier);

            actual.SupplierId.Should().Be(supplier.Id);
            actual.Caption.Should().Be(supplier.Name);
            actual.Advice.Should().Be(EditContactModel.AddContactAdvice);
        }

        [Theory]
        [MockAutoData]
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
            actual.SolutionsReferencingThisContact.Should().NotBeEmpty().And.HaveCount(solutions.Count);
            actual.Caption.Should().Be(contact.NameOrDepartment);
            actual.Advice.Should().Be(string.Format(EditContactModel.EditContactAdvice, supplier.Name));
        }

        [Theory]
        [MockAutoData]
        public static void ValidConstructor_WithActiveSupplierAndASingleUnreferencedContact_AssignsCanDelete_False(
            Supplier supplier,
            SupplierContact contact)
        {
            supplier.IsActive = true;
            supplier.SupplierContacts = new List<SupplierContact>(new[] { contact });
            var solutions = new List<CatalogueItem>();

            var actual = new EditContactModel(supplier.SupplierContacts.First(), supplier, solutions);

            actual.CanDelete.Should().Be(false);
        }

        [Theory]
        [MockAutoData]
        public static void ValidConstructor_WithInactiveSupplierAndASingleUnreferencedContact_AssignsCanDelete_True(
            Supplier supplier,
            SupplierContact contact)
        {
            supplier.IsActive = false;
            supplier.SupplierContacts = new List<SupplierContact>(new[] { contact });
            var solutions = new List<CatalogueItem>();

            var actual = new EditContactModel(supplier.SupplierContacts.First(), supplier, solutions);

            actual.CanDelete.Should().Be(true);
        }

        [Theory]
        [MockAutoData]
        public static void ValidConstructor_WithInactiveSupplierAndASingleReferencedContact_AssignsCanDelete_False(
            Supplier supplier,
            SupplierContact contact,
            CatalogueItem catalogueItem)
        {
            supplier.IsActive = false;
            supplier.SupplierContacts = new List<SupplierContact>(new[] { contact });
            var solutions = new List<CatalogueItem>(new[] { catalogueItem });

            var actual = new EditContactModel(supplier.SupplierContacts.First(), supplier, solutions);

            actual.CanDelete.Should().Be(false);
        }

        [Theory]
        [MockAutoData]
        public static void ValidConstructor_WithActiveSupplierAndUnreferencedContacts_AssignsCanDelete_True(
            Supplier supplier,
            IFixture fixture)
        {
            supplier.IsActive = true;
            supplier.SupplierContacts = new List<SupplierContact>();
            fixture.AddManyTo(supplier.SupplierContacts, 2);

            var solutions = new List<CatalogueItem>();

            var actual = new EditContactModel(supplier.SupplierContacts.First(), supplier, solutions);

            actual.CanDelete.Should().Be(true);
        }
    }
}
