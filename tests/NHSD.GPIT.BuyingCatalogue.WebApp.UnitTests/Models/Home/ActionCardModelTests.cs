using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Home;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Home;

public static class ActionCardModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        string title,
        string text,
        string url)
    {
        var model = new ActionCardModel(title, text, url);

        model.Title.Should().Be(title);
        model.Text.Should().Be(text);
        model.Url.Should().Be(url);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SetsLinkTextAsExpected(
        string title,
        string text,
        string url,
        string linkText)
    {
        var model = new ActionCardModel(title, text, url, linkText);

        model.LinkText.Should().Be(linkText);
    }
}
