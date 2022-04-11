using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class ServiceRecipientServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ServiceRecipientService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void AddServiceRecipient_RecipientIsNull_ThrowsException(ServiceRecipientService service)
        {
            FluentActions
                .Awaiting(() => service.AddServiceRecipient(null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddServiceRecipient_RecipientNotInDatabase_AddsServiceRecipient(
            [Frozen] BuyingCatalogueDbContext context,
            ServiceRecipientDto serviceRecipient,
            ServiceRecipientService service)
        {
            await service.AddServiceRecipient(serviceRecipient);

            var actual = context.ServiceRecipients.FirstOrDefault(x => x.OdsCode == serviceRecipient.OdsCode);

            actual.Should().NotBeNull();
            actual!.OdsCode.Should().Be(serviceRecipient.OdsCode);
            actual.Name.Should().Be(serviceRecipient.Name);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddServiceRecipient_RecipientInDatabase_UpdatesServiceRecipientName(
            [Frozen] BuyingCatalogueDbContext context,
            ServiceRecipientDto serviceRecipient,
            ServiceRecipientService service)
        {
            context.ServiceRecipients.Add(new ServiceRecipient
            {
                OdsCode = serviceRecipient.OdsCode,
                Name = string.Empty,
            });

            await context.SaveChangesAsync();
            await service.AddServiceRecipient(serviceRecipient);

            var actual = context.ServiceRecipients.FirstOrDefault(x => x.OdsCode == serviceRecipient.OdsCode);

            actual.Should().NotBeNull();
            actual!.OdsCode.Should().Be(serviceRecipient.OdsCode);
            actual.Name.Should().Be(serviceRecipient.Name);
        }
    }
}
