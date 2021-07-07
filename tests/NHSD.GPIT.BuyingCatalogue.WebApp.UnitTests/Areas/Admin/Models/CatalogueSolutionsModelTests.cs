using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;
using PublicationStatus = NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models.PublicationStatus;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class CatalogueSolutionsModelTests
    {
        [Fact]
        public static void AllPublicationStatuses_StandardCall_ResultAsExpected()
        {
            var actual = new CatalogueSolutionsModel(new List<CatalogueItem>()).AllPublicationStatuses;

            actual.Should()
                .BeEquivalentTo(
                    new List<WebApp.Areas.Admin.Models.PublicationStatus>
                    {
                        new() { Id = 1, Display = "Draft" },
                        new() { Id = 3, Display = "Published" },
                        new() { Id = 2, Display = "Unpublished" },
                        new() { Id = 5, Display = "In Remediation" },
                        new() { Id = 4, Display = "Suspended" },
                    });
        }

        [Theory]
        [InlineData(PublicationStatus.Draft)]
        [InlineData(PublicationStatus.Published)]
        [InlineData(PublicationStatus.Unpublished)]
        [InlineData(PublicationStatus.Suspended)]
        [InlineData(PublicationStatus.InRemediation)]
        public static void HasSelected_StatusSelected_ReturnsTrue(PublicationStatus publicationStatus)
        {
            var model = new CatalogueSolutionsModel(new List<CatalogueItem>());
            model.HasSelected.Should().BeFalse();
            
            model.SetSelected(publicationStatus);

            model.HasSelected.Should().BeTrue();
        }

        [Theory]
        [InlineData(PublicationStatus.Draft)]
        [InlineData(PublicationStatus.Published)]
        [InlineData(PublicationStatus.Unpublished)]
        [InlineData(PublicationStatus.Suspended)]
        [InlineData(PublicationStatus.InRemediation)]
        public static void SetSelected_StatusInput_SetsCorrespondingItemSelected(PublicationStatus publicationStatus)
        {
            var model = new CatalogueSolutionsModel(new List<CatalogueItem>());
            
            model.SetSelected(publicationStatus);

            model.AllPublicationStatuses.Single(p => p.Checked).Id.Should().Be((int)publicationStatus);
        }
    }
}
