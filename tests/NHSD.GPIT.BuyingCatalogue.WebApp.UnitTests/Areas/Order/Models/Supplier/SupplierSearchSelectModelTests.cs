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
            string internalOrgId,
            CallOffId callOffId,
            List<EntityFramework.Catalogue.Models.Supplier> suppliers)
        {
            var model = new SupplierSearchSelectModel(internalOrgId, callOffId, suppliers);

            model.Title.Should().Be("Suppliers found");
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Suppliers.Should().BeEquivalentTo(suppliers);
        }
    }
}
