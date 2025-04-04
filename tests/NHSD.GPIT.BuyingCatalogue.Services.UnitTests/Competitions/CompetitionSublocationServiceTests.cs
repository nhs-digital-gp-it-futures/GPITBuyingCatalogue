using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Services.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
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
            sublocation.OwnerOdsCode = organisation.ExternalIdentifier;

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

            actualSublocation.Should()
                .BeEquivalentTo(
                    sublocation,
                    opt => opt.Excluding(m => m.Competition)
                        .Excluding(m => m.SublocationOrganisation)
                        .Excluding(m => m.SublocationRecipients));
            actualSublocation.SublocationRecipients.Should()
                .BeEquivalentTo(
                    competitionSublocationRecipients,
                    opt => opt.Excluding(m => m.Competition)
                        .Excluding(m => m.RecipientOrganisation)
                        .Excluding(m => m.ParentSublocation));
        }

        public static IEnumerable<object[]> CompetitionSublocationsWithRecipientsForCount()
        {
            return
            [
                [
                    CommonOrganisationFactory(57), CommonCompetitionFactory(22, 57),
                    CommonCompetitionSublocationFactory(
                        "XXXX",
                        [
                            CommonCompetitionSublocationRecipientFactory("AAAA", "XXXX", 22),
                            CommonCompetitionSublocationRecipientFactory("AAAB", "XXXX"),
                        ]),
                    2,
                ],
                [
                    CommonOrganisationFactory(34), CommonCompetitionFactory(89, 34),
                    CommonCompetitionSublocationFactory(
                        "XXXX",
                        [
                        ]),
                    0,
                ],
                [
                    CommonOrganisationFactory(61), CommonCompetitionFactory(61, 79),
                    CommonCompetitionSublocationFactory(
                        "XXXX",
                        [
                            CommonCompetitionSublocationRecipientFactory("AAAA", "XXXX", 61),
                        ]),
                    1,
                ],
                [
                    CommonOrganisationFactory(1), CommonCompetitionFactory(43, 1),
                    CommonCompetitionSublocationFactory(
                        "XXXX",
                        [
                            CommonCompetitionSublocationRecipientFactory("AAAA", "XXXX", 1),
                            CommonCompetitionSublocationRecipientFactory("AAAB", "XXXX", 1),
                            CommonCompetitionSublocationRecipientFactory("AAAC", "XXXX", 1),
                            CommonCompetitionSublocationRecipientFactory("AAAD", "XXXX", 1),
                            CommonCompetitionSublocationRecipientFactory("AAAE", "XXXX", 1),
                        ]),
                    5,
                ],
            ];
        }

        [Theory]
        [MockInMemoryDbMemberAutoData(nameof(CompetitionSublocationsWithRecipientsForCount))]
        public static async Task GetCountForCompetitionSublocationRecipients_ReturnsCount(
            Organisation organisation,
            Competition competition,
            CompetitionSublocation sublocation,
            int expectedCount,
            [Frozen] BuyingCatalogueDbContext context,
            CompetitionSublocationService service)
        {
            competition.Organisation = organisation;

            sublocation.Competition = competition;
            sublocation.CompetitionId = competition.Id;
            sublocation.OwnerOdsCode = organisation.ExternalIdentifier;

            context.Add(sublocation);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var count = await service.GetCountForCompetitionSublocationRecipients(
                organisation.ExternalIdentifier,
                competition.Id,
                sublocation.SublocationOdsCode);

            Assert.Equal(expectedCount, count);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData("", 5, "MY-RECIPIENT-ID", true, typeof(ArgumentException))]
        [MockInMemoryDbInlineAutoData(null, 5, "MY-RECIPIENT-ID", true, typeof(ArgumentNullException))]
        [MockInMemoryDbInlineAutoData("MY-ORG-ID", 5, "", true, typeof(ArgumentException))]
        [MockInMemoryDbInlineAutoData("MY-ORG-ID", 5, null, true, typeof(ArgumentNullException))]
        [MockInMemoryDbInlineAutoData("MY-ORG-ID", 5, "MY-RECIPIENT-ID", false, typeof(ArgumentException))]
        public static async Task AddSublocationRecipients_RejectsNullArguments(
            string parentOdsCode,
            int competitionId,
            string sublocationOdsCode,
            bool populateOdsCodes,
            Type expectedExceptionType,
            HashSet<string> odsCodes,
            CompetitionSublocationService service)
        {
            Exception exception = await Record.ExceptionAsync(
                async () =>
                {
                    if (!populateOdsCodes)
                    {
                        await service.AddSublocationRecipients(
                            parentOdsCode,
                            competitionId,
                            sublocationOdsCode,
                            new HashSet<string>());
                    }
                    else
                    {
                        await service.AddSublocationRecipients(
                            parentOdsCode,
                            competitionId,
                            sublocationOdsCode,
                            odsCodes);
                    }
                });

            exception.Should().NotBeNull();
            exception!.GetType().Should().Be(expectedExceptionType);
        }

        private static IEnumerable<object[]> AddSublocationRecipientsNotValidData()
        {
            Competition completeCompetition = CommonCompetitionFactory(32, 45);
            completeCompetition.Completed = new DateTime(2024, 01, 03);

            return
            [
                [
                    CommonOrganisationFactory(45), completeCompetition,
                    CommonCompetitionSublocationFactory("XXXX", [], true, 32),
                    new HashSet<string> { "AAAA" }, "Cannot add recipients to sublocations on a completed competition.",
                ],
                [
                    CommonOrganisationFactory(75), CommonCompetitionFactory(23, 75),
                    CommonCompetitionSublocationFactory(
                        "XXXY",
                        [CommonCompetitionSublocationRecipientFactory("AAAA", "XXXY", 23)],
                        true,
                        23),
                    new HashSet<string> { "AAAA" }, "One or more requested Ids already present in sublocation.",
                ],
                [
                    CommonOrganisationFactory(61), CommonCompetitionFactory(78, 61),
                    CommonCompetitionSublocationFactory(
                        "XXXZ",
                        [],
                        true,
                        78),
                    new HashSet<string> { "AAAA" },
                    "One or more requested Ids not found or not valid for this sublocation.",
                ],
            ];
        }

        [Theory]
        [MockInMemoryDbMemberAutoData(nameof(AddSublocationRecipientsNotValidData))]
        public static async Task AddSublocationRecipients_RejectsInvalidOperations(
            Organisation organisation,
            Competition competition,
            CompetitionSublocation competitionSublocation,
            HashSet<string> recipientOdsCodes,
            string expectedMessage,
            [Frozen] IOdsService odsService,
            [Frozen] BuyingCatalogueDbContext context,
            CompetitionSublocationService service)
        {
            competition.Organisation = organisation;

            context.Add(competitionSublocation);
            context.Add(competition);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            odsService.GetServiceRecipientsBySublocation(competitionSublocation.SublocationOdsCode)
                .Returns([]);

            Exception exception = await Record.ExceptionAsync(
                async () =>
                {
                    await service.AddSublocationRecipients(
                        organisation.ExternalIdentifier,
                        competition.Id,
                        competitionSublocation.SublocationOdsCode,
                        recipientOdsCodes);
                });

            exception.Should().NotBeNull();
            exception!.GetType().Should().Be(typeof(InvalidOperationException));
            exception!.Message.Should().Be(expectedMessage);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddSublocationRecipients_AddsAsExpected(
            CompetitionSublocationService service)
        {
            Assert.Fail("not implemented");
        }

        [Theory]
        [MockInMemoryDbInlineAutoData("", 5, "MY-RECIPIENT-ID", true, typeof(ArgumentException))]
        [MockInMemoryDbInlineAutoData(null, 5, "MY-RECIPIENT-ID", true, typeof(ArgumentNullException))]
        [MockInMemoryDbInlineAutoData("MY-ORG-ID", 5, "", true, typeof(ArgumentException))]
        [MockInMemoryDbInlineAutoData("MY-ORG-ID", 5, null, true, typeof(ArgumentNullException))]
        [MockInMemoryDbInlineAutoData("MY-ORG-ID", 5, "MY-RECIPIENT-ID", false, typeof(ArgumentException))]
        public static async Task RemoveSublocationRecipients_RejectsNullArguments(
            string parentOdsCode,
            int competitionId,
            string sublocationOdsCode,
            bool populateOdsCodes,
            Type expectedExceptionType,
            HashSet<string> odsCodes,
            CompetitionSublocationService service)
        {
            Exception exception = await Record.ExceptionAsync(
                async () =>
                {
                    if (!populateOdsCodes)
                    {
                        await service.RemoveSublocationRecipients(
                            parentOdsCode,
                            competitionId,
                            sublocationOdsCode,
                            new HashSet<string>());
                    }
                    else
                    {
                        await service.RemoveSublocationRecipients(
                            parentOdsCode,
                            competitionId,
                            sublocationOdsCode,
                            odsCodes);
                    }
                });

            exception.Should().NotBeNull();
            exception!.GetType().Should().Be(expectedExceptionType);
        }

        private static IEnumerable<object[]> RemoveSublocationRecipientsNotValidData()
        {
            Competition completeCompetition = CommonCompetitionFactory(32, 45);
            completeCompetition.Completed = new DateTime(2024, 01, 03);

            return
            [
                [
                    CommonOrganisationFactory(45), completeCompetition,
                    CommonCompetitionSublocationFactory(
                        "XXXX",
                        [CommonCompetitionSublocationRecipientFactory("AAAA", "XXXX", 32, true)],
                        true,
                        32),
                    new HashSet<string> { "AAAA" },
                    "Cannot remove recipients from sublocations on a completed competition.",
                ],
                [
                    CommonOrganisationFactory(89), CommonCompetitionFactory(55, 89),
                    CommonCompetitionSublocationFactory("XXXY", [], true, 55),
                    new HashSet<string> { "AAAA" }, "Sublocation has no recipients to remove.",
                ],
                [
                    CommonOrganisationFactory(75), CommonCompetitionFactory(23, 75),
                    CommonCompetitionSublocationFactory(
                        "XXXZ",
                        [CommonCompetitionSublocationRecipientFactory("AAAA", "XXXZ", 23)],
                        true,
                        23),
                    new HashSet<string> { "AAZZ" }, "Can only remove recipient if present in sublocation.",
                ],
            ];
        }

        [Theory]
        [MockInMemoryDbMemberAutoData(nameof(RemoveSublocationRecipientsNotValidData))]
        public static async Task RemoveSublocationRecipients_RejectsInvalidOperations(
            Organisation organisation,
            Competition competition,
            CompetitionSublocation competitionSublocation,
            HashSet<string> recipientOdsCodes,
            string expectedMessage,
            [Frozen] IOdsService odsService,
            [Frozen] BuyingCatalogueDbContext context,
            CompetitionSublocationService service)
        {
            competition.Organisation = organisation;

            context.Add(competitionSublocation);
            context.Add(competition);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            odsService.GetServiceRecipientsBySublocation(competitionSublocation.SublocationOdsCode)
                .Returns([]);

            Exception exception = await Record.ExceptionAsync(
                async () =>
                {
                    await service.RemoveSublocationRecipients(
                        organisation.ExternalIdentifier,
                        competition.Id,
                        competitionSublocation.SublocationOdsCode,
                        recipientOdsCodes);
                });

            exception.Should().NotBeNull();
            exception!.GetType().Should().Be(typeof(InvalidOperationException));
            exception!.Message.Should().Be(expectedMessage);
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
            bool hasOrganisation = false,
            int customCompetitionId = 0)
        {
            return new CompetitionSublocation
            {
                CompetitionId = customCompetitionId == 0 ? CommonCompetitionId : customCompetitionId,
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
            int competitionId = 0,
            bool hasOrganisation = false)
        {
            return new CompetitionSublocationRecipient
            {
                CompetitionId = competitionId == 0 ? CommonCompetitionId : competitionId,
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
