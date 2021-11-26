using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Supplier
{
    public static class SupplierModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            ICollection<SupplierContact> supplierContacts)
        {
            var model = new SupplierModel(odsCode, order, supplierContacts);

            model.Title.Should().Be($"Supplier information for {order.CallOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.Id.Should().Be(order.Supplier.Id);
            model.Name.Should().Be(order.Supplier.Name);
            model.Address.Should().BeEquivalentTo(order.Supplier.Address);
            model.PrimaryContact.Should().BeEquivalentTo(order.SupplierContact);
        }

        [Theory]
        [CommonAutoData]
        public static void WithNoSupplierContact_AndAvailableContacts_PropertiesCorrectlySet(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            ICollection<SupplierContact> supplierContacts)
        {
            order.SupplierContact = null;

            var model = new SupplierModel(odsCode, order, supplierContacts);

            model.PrimaryContact.FirstName.Should().BeEquivalentTo(supplierContacts.First().FirstName);
            model.PrimaryContact.LastName.Should().BeEquivalentTo(supplierContacts.First().LastName);
            model.PrimaryContact.Email.Should().BeEquivalentTo(supplierContacts.First().Email);
            model.PrimaryContact.Phone.Should().BeEquivalentTo(supplierContacts.First().PhoneNumber);
        }

        [Theory]
        [CommonAutoData]
        public static void WithNoSupplierContact_AndNoAvailableContacts_PropertiesCorrectlySet(
            string odsCode,
            EntityFramework.Ordering.Models.Order order)
        {
            order.SupplierContact = null;

            var model = new SupplierModel(odsCode, order, new List<SupplierContact>());

            model.PrimaryContact.Should().BeNull();
        }
    }
}
