using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;
using EntityOdsOrganisation = NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models.OdsOrganisation;
using ServiceContractOdsOrganisation = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations.OdsOrganisation;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Competitions
{
    public static class CompetitionSublocationServiceTests
    {
        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetCompetitionSublocationWithRecipients_ReturnsCompetitionSublocation(
            Organisation organisation,
            Competition competition,
            CompetitionSublocation sublocation,
            EntityOdsOrganisation sublocationOrganisation,
            List<CompetitionSublocationRecipient> competitionSublocationRecipients,
            [Frozen] BuyingCatalogueDbContext context,
            CompetitionSublocationService service)
        {
            competition.Organisation = organisation;

            sublocation.Competition = competition;
            sublocation.CompetitionId = competition.Id;

            sublocation.SublocationOrganisation = sublocationOrganisation;
            sublocation.SublocationRecipients = competitionSublocationRecipients;

            competitionSublocationRecipients.ForEach(
                x => x.RecipientOrganisation = CommonEntityOdsOrganisationFactory(x.RecipientOdsCode));

            context.Add(sublocation);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            CompetitionSublocation actualSublocation = await service.GetCompetitionSublocationWithRecipients(
                organisation.ExternalIdentifier,
                competition.Id,
                sublocation.SublocationOdsCode);

            actualSublocation.Should().BeEquivalentTo(sublocation);
        }

        public static IEnumerable<object[]> CompetitionSublocationsWithRecipientsForCount()
        {
            return [[new List<CompetitionSublocation>()]];
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetCountForCompetitionSublocationRecipients_ReturnsCount(
            List<CompetitionSublocation> competitionSublocationsWithRecipients,
            int expectedQuantity,
            Competition competition,
            Organisation organisation,
            [Frozen] BuyingCatalogueDbContext context,
            CompetitionSublocationService service)
        {
            Assert.Fail("Not implemented");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddSublocationRecipients_RejectsNullArguments(
            CompetitionSublocationService service)
        {
            Assert.Fail("not implemented");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddSublocationRecipients_RejectsInvalidOperations(
            CompetitionSublocationService service)
        {
            Assert.Fail("not implemented");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddSublocationRecipients_AddsAsExpected(
            CompetitionSublocationService service)
        {
            Assert.Fail("not implemented");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task RemoveSublocationRecipients_RejectsNullArguments(
            CompetitionSublocationService service)
        {
            Assert.Fail("not implemented");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task RemoveSublocationRecipients_RejectsInvalidOperations(
            CompetitionSublocationService service)
        {
            Assert.Fail("not implemented");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task RemoveSublocationRecipients_RemovesAsExpected(
            CompetitionSublocationService service)
        {
            Assert.Fail("not implemented");
        }

        private const int CommonCompetitionId = 34;

        private const int CommonOrganisationId = 21;
        private const string CommonOrganisationInternalIdentifier = "BB-FFGG";
        private const string CommonOrganisationExternalIdentifier = "FFGG";

        private static Organisation CommonOrganisationFactory(int customId = 0)
        {
            return new Organisation
            {
                Id = customId == 0 ? CommonCompetitionId : customId,
                InternalIdentifier = CommonOrganisationInternalIdentifier,
                ExternalIdentifier = CommonOrganisationExternalIdentifier,
                Name = "A Local ICB",
            };
        }

        private static Competition CommonCompetitionFactory(int customId = 0, int customOrganisationId = 0)
        {
            return new Competition
            {
                Id = customId == 0 ? CommonCompetitionId : customId,
                OrganisationId = customOrganisationId == 0 ? CommonOrganisationId : customOrganisationId,
                Name = "My Competition",
                Description = "Competition for competitiony things",
            };
        }

        private static CompetitionSublocation CommonCompetitionSublocationFactory(
            string sublocationOdsCode,
            List<CompetitionSublocationRecipient> sublocationRecipients = null,
            bool hasOrganisation = false)
        {
            return new CompetitionSublocation
            {
                CompetitionId = CommonCompetitionId,
                SublocationOdsCode = sublocationOdsCode,
                OwnerOdsCode = CommonOrganisationExternalIdentifier,
                SublocationRecipients = sublocationRecipients,
                SublocationOrganisation =
                    hasOrganisation ? CommonEntityOdsOrganisationFactory(sublocationOdsCode) : null,
            };
        }

        private static CompetitionSublocationRecipient CommonCompetitionSublocationRecipientFactory(
            string recipientOdsCode,
            string parentSublocationOdsCode,
            bool hasOrganisation = false)
        {
            return new CompetitionSublocationRecipient
            {
                CompetitionId = CommonCompetitionId,
                RecipientOdsCode = recipientOdsCode,
                ParentSublocationOdsCode = parentSublocationOdsCode,
                RecipientOrganisation = hasOrganisation ? CommonEntityOdsOrganisationFactory(recipientOdsCode) : null,
            };
        }

        private static EntityOdsOrganisation CommonEntityOdsOrganisationFactory(string id)
        {
            return new EntityOdsOrganisation { Id = id, Name = $"An organisation - {id}", IsActive = true };
        }

        private static ServiceContractOdsOrganisation CommonServiceContractOdsOrganisationFactory(string id)
        {
            return new ServiceContractOdsOrganisation
            {
                OdsCode = id, OrganisationName = $"An organisation - {id}", IsActive = true,
            };
        }

        private static ServiceRecipient CommonServiceRecipientFactory(string orgId, string locationOrgId)
        {
            return new ServiceRecipient { OrgId = orgId, LocationOrgId = locationOrgId };
        }
    }
}
