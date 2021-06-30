using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class CatalogueSolutionsModelTests
    {
        [Fact]
        public static void AllPublicationStatuses_StandardCall_ResultAsExpected()
        {
            var actual = new CatalogueSolutionsModel().AllPublicationStatuses;

            var publicationStatuses = Enum.GetValues<PublicationStatus>();
            actual.Count.Should().Be(publicationStatuses.Length);
            foreach (var status in publicationStatuses)
            {
                actual.Single(p => p.Id == (int)status).Display.Should().Be(status.GetDisplayName());
            }
        }
    }
}
