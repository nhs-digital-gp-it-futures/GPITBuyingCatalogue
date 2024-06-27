using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.Services.Session;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Session
{
    public static class SupplierContactSessionServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SupplierContactSessionService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static void GetSupplierContact_NoDataInSession_ReturnsNull(
            CallOffId callOffId,
            int supplierId,
            [Frozen] ISessionService mockSessionService,
            SupplierContactSessionService systemUnderTest)
        {
            mockSessionService.GetObject<SupplierContact>(Arg.Any<string>()).Returns((SupplierContact)null);

            var result = systemUnderTest.GetSupplierContact(callOffId, supplierId);

            mockSessionService.Received().GetObject<SupplierContact>(Arg.Any<string>());

            result.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static void GetSupplierContact_DataInSession_ReturnsData(
            CallOffId callOffId,
            int supplierId,
            SupplierContact supplierContact,
            [Frozen] ISessionService mockSessionService,
            SupplierContactSessionService systemUnderTest)
        {
            mockSessionService.GetObject<SupplierContact>(Arg.Any<string>()).Returns(supplierContact);

            var result = systemUnderTest.GetSupplierContact(callOffId, supplierId);

            mockSessionService.Received().GetObject<SupplierContact>(Arg.Any<string>());

            result.Should().Be(supplierContact);
        }

        [Theory]
        [MockAutoData]
        public static void SetSupplierContact_WritesDataToSession(
            CallOffId callOffId,
            int supplierId,
            SupplierContact supplierContact,
            [Frozen] ISessionService mockSessionService,
            SupplierContactSessionService systemUnderTest)
        {
            systemUnderTest.SetSupplierContact(callOffId, supplierId, supplierContact);

            mockSessionService.Received().SetObject(Arg.Any<string>(), supplierContact);
        }
    }
}
