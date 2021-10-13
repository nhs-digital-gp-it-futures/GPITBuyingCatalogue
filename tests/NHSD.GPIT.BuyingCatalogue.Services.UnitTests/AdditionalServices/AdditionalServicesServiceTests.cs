using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using NHSD.GPIT.BuyingCatalogue.Services.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.AdditionalServices
{
    public static class AdditionalServicesServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AdditionalServicesService).GetConstructors();

            assertion.Verify(constructors);
        }
    }
}
