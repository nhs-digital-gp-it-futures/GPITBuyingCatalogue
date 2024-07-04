using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.SupplierDefinedEpics
{
    public static class SupplierDefinedEpicModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Constructor_PropertiesAreSetCorrectly(
            Epic epic)
        {
            var model = new SupplierDefinedEpicModel(epic);

            model.Id.Should().Be(epic.Id);
            model.Name.Should().Be(epic.Name);
            model.IsActive.Should().Be(epic.IsActive);
        }
    }
}
