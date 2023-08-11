using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using BuyingCatalogueFunction.IncrementalUpdate.Interfaces;
using BuyingCatalogueFunction.IncrementalUpdate.Models;
using BuyingCatalogueFunction.IncrementalUpdate.Models.Ods;
using BuyingCatalogueFunction.IncrementalUpdate.Services;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace BuyingCatalogueFunctionTests.Services
{
    public static class IncrementalUpdateServiceTests
    {
        [Fact]
        public static void Constructor_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(IncrementalUpdateService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateOrganisationData_OdsServiceReturnsNothing_ExpectedResult(
            DateTime lastRunDate,
            [Frozen] Mock<IOdsService> odsService,
            [Frozen] Mock<IOrganisationUpdateService> organisationUpdateService,
            IncrementalUpdateService service)
        {
            organisationUpdateService
                .Setup(x => x.GetLastRunDate())
                .ReturnsAsync(lastRunDate);

            odsService
                .Setup(x => x.SearchByLastChangeDate(lastRunDate))
                .ReturnsAsync(Enumerable.Empty<string>());

            await service.UpdateOrganisationData();

            odsService.VerifyAll();
            odsService.VerifyNoOtherCalls();
            organisationUpdateService.VerifyAll();
            organisationUpdateService.VerifyNoOtherCalls();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateOrganisationData_OdsServiceReturnsData_ExpectedResult(
            DateTime lastRunDate,
            Org organisation,
            Org relatedOrganisation,
            List<Relationship> relationships,
            List<Role> roles,
            [Frozen] Mock<IOdsService> odsService,
            [Frozen] Mock<IOrganisationUpdateService> organisationUpdateService,
            IncrementalUpdateService service)
        {
            odsService
                .Setup(x => x.SearchByLastChangeDate(lastRunDate))
                .ReturnsAsync(new[] { organisation.OrgId.extension });

            odsService
                .Setup(x => x.GetRelationships())
                .ReturnsAsync(relationships);

            odsService
                .Setup(x => x.GetRoles())
                .ReturnsAsync(roles);

            odsService
                .Setup(x => x.GetOrganisation(It.IsAny<string>()))
                .ReturnsAsync(relatedOrganisation);

            odsService
                .Setup(x => x.GetOrganisation(organisation.OrgId.extension))
                .ReturnsAsync(organisation);

            IncrementalUpdateData data = new();

            organisationUpdateService
                .Setup(x => x.IncrementalUpdate(It.IsAny<IncrementalUpdateData>()))
                .Callback<IncrementalUpdateData>(x => data = x);

            organisationUpdateService
                .Setup(x => x.GetLastRunDate())
                .ReturnsAsync(lastRunDate);

            organisationUpdateService
                .Setup(x => x.SetLastRunDate(DateTime.Today))
                .Verifiable();

            await service.UpdateOrganisationData();

            odsService.VerifyAll();
            organisationUpdateService.VerifyAll();

            data.Relationships.Should().BeEquivalentTo(relationships);
            data.Roles.Should().BeEquivalentTo(roles);
            data.Organisations.Should().BeEquivalentTo(new[] { organisation });
            data.RelatedOrganisations.Should().BeEquivalentTo(new[]
            {
                relatedOrganisation,
                relatedOrganisation,
                relatedOrganisation,
            });
        }
    }
}
