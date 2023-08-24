using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using BuyingCatalogueFunction.IncrementalUpdate.Models;
using BuyingCatalogueFunction.IncrementalUpdate.Services;
using FluentAssertions;
using Flurl.Http.Testing;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace BuyingCatalogueFunctionTests.Services
{
    public static class OdsServiceTests
    {
        [Fact]
        public static void Constructor_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OdsService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonInlineAutoData(301)]
        [CommonInlineAutoData(401)]
        [CommonInlineAutoData(404)]
        public static async Task GetOrganisation_Error_ExpectedResult(
            int status,
            string organisationId,
            OdsService service)
        {
            new HttpTest().RespondWith(status: status);

            var result = await service.GetOrganisation(organisationId);

            result.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static async Task GetOrganisation_EverythingOk_ExpectedResult(
            string organisationId,
            OrganisationResponse response,
            OdsService service)
        {
            new HttpTest().RespondWith(status: 200, body: JsonConvert.SerializeObject(response));

            var result = await service.GetOrganisation(organisationId);

            result.Should().BeEquivalentTo(response.Organisation);
        }

        [Theory]
        [CommonInlineAutoData(301)]
        [CommonInlineAutoData(401)]
        [CommonInlineAutoData(404)]
        public static async Task GetRelationships_Error_ExpectedResult(
            int status,
            OdsService service)
        {
            new HttpTest().RespondWith(status: status);

            var result = await service.GetRelationships();

            result.Should().BeEmpty();
        }

        [Theory]
        [CommonAutoData]
        public static async Task GetRelationships_EverythingOk_ExpectedResult(
            RelationshipsResponse response,
            OdsService service)
        {
            new HttpTest().RespondWith(status: 200, body: JsonConvert.SerializeObject(response));

            var result = await service.GetRelationships();

            result.Should().BeEquivalentTo(response.Relationships);
        }

        [Theory]
        [CommonInlineAutoData(301)]
        [CommonInlineAutoData(401)]
        [CommonInlineAutoData(404)]
        public static async Task GetRoles_Error_ExpectedResult(
            int status,
            OdsService service)
        {
            new HttpTest().RespondWith(status: status);

            var result = await service.GetRoles();

            result.Should().BeEmpty();
        }

        [Theory]
        [CommonAutoData]
        public static async Task GetRoles_EverythingOk_ExpectedResult(
            RolesResponse response,
            OdsService service)
        {
            new HttpTest().RespondWith(status: 200, body: JsonConvert.SerializeObject(response));

            var result = await service.GetRoles();

            result.Should().BeEquivalentTo(response.Roles);
        }

        [Theory]
        [CommonInlineAutoData(301)]
        [CommonInlineAutoData(401)]
        [CommonInlineAutoData(404)]
        public static async Task SearchByLastChangeDate_Error_ExpectedResult(
            int status,
            DateTime lastChangeDate,
            OdsService service)
        {
            new HttpTest().RespondWith(status: status);

            var result = await service.SearchByLastChangeDate(lastChangeDate);

            result.Should().BeEmpty();
        }

        [Theory]
        [CommonAutoData]
        public static async Task SearchByLastChangeDate_EverythingOk_ExpectedResult(
            DateTime lastChangeDate,
            SearchResponse response,
            OdsService service)
        {
            new HttpTest().RespondWith(status: 200, body: JsonConvert.SerializeObject(response));

            var result = await service.SearchByLastChangeDate(lastChangeDate);

            result.Should().BeEquivalentTo(response.OrganisationIds);
        }
    }
}
