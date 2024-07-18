using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Home;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.Home;

public static class DashboardCardModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        string title,
        string text,
        string url)
    {
        var model = new DashboardCardModel(title, text, url);

        model.Title.Should().Be(title);
        model.Text.Should().Be(text);
        model.Url.Should().Be(url);
        model.HeadingSize.Should().Be(HeadingSize.Large);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SetsLinkTextAsExpected(
        string title,
        string text,
        string url,
        string linkText)
    {
        var model = new DashboardCardModel(title, text, url, linkText);

        model.LinkText.Should().Be(linkText);
        model.HeadingSize.Should().Be(HeadingSize.Large);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SetsHeadingSize(
        string title,
        string text,
        string url,
        string linkText,
        HeadingSize headingSize)
    {
        var model = new DashboardCardModel(title, text, url, linkText, headingSize);

        model.HeadingSize.Should().Be(headingSize);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SetsColumnWidth(
        string title,
        string text,
        string url,
        string linkText,
        ColumnWidth columnWidth)
    {
        var model = new DashboardCardModel(title, text, url, linkText, columnWidth);

        model.ColumnWidth.Should().Be(columnWidth);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SetsColumnWidthAndHeadingSize(
        string title,
        string text,
        string url,
        string linkText,
        HeadingSize headingSize,
        ColumnWidth columnWidth)
    {
        var model = new DashboardCardModel(title, text, url, linkText, headingSize, columnWidth);

        model.HeadingSize.Should().Be(headingSize);
        model.ColumnWidth.Should().Be(columnWidth);
    }
}
