using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.Services.Session;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Session
{
    public static class SupplierContactSessionServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SupplierContactSessionService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static void GetSupplierContact_NoDataInSession_ReturnsNull(
            CallOffId callOffId,
            int supplierId,
            [Frozen] Mock<ISessionService> mockSessionService,
            SupplierContactSessionService systemUnderTest)
        {
            mockSessionService
                .Setup(x => x.GetObject<SupplierContact>(It.IsAny<string>()))
                .Returns((SupplierContact)null);

            var result = systemUnderTest.GetSupplierContact(callOffId, supplierId);

            mockSessionService.VerifyAll();

            result.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void GetSupplierContact_DataInSession_ReturnsData(
            CallOffId callOffId,
            int supplierId,
            SupplierContact supplierContact,
            [Frozen] Mock<ISessionService> mockSessionService,
            SupplierContactSessionService systemUnderTest)
        {
            mockSessionService
                .Setup(x => x.GetObject<SupplierContact>(It.IsAny<string>()))
                .Returns(supplierContact);

            var result = systemUnderTest.GetSupplierContact(callOffId, supplierId);

            mockSessionService.VerifyAll();

            result.Should().Be(supplierContact);
        }

        [Theory]
        [CommonAutoData]
        public static void SetSupplierContact_WritesDataToSession(
            CallOffId callOffId,
            int supplierId,
            SupplierContact supplierContact,
            [Frozen] Mock<ISessionService> mockSessionService,
            SupplierContactSessionService systemUnderTest)
        {
            mockSessionService
                .Setup(x => x.SetObject(It.IsAny<string>(), supplierContact))
                .Verifiable();

            systemUnderTest.SetSupplierContact(callOffId, supplierId, supplierContact);

            mockSessionService.VerifyAll();
        }
    }
}
