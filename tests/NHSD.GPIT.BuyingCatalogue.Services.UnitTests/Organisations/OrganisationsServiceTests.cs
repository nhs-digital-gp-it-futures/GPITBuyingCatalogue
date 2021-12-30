using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Services.Organisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Organisations
{
    public static class OrganisationsServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrganisationsService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static Task AddOdsOrganisation_NullOdsOrganisation_ThrowsException(OrganisationsService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.AddOdsOrganisation(null, true));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddOdsOrganisation_OrganisationAlreadyExists_ReturnsError(
            [Frozen] BuyingCatalogueDbContext context,
            OdsOrganisation odsOrganisation,
            bool agreementSigned,
            Organisation organisation,
            OrganisationsService service)
        {
            organisation.OdsCode = odsOrganisation.OdsCode;
            context.Organisations.Add(organisation);
            await context.SaveChangesAsync();

            (int orgId, var error) = await service.AddOdsOrganisation(odsOrganisation, agreementSigned);

            orgId.Should().Be(0);
            error.Should().Be($"The organisation with ODS code {odsOrganisation.OdsCode} already exists.");
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddOdsOrganisation_OrganisationCreated(
            [Frozen] BuyingCatalogueDbContext context,
            OdsOrganisation odsOrganisation,
            bool agreementSigned,
            OrganisationsService service)
        {
            (int orgId, var error) = await service.AddOdsOrganisation(odsOrganisation, agreementSigned);

            orgId.Should().NotBe(0);
            error.Should().BeNull();

            var newOrganisation = await context.Organisations.SingleAsync(o => o.Id == orgId);

            newOrganisation.Address.Should().BeEquivalentTo(odsOrganisation.Address);
            newOrganisation.Id.Should().Be(orgId);
            newOrganisation.CatalogueAgreementSigned.Should().Be(agreementSigned);
            newOrganisation.LastUpdated.Date.Should().Be(DateTime.UtcNow.Date);
            newOrganisation.Name.Should().Be(odsOrganisation.OrganisationName);
            newOrganisation.OdsCode.Should().Be(odsOrganisation.OdsCode);
            newOrganisation.PrimaryRoleId.Should().Be(odsOrganisation.PrimaryRoleId);
        }
    }
}
