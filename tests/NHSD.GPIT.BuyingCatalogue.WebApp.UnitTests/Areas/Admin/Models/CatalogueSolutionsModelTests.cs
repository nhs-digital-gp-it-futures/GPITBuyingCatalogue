using System;
using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;
using PublicationStatus = NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue.PublicationStatus;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class CatalogueSolutionsModelTests
    {
        [Fact]
        public static void AllPublicationStatuses_StandardCall_ResultAsExpected()
        {
            var actual = new CatalogueSolutionsModel(new List<CatalogueItem>()).AllPublicationStatuses;

            var publicationStatuses = Enum.GetValues<PublicationStatus>();
            actual.Count.Should().Be(publicationStatuses.Length);
            foreach (var status in publicationStatuses)
            {
                actual.Single(p => p.Id == (int)status).Display.Should().Be(status.AsString(EnumFormat.DisplayName));
            }
        }

        [Theory]
        [InlineData(PublicationStatus.Draft)]
        [InlineData(PublicationStatus.Published)]
        [InlineData(PublicationStatus.Unpublished)]
        [InlineData(PublicationStatus.Withdrawn)]
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
        [InlineData(PublicationStatus.Withdrawn)]
        public static void SetSelected_StatusInput_SetsCorrespondingItemSelected(PublicationStatus publicationStatus)
        {
            var model = new CatalogueSolutionsModel(new List<CatalogueItem>());
            
            model.SetSelected(publicationStatus);

            model.AllPublicationStatuses.Single(p => p.Checked).Id.Should().Be((int)publicationStatus);
        }
    }
}
