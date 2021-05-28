using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.BuyingCatalogue
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class CatalogueItemTests
    {
        [Test, CommonAutoData]
        public static void FirstContact_ValidModel_ReturnsFirstContact(CatalogueItem catalogueItem)
        {
            var expected = catalogueItem.Solution.MarketingContacts.First();

            var actual = catalogueItem.FirstContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Test, CommonAutoData]
        public static void FirstContact_NoContactsInSolution_ReturnsEmptyObject(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.MarketingContacts = null;
            var expected = new MarketingContact();

            var actual = catalogueItem.FirstContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Test, CommonAutoData]
        public static void FirstContact_NullSolution_ReturnsEmptyObject(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;
            var expected = new MarketingContact();

            var actual = catalogueItem.FirstContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Test, CommonAutoData]
        public static void Framework_SolutionHasValidFrameworkSet_ReturnsFramework(
            CatalogueItem catalogueItem,
            string expected)
        {
            catalogueItem.Solution.FrameworkSolutions.First().Framework.Name = expected;

            var actual = catalogueItem.Framework();

            actual.Should().Be(expected);
        }

        [Test, CommonAutoData]
        public static void Framework_SolutionHasNullFramework_ReturnsNull(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.FrameworkSolutions.First().Framework = null;

            var actual = catalogueItem.Framework();

            actual.Should().BeNull();
        }

        [Test, CommonAutoData]
        public static void Framework_FrameworkSolutionNull_ReturnsNull(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.FrameworkSolutions = null;

            var actual = catalogueItem.Framework();

            actual.Should().BeNull();
        }

        [Test, CommonAutoData]
        public static void Framework_SolutionNull_ReturnsNull(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;

            var actual = catalogueItem.Framework();

            actual.Should().BeNull();
        }

        [Test, CommonAutoData]
        public static void IsFoundation_OneSolutionIsFoundation_ReturnsTrue(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.FrameworkSolutions = new List<FrameworkSolution> { new() { IsFoundation = true } };

            var actual = catalogueItem.IsFoundation();

            actual.Should().BeTrue();
        }

        [Test, CommonAutoData]
        public static void IsFoundation_NoSolutionIsFoundation_ReturnsFalse(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.FrameworkSolutions.Should().NotBeEmpty();
            catalogueItem.Solution.FrameworkSolutions.ToList().ForEach(f => f.IsFoundation = false);

            var actual = catalogueItem.IsFoundation();

            actual.Should().BeFalse();
        }

        [Test, CommonAutoData]
        public static void IsFoundation_NullFrameworkSolutions_ReturnsNull(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.FrameworkSolutions = null;

            var actual = catalogueItem.IsFoundation();

            actual.Should().BeNull();
        }

        [Test, CommonAutoData]
        public static void IsFoundation_NullSolution_ReturnsNull(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.FrameworkSolutions = null;

            var actual = catalogueItem.IsFoundation();

            actual.Should().BeNull();
        }

        [Test, CommonAutoData]
        public static void SecondContact_ValidModel_ReturnsSecondContact(CatalogueItem catalogueItem)
        {
            var expected = catalogueItem.Solution.MarketingContacts.Skip(1).First();

            var actual = catalogueItem.SecondContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Test, CommonAutoData]
        public static void SecondContact_NoContactsInSolution_ReturnsEmptyObject(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution.MarketingContacts = null;
            var expected = new MarketingContact();

            var actual = catalogueItem.SecondContact();

            actual.Should().BeEquivalentTo(expected);
        }

        [Test, CommonAutoData]
        public static void SecondContact_NullSolution_ReturnsEmptyObject(CatalogueItem catalogueItem)
        {
            catalogueItem.Solution = null;
            var expected = new MarketingContact();

            var actual = catalogueItem.SecondContact();

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
