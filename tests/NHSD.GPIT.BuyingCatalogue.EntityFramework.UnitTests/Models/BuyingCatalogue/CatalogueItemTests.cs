﻿using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.BuyingCatalogue
{
    public static class CatalogueItemTests
    {
        [Theory]
        [CommonAutoData]
        public static void Features_NullSolution_ReturnsNull(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;

            var actual = catalogueItem.Features();

            actual.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Features_NullFeatures_ReturnsNull(Solution solution)
        {
            solution.Features = null;

            var actual = solution.CatalogueItem.Features();

            actual.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Features_EmptyFeatures_ReturnsNull(Solution solution)
        {
            solution.Features = string.Empty;

            var actual = solution.CatalogueItem.Features();

            actual.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Features_SolutionHasValidFeatures_ReturnsFeatures(
            Solution solution,
            string[] expected)
        {
            solution.Features = JsonSerializer.Serialize(expected);

            var actual = solution.CatalogueItem.Features();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void FirstContact_ValidModel_ReturnsFirstContact(Solution solution)
        {
            var expected = solution.MarketingContacts.First();

            var actual = solution.CatalogueItem.FirstContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void FirstContact_NoContactsInSolution_ReturnsEmptyObject(Solution solution)
        {
            solution.MarketingContacts = null;
            var expected = new MarketingContact();

            var actual = solution.CatalogueItem.FirstContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void FirstContact_NullSolution_ReturnsEmptyObject(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;
            var expected = new MarketingContact();

            var actual = catalogueItem.FirstContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void SecondContact_ValidModel_ReturnsSecondContact(Solution solution)
        {
            var expected = solution.MarketingContacts.Skip(1).First();

            var actual = solution.CatalogueItem.SecondContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void SecondContact_NoContactsInSolution_ReturnsEmptyObject(Solution solution)
        {
            solution.MarketingContacts = null;
            var expected = new MarketingContact();

            var actual = solution.CatalogueItem.SecondContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void SecondContact_NullSolution_ReturnsEmptyObject(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;
            var expected = new MarketingContact();

            var actual = catalogueItem.SecondContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonInlineAutoData(PublicationStatus.Published, true)]
        [CommonInlineAutoData(PublicationStatus.InRemediation, true)]
        [CommonInlineAutoData(PublicationStatus.Draft, false)]
        [CommonInlineAutoData(PublicationStatus.Suspended, false)]
        [CommonInlineAutoData(PublicationStatus.Unpublished, false)]
        public static void IsBrowsable_WithPublicationStatus_ReturnsExpectedResult(
            PublicationStatus publicationStatus,
            bool expectedResult,
            CatalogueItem catalogueItem)
        {
            catalogueItem.PublishedStatus = publicationStatus;

            catalogueItem
                .IsBrowsable
                .Should()
                .Be(expectedResult);
        }
    }
}
