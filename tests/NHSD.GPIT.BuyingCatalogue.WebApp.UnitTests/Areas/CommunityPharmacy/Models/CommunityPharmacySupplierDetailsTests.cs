using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.CommunityPharmacy.Models;

public static class CommunityPharmacySupplierDetailsTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_MapsSupplierDetailsAndContacts(
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        var supplier = solution.CatalogueItem.Supplier;

        var model = new CommunityPharmacySupplierDetails(solution.CatalogueItem, contentStatus);

        model.Summary.Should().Be(supplier.Summary);
        model.Name.Should().Be(supplier.Name);
        model.Url.Should().Be(supplier.SupplierUrl);
        model.Contacts.Should().BeEquivalentTo(supplier.SupplierContacts.Select(x => new SupplierContactViewModel(x)));
        model.HasContacts().Should().BeTrue();
        model.SelectedSection.Should().Be("Supplier contact details");
        model.PaginationFooter.Previous.Name.Should().Be("Development plans");
        model.PaginationFooter.Next.Should().BeNull();
    }

    [Theory]
    [MockAutoData]
    public static void HasContacts_EmptyContacts_ReturnsFalse(
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        solution.CatalogueItem.Supplier.SupplierContacts.Clear();

        var model = new CommunityPharmacySupplierDetails(solution.CatalogueItem, contentStatus);

        model.HasContacts().Should().BeFalse();
    }

    [Theory]
    [MockAutoData]
    public static void HasContacts_NullContacts_ReturnsFalse(
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        var model = new CommunityPharmacySupplierDetails(solution.CatalogueItem, contentStatus) { Contacts = null };

        model.HasContacts().Should().BeFalse();
    }

    [Theory]
    [MockAutoData]
    public static void HasContacts_OnlyNullContacts_ReturnsFalse(
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        var model = new CommunityPharmacySupplierDetails(solution.CatalogueItem, contentStatus)
        {
            Contacts = new List<SupplierContactViewModel> { null },
        };

        model.HasContacts().Should().BeFalse();
    }
}
