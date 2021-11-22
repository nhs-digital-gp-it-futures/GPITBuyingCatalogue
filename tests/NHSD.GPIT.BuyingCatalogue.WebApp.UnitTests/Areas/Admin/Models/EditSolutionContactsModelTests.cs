using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class EditSolutionContactsModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Constructing_ValidCatalogueItem_SetsPropertiesAsExpected(
            CatalogueItem catalogueItem)
        {
            var model = new EditSolutionContactsModel(catalogueItem);

            model.SupplierName.Should().Be(catalogueItem.Supplier.Name);
            model.SupplierSummary.Should().Be(catalogueItem.Supplier.Summary);
        }

        [Theory]
        [CommonAutoData]
        public static void Status_ValidSupplierContacts_ReturnsTaskCompleted(
            IList<SupplierContact> supplierContacts,
            CatalogueItem catalogueItem)
        {
            catalogueItem.Supplier.SupplierContacts = supplierContacts;
            catalogueItem.CatalogueItemContacts = supplierContacts;

            var model = new EditSolutionContactsModel(catalogueItem);

            model.Status().Should().Be(TaskProgress.Completed);
        }

        [Theory]
        [CommonAutoData]
        public static void Status_NoSupplierContacts_ReturnsTaskNotStarted(
            CatalogueItem catalogueItem)
        {
            catalogueItem.CatalogueItemContacts = new List<SupplierContact>();

            var model = new EditSolutionContactsModel(catalogueItem);

            model.Status().Should().Be(TaskProgress.NotStarted);
        }

        [Theory]
        [CommonAutoData]
        public static void Constructing_DisplayNameSetAsExpected(
            IList<SupplierContact> supplierContacts,
            CatalogueItem catalogueItem)
        {
            supplierContacts[0].Department = string.Empty;
            catalogueItem.Supplier.SupplierContacts = supplierContacts.Take(2).ToList();
            catalogueItem.CatalogueItemContacts = new List<SupplierContact>();

            var expectedAvailableContacts = new List<AvailableSupplierContact>
            {
                new()
                {
                    Id = supplierContacts[0].Id,
                    DisplayName = $"{supplierContacts[0].FirstName} {supplierContacts[0].LastName}",
                    Selected = false,
                },
                new()
                {
                    Id = supplierContacts[1].Id,
                    DisplayName = $"{supplierContacts[1].FirstName} {supplierContacts[1].LastName} ({supplierContacts[1].Department})",
                    Selected = false,
                },
            };

            var model = new EditSolutionContactsModel(catalogueItem);

            model.AvailableSupplierContacts.Should().BeEquivalentTo(expectedAvailableContacts);
        }
    }
}
