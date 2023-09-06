using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Suppliers
{
    public static class SupplierTemporalServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SupplierTemporalService).GetConstructors();

            assertion.Verify(constructors);
        }
    }
}
