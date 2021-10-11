using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using NHSD.GPIT.BuyingCatalogue.Services.Suppliers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Suppliers
{
    public static class SuppliersServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SuppliersService).GetConstructors();

            assertion.Verify(constructors);
        }
    }
}
