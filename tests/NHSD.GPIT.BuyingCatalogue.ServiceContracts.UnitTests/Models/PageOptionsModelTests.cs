using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Models
{
    public static class PageOptionsModelTests
    {
        [Fact]
        public static void PageOptions_DefaultConstructor_DefaultValues()
        {
            var pageOptions = new PageOptions();

            pageOptions.PageNumber.Should().Be(1);
            pageOptions.PageSize.Should().Be(30);
            pageOptions.Sort.Should().Be(PageOptions.SortOptions.AtoZ);
            pageOptions.NumberOfPages.Should().Be(0);
            pageOptions.TotalNumberOfItems.Should().Be(0);
        }

        [Theory]
        [InlineData("", "", 1, PageOptions.SortOptions.AtoZ)]
        [InlineData("2", "", 2, PageOptions.SortOptions.AtoZ)]
        [InlineData("", "alphabetical", 1, PageOptions.SortOptions.AtoZ)]
        [InlineData("2", "lastpublished", 2, PageOptions.SortOptions.LastPublished)]
        [InlineData("IncorrectDate", "IncorrectDate", 1, PageOptions.SortOptions.AtoZ)]
        public static void PageOptions_ValuesInConstructor_ExpectedValues(string page, string sortBy, int expectedPage, PageOptions.SortOptions expectedSort)
        {
            var pageOptions = new PageOptions(page, sortBy);

            pageOptions.PageNumber.Should().Be(expectedPage);
            pageOptions.Sort.Should().Be(expectedSort);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(20, 1)]
        [InlineData(31, 2)]
        [InlineData(-1, 0)]
        public static void PageOptions_NumberOfItemsSet_ExpectedNumberOfPages(int numberOfItems, int expectedResult)
        {
            var pageOptions = new PageOptions
            {
                TotalNumberOfItems = numberOfItems,
            };

            pageOptions.NumberOfPages.Should().Be(expectedResult);
        }
    }
}
