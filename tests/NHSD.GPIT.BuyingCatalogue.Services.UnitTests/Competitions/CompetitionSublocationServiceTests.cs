using System;
using System.Collections.Generic;
using System.Linq;
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

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Competitions
{
    public static class CompetitionSublocationServiceTests
    {
        private const int CommonCompetitionId = 34;
        private const int CommonOrganisationId = 21;
        private const string CommonOrganisationInternalIdentifier = "BB-FFGG";
        private const string CommonOrganisationExternalIdentifier = "FFGG";

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
        public static async Task SetSublocationRecipients_RejectsNullArguments(
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
                        await service.SetSublocationRecipients(
                            parentOdsCode,
                            competitionId,
                            sublocationOdsCode,
                            new HashSet<string>());
                    }
                    else
                    {
                        await service.SetSublocationRecipients(
                            parentOdsCode,
                            competitionId,
                            sublocationOdsCode,
                            odsCodes);
                    }
                });

            exception.Should().NotBeNull();
            exception!.GetType().Should().Be(expectedExceptionType);
        }

        public static IEnumerable<object[]> SetSublocationRecipientsNotValidData()
        {
            Competition completeCompetition = CommonCompetitionFactory(32, 45);
            completeCompetition.Completed = new DateTime(2024, 01, 03);

            return
            [
                [
                    CommonOrganisationFactory(45), completeCompetition,
                    CommonCompetitionSublocationFactory("XXXX", [], true, 32),
                    new HashSet<string> { "AAAA" }, "Cannot set sublocation recipients on a completed competition.",
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
        [MockInMemoryDbMemberAutoData(nameof(SetSublocationRecipientsNotValidData))]
        public static async Task SetSublocationRecipients_RejectsInvalidOperations(
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
                    await service.SetSublocationRecipients(
                        organisation.ExternalIdentifier,
                        competition.Id,
                        competitionSublocation.SublocationOdsCode,
                        recipientOdsCodes);
                });

            exception.Should().NotBeNull();
            exception!.GetType().Should().Be(typeof(InvalidOperationException));
            exception!.Message.Should().Be(expectedMessage);
        }

        public static IEnumerable<object[]> SetSublocationRecipientsData()
        {
            return
            [
                // Adds
                [
                    CommonOrganisationFactory(45), CommonCompetitionFactory(32, 45),
                    CommonCompetitionSublocationFactory("XXXX", [], true, 32),
                    new List<EntityOdsOrganisation>
                    {
                        CommonEntityOdsOrganisationFactory("AAAA"),
                        CommonEntityOdsOrganisationFactory("AAAB"),
                        CommonEntityOdsOrganisationFactory("AAAC"),
                    },
                    new List<ServiceRecipient>
                    {
                        CommonServiceRecipientFactory("AAAA", "XXXX"),
                        CommonServiceRecipientFactory("AAAB", "XXXX"),
                        CommonServiceRecipientFactory("AAAC", "XXXX"),
                    },
                    new HashSet<string> { "AAAA", "AAAB" },
                    CommonCompetitionSublocationFactory(
                        "XXXX",
                        [
                            CommonCompetitionSublocationRecipientFactory("AAAA", "XXXX", 32, true),
                            CommonCompetitionSublocationRecipientFactory("AAAB", "XXXX", 32, true),
                        ],
                        true,
                        32),
                ],

                // Removes
                [
                    CommonOrganisationFactory(75), CommonCompetitionFactory(14, 75),
                    CommonCompetitionSublocationFactory(
                        "YXXX",
                        [
                            CommonCompetitionSublocationRecipientFactory("BAAA", "YXXX", 14),
                            CommonCompetitionSublocationRecipientFactory("BAAB", "YXXX", 14),
                            CommonCompetitionSublocationRecipientFactory("BAAC", "YXXX", 14),
                        ],
                        true,
                        14),

                    new List<EntityOdsOrganisation>
                    {
                        CommonEntityOdsOrganisationFactory("BAAA"),
                        CommonEntityOdsOrganisationFactory("BAAB"),
                        CommonEntityOdsOrganisationFactory("BAAC"),
                    },
                    new List<ServiceRecipient>
                    {
                        CommonServiceRecipientFactory("BAAA", "YXXX"),
                        CommonServiceRecipientFactory("BAAB", "YXXX"),
                        CommonServiceRecipientFactory("BAAC", "YXXX"),
                    },
                    new HashSet<string> { "BAAA", "BAAB" },
                    CommonCompetitionSublocationFactory(
                        "YXXX",
                        [
                            CommonCompetitionSublocationRecipientFactory("BAAA", "YXXX", 14, true),
                            CommonCompetitionSublocationRecipientFactory("BAAB", "YXXX", 14, true),
                        ],
                        true,
                        14),
                ],

                // Adds and removes
                [
                    CommonOrganisationFactory(81), CommonCompetitionFactory(9, 81),
                    CommonCompetitionSublocationFactory(
                        "ZXXX",
                        [
                            CommonCompetitionSublocationRecipientFactory("CAAA", "ZXXX", 9),
                            CommonCompetitionSublocationRecipientFactory("CAAB", "ZXXX", 9),
                            CommonCompetitionSublocationRecipientFactory("CAAC", "ZXXX", 9),
                        ],
                        true,
                        9),

                    new List<EntityOdsOrganisation>
                    {
                        CommonEntityOdsOrganisationFactory("CAAA"),
                        CommonEntityOdsOrganisationFactory("CAAB"),
                        CommonEntityOdsOrganisationFactory("CAAC"),
                        CommonEntityOdsOrganisationFactory("CAAD"),
                        CommonEntityOdsOrganisationFactory("CAAE"),
                        CommonEntityOdsOrganisationFactory("CAAF"),
                    },
                    new List<ServiceRecipient>
                    {
                        CommonServiceRecipientFactory("CAAA", "ZXXX"),
                        CommonServiceRecipientFactory("CAAB", "ZXXX"),
                        CommonServiceRecipientFactory("CAAC", "ZXXX"),
                        CommonServiceRecipientFactory("CAAD", "ZXXX"),
                        CommonServiceRecipientFactory("CAAE", "ZXXX"),
                        CommonServiceRecipientFactory("CAAF", "ZXXX"),
                    },
                    new HashSet<string> { "CAAA", "CAAB", "CAAD", "CAAF" },
                    CommonCompetitionSublocationFactory(
                        "ZXXX",
                        [
                            CommonCompetitionSublocationRecipientFactory("CAAA", "ZXXX", 9, true),
                            CommonCompetitionSublocationRecipientFactory("CAAB", "ZXXX", 9, true),
                            CommonCompetitionSublocationRecipientFactory("CAAD", "ZXXX", 9, true),
                            CommonCompetitionSublocationRecipientFactory("CAAF", "ZXXX", 9, true),
                        ],
                        true,
                        9),
                ],
            ];
        }

        [Theory]
        [MockInMemoryDbMemberAutoData(nameof(SetSublocationRecipientsData))]
        public static async Task SetSublocationRecipients_SetsAsExpected(
            Organisation organisation,
            Competition competition,
            CompetitionSublocation workingCompetitionSublocation,
            List<EntityOdsOrganisation> validSublocationRecipientsAsEntityModels,
            List<ServiceRecipient> validSublocationRecipientsAsServiceModels,
            HashSet<string> addSublocationRecipientsOdsCodes,
            CompetitionSublocation expectedCompetitionSublocation,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IOdsService odsService,
            CompetitionSublocationService service)
        {
            competition.Organisation = organisation;

            context.AddRange(validSublocationRecipientsAsEntityModels);
            context.Add(workingCompetitionSublocation);
            context.Add(competition);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            odsService.GetServiceRecipientsBySublocation(workingCompetitionSublocation.SublocationOdsCode)
                .Returns(validSublocationRecipientsAsServiceModels);

            await service.SetSublocationRecipients(
                organisation.ExternalIdentifier,
                competition.Id,
                workingCompetitionSublocation.SublocationOdsCode,
                addSublocationRecipientsOdsCodes);

            CompetitionSublocation actualCompetitionSublocation = await service.GetCompetitionSublocationWithRecipients(
                organisation.ExternalIdentifier,
                competition.Id,
                workingCompetitionSublocation.SublocationOdsCode);

            actualCompetitionSublocation.Should()
                .BeEquivalentTo(
                    expectedCompetitionSublocation,
                    opt => opt.WithoutStrictOrdering()
                        .Excluding(m => m.Competition)
                        .Excluding(m => m.SublocationOrganisation)
                        .Excluding(m => m.SublocationRecipients));

            foreach (CompetitionSublocationRecipient expectedRecipient in expectedCompetitionSublocation
                         .SublocationRecipients)
            {
                CompetitionSublocationRecipient actualRecipient =
                    actualCompetitionSublocation.SublocationRecipients.First(
                        x => x.RecipientOdsCode == expectedRecipient.RecipientOdsCode);

                actualRecipient.Should()
                    .BeEquivalentTo(
                        expectedRecipient,
                        opt => opt.Excluding(m => m.Competition).Excluding(m => m.ParentSublocation));
            }
        }

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

        private static ServiceRecipient CommonServiceRecipientFactory(string orgId, string locationOrgId)
        {
            return new ServiceRecipient { OrgId = orgId, LocationOrgId = locationOrgId };
        }
    }
}
