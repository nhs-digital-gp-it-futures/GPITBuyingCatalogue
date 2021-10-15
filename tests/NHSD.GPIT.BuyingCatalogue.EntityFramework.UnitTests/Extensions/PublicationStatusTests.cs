using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Extensions
{
    public static class PublicationStatusTests
    {
        [Theory]
        [MemberData(nameof(SolutionPublicationStatusesTestCases))]
        public static void GetAvailableSolutionPublicationStatuses(
            PublicationStatus publicationStatus,
            IList<PublicationStatus> expectedPublicationStatuses)
        {
            var actualPublicationStatuses = publicationStatus.GetAvailablePublicationStatuses(CatalogueItemType.Solution);

            actualPublicationStatuses.Should().BeEquivalentTo(expectedPublicationStatuses);
        }

        [Theory]
        [MemberData(nameof(ServicesPublicationStatusesTestCases))]
        public static void GetAvailableAssociatedServicesPublicationStatuses(
            PublicationStatus publicationStatus,
            IList<PublicationStatus> expectedPublicationStatuses)
        {
            var actualPublicationStatuses = publicationStatus.GetAvailablePublicationStatuses(CatalogueItemType.AssociatedService);

            actualPublicationStatuses.Should().BeEquivalentTo(expectedPublicationStatuses);
        }

        [Theory]
        [MemberData(nameof(ServicesPublicationStatusesTestCases))]
        public static void GetAvailableAdditionalServicesPublicationStatuses(
            PublicationStatus publicationStatus,
            IList<PublicationStatus> expectedPublicationStatuses)
        {
            var actualPublicationStatuses = publicationStatus.GetAvailablePublicationStatuses(CatalogueItemType.AdditionalService);

            actualPublicationStatuses.Should().BeEquivalentTo(expectedPublicationStatuses);
        }

        public static IEnumerable<object[]> SolutionPublicationStatusesTestCases()
        {
            yield return new object[]
            {
                PublicationStatus.Draft,
                new List<PublicationStatus>(2)
                {
                    PublicationStatus.Draft,
                    PublicationStatus.Published,
                },
            };

            yield return new object[]
            {
                PublicationStatus.Draft,
                new List<PublicationStatus>(2)
                {
                    PublicationStatus.Draft,
                    PublicationStatus.Published,
                },
            };

            yield return new object[]
            {
                PublicationStatus.Published,
                new List<PublicationStatus>(3)
                {
                    PublicationStatus.Published,
                    PublicationStatus.InRemediation,
                    PublicationStatus.Unpublished,
                },
            };

            yield return new object[]
            {
                PublicationStatus.InRemediation,
                new List<PublicationStatus>(4)
                {
                    PublicationStatus.Published,
                    PublicationStatus.InRemediation,
                    PublicationStatus.Suspended,
                    PublicationStatus.Unpublished,
                },
            };

            yield return new object[]
            {
                PublicationStatus.Suspended,
                new List<PublicationStatus>(4)
                {
                    PublicationStatus.Published,
                    PublicationStatus.InRemediation,
                    PublicationStatus.Suspended,
                    PublicationStatus.Unpublished,
                },
            };

            yield return new object[]
            {
                PublicationStatus.Unpublished,
                new List<PublicationStatus>(2)
                {
                    PublicationStatus.Published,
                    PublicationStatus.Unpublished,
                },
            };
        }

        public static IEnumerable<object[]> ServicesPublicationStatusesTestCases()
        {
            yield return new object[]
            {
                PublicationStatus.Draft,
                new List<PublicationStatus>(2)
                {
                    PublicationStatus.Draft,
                    PublicationStatus.Published,
                },
            };

            yield return new object[]
            {
                PublicationStatus.Draft,
                new List<PublicationStatus>(2)
                {
                    PublicationStatus.Draft,
                    PublicationStatus.Published,
                },
            };

            yield return new object[]
            {
                PublicationStatus.Published,
                new List<PublicationStatus>(2)
                {
                    PublicationStatus.Published,
                    PublicationStatus.Unpublished,
                },
            };

            yield return new object[]
            {
                PublicationStatus.Unpublished,
                new List<PublicationStatus>(2)
                {
                    PublicationStatus.Published,
                    PublicationStatus.Unpublished,
                },
            };
        }
    }
}
