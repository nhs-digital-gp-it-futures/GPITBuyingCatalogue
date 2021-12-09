using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using NHSD.GPIT.BuyingCatalogue.Services.Csv;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Csv
{
    public static class CsvServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CsvService).GetConstructors();

            assertion.Verify(constructors);
        }
    }
}
