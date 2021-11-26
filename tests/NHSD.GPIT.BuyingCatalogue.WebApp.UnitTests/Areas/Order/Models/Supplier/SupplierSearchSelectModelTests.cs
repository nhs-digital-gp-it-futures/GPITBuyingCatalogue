using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Supplier
{
    public static class SupplierSearchSelectModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            CallOffId callOffId,
            List<EntityFramework.Catalogue.Models.Supplier> suppliers)
        {
            var model = new SupplierSearchSelectModel(odsCode, callOffId, suppliers);

            model.Title.Should().Be("Suppliers found");
            model.OdsCode.Should().Be(odsCode);
            model.Suppliers.Should().BeEquivalentTo(suppliers);
        }
    }
}
