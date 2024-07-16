using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ServiceLevelAgreements;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.ServiceLevelAgreements
{
    public static class EditSlaTypeConfirmationModelTests
    {
        [Fact]
        public static void Constructor_NullSupplierContact_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new EditSlaTypeConfirmationModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }

        [Theory]
        [MockAutoData]
        public static void WithValidConstruction_PropertiesSetAsExpected(
            Solution solution)
        {
            solution.ServiceLevelAgreement.SlaType = SlaType.Type1;

            var actual = new EditSlaTypeConfirmationModel(solution.CatalogueItem);

            actual.SolutionName.Should().Be(solution.CatalogueItem.Name);

            actual.Advice.Should().Be($"If you change from a {SlaType.Type1} to a {SlaType.Type2} Catalogue Solution, the SLA information that was previously entered will be replaced");
        }
    }
}
