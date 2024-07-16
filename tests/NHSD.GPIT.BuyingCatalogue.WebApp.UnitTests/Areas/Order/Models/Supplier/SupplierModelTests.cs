using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Supplier;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Supplier
{
    public static class SupplierModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order)
        {
            var model = new SupplierModel(internalOrgId, callOffId, order);

            model.Title.Should().Be("Supplier contact details");
            model.CallOffId.Should().Be(callOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.SupplierId.Should().Be(order.Supplier.Id);
            model.SupplierName.Should().Be(order.Supplier.Name);

            model.Contacts.Should().BeNull();
            model.FormattedContacts.Should().BeNull();
            model.TemporaryContact.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static void WithContacts_PropertiesCorrectlySet(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<SupplierContact> supplierContacts)
        {
            var model = new SupplierModel(internalOrgId, callOffId, order)
            {
                Contacts = supplierContacts,
            };

            model.Title.Should().Be("Supplier contact details");
            model.CallOffId.Should().Be(callOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.SupplierId.Should().Be(order.Supplier.Id);
            model.SupplierName.Should().Be(order.Supplier.Name);

            model.Contacts.Should().BeEquivalentTo(supplierContacts);

            foreach (var contact in supplierContacts)
            {
                model.FormattedContacts.Should().Contain(x => x.Value == contact.Id
                    && x.Text.Contains(contact.FirstName)
                    && x.Text.Contains(contact.LastName)
                    && x.Text.Contains(contact.Department));
            }

            model.TemporaryContact.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static void WithContacts_AndATemporaryContact_PropertiesCorrectlySet(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<SupplierContact> supplierContacts)
        {
            supplierContacts.First().Id = SupplierContact.TemporaryContactId;

            var model = new SupplierModel(internalOrgId, callOffId, order)
            {
                Contacts = supplierContacts,
            };

            model.Title.Should().Be("Supplier contact details");
            model.CallOffId.Should().Be(callOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.SupplierId.Should().Be(order.Supplier.Id);
            model.SupplierName.Should().Be(order.Supplier.Name);

            model.Contacts.Should().BeEquivalentTo(supplierContacts);

            foreach (var contact in supplierContacts)
            {
                model.FormattedContacts.Should().Contain(x => x.Value == contact.Id
                    && x.Text.Contains(contact.FirstName)
                    && x.Text.Contains(contact.LastName)
                    && x.Text.Contains(contact.Department));
            }

            model.TemporaryContact.Should().Be(supplierContacts.First());
        }
    }
}
