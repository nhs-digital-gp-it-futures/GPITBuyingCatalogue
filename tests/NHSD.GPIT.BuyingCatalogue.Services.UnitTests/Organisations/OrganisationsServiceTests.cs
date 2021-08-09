using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
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
        [CommonAutoData]
        public static async Task AddOdsOrganisation_OrganisationAlreadyExists_ReturnsError(
            OdsOrganisation odsOrganisation,
            bool agreementSigned,
            Organisation organisation,
            [Frozen] Mock<IDbRepository<Organisation, BuyingCatalogueDbContext>> repositoryMock,
            OrganisationsService service)
        {
            repositoryMock.Setup(r => r.GetAllAsync(o => o.OdsCode == odsOrganisation.OdsCode))
                .ReturnsAsync(new[] { organisation });

            (Guid orgId, var error) = await service.AddOdsOrganisation(odsOrganisation, agreementSigned);

            orgId.Should().Be(Guid.Empty);
            error.Should().Be($"The organisation with ODS code {odsOrganisation.OdsCode} already exists.");
        }

        [Theory]
        [CommonAutoData]
        public static async Task AddOdsOrganisation_OrganisationCreated(
            OdsOrganisation odsOrganisation,
            bool agreementSigned,
            [Frozen] Mock<IDbRepository<Organisation, BuyingCatalogueDbContext>> repositoryMock,
            OrganisationsService service)
        {
            repositoryMock.Setup(r => r.GetAllAsync(o => o.OdsCode == odsOrganisation.OdsCode))
                .ReturnsAsync(Array.Empty<Organisation>());

            Organisation newOrganisation = null;

            repositoryMock.Setup(s => s.Add(It.IsAny<Organisation>()))
                .Callback<Organisation>(o => newOrganisation = o);

            (Guid orgId, var error) = await service.AddOdsOrganisation(odsOrganisation, agreementSigned);

            orgId.Should().NotBe(Guid.Empty);
            error.Should().BeNull();

            repositoryMock.Verify(v => v.Add(It.IsAny<Organisation>()), Times.Once);
            repositoryMock.Verify(v => v.SaveChangesAsync(), Times.Once);

            newOrganisation.Address.Should().BeEquivalentTo(odsOrganisation.Address);
            newOrganisation.Id.Should().Be(orgId);
            newOrganisation.CatalogueAgreementSigned.Should().Be(agreementSigned);
            newOrganisation.LastUpdated.Date.Should().Be(DateTime.UtcNow.Date);
            newOrganisation.Name.Should().Be(odsOrganisation.OrganisationName);
            newOrganisation.OdsCode.Should().Be(odsOrganisation.OdsCode.ToUpper());
            newOrganisation.PrimaryRoleId.Should().Be(odsOrganisation.PrimaryRoleId);
        }
    }
}
