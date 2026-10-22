using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.CommunityPharmacy.Models;

public static class SummaryModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_PropertiesCorrectlySet(
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        var model = new SummaryModel(solution.CatalogueItem, contentStatus);

        model.Summary.Should().Be(solution.Summary);
        model.Contacts.Should().BeEquivalentTo(solution.CatalogueItem.Supplier.SupplierContacts);
        model.SupplierUrl.Should().Be(solution.CatalogueItem.Supplier.SupplierUrl);
        model.SelectedSection.Should().Be("Summary");
        model.PaginationFooter.Previous.Should().BeNull();
        model.PaginationFooter.Next.Name.Should().Be("List price");
    }

    [Theory]
    [MockInlineAutoData(null, false)]
    [MockInlineAutoData("", false)]
    [MockInlineAutoData(" ", false)]
    [MockInlineAutoData("Solution summary", true)]
    public static void HasSummary_ReturnsExpected(
        string summary,
        bool expected,
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        solution.Summary = summary;

        var model = new SummaryModel(solution.CatalogueItem, contentStatus);

        model.HasSummary().Should().Be(expected);
    }

    [Theory]
    [MockInlineAutoData(null, false)]
    [MockInlineAutoData("", false)]
    [MockInlineAutoData(" ", false)]
    [MockInlineAutoData("supplier.example", true)]
    public static void HasSupplierUrl_ReturnsExpected(
        string supplierUrl,
        bool expected,
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        solution.CatalogueItem.Supplier.SupplierUrl = supplierUrl;

        var model = new SummaryModel(solution.CatalogueItem, contentStatus);

        model.HasSupplierUrl().Should().Be(expected);
    }

    [Theory]
    [MockAutoData]
    public static void HasContactDetails_WithContacts_ReturnsTrue(
        Solution solution,
        List<SupplierContact> contacts,
        CatalogueItemContentStatus contentStatus)
    {
        solution.CatalogueItem.Supplier.SupplierContacts = contacts;

        var model = new SummaryModel(solution.CatalogueItem, contentStatus);

        model.HasContactDetails().Should().BeTrue();
    }

    [Theory]
    [MockAutoData]
    public static void HasContactDetails_WithoutContacts_ReturnsFalse(
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        solution.CatalogueItem.Supplier.SupplierContacts.Clear();

        var model = new SummaryModel(solution.CatalogueItem, contentStatus);

        model.HasContactDetails().Should().BeFalse();
    }

    [Theory]
    [MockAutoData]
    public static void HasContactDetails_NullContacts_ReturnsFalse(
        Solution solution,
        CatalogueItemContentStatus contentStatus)
    {
        var model = new SummaryModel(solution.CatalogueItem, contentStatus) { Contacts = null };

        model.HasContactDetails().Should().BeFalse();
    }
}
