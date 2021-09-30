using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Extensions
{
    public static class PublicationStatusTests
    {
        [Theory]
        [CommomMemberAutoData(nameof(PublicationStatusesTestCases))]
        public static void GetAvailablePublicationStatuses(
            PublicationStatus publicationStatus,
            IList<PublicationStatus> expectedPublicationStatuses)
        {
            var actualPublicationStatuses = publicationStatus.GetAvailablePublicationStatuses();

            actualPublicationStatuses.Should().BeEquivalentTo(expectedPublicationStatuses);
        }

        public static IEnumerable<object[]> PublicationStatusesTestCases()
            => new object[][]
            {
                new object[]
                {
                    PublicationStatus.Draft,
                    new List<PublicationStatus>(2)
                    {
                       PublicationStatus.Draft,
                       PublicationStatus.Published,
                    },
                },
                new object[]
                {
                    PublicationStatus.Published,
                    new List<PublicationStatus>(3)
                    {
                       PublicationStatus.Published,
                       PublicationStatus.InRemediation,
                       PublicationStatus.Unpublished,
                    },
                },
                new object[]
                {
                    PublicationStatus.InRemediation,
                    new List<PublicationStatus>(4)
                    {
                       PublicationStatus.Published,
                       PublicationStatus.InRemediation,
                       PublicationStatus.Suspended,
                       PublicationStatus.Unpublished,
                    },
                },
                new object[]
                {
                    PublicationStatus.Suspended,
                    new List<PublicationStatus>(4)
                    {
                       PublicationStatus.Published,
                       PublicationStatus.InRemediation,
                       PublicationStatus.Suspended,
                       PublicationStatus.Unpublished,
                    },
                },
                new object[]
                {
                    PublicationStatus.Unpublished,
                    new List<PublicationStatus>(2)
                    {
                       PublicationStatus.Published,
                       PublicationStatus.Unpublished,
                    },
                },
            };
    }
}
