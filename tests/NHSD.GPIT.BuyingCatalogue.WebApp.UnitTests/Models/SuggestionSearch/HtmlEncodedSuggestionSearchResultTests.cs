using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models;

public static class HtmlEncodedSuggestionSearchResultTests
{
    [Theory]
    [MockAutoData]
    public static void Title_And_Category_Are_HTML_Encoded(string url)
    {
        var model = new HtmlEncodedSuggestionSearchResult("<Title>", "<Category>", url);

        model.Title.Should().Be("&lt;Title&gt;");
        model.Category.Should().Be("&lt;Category&gt;");
        model.Url.Should().Be(url);
    }
}
