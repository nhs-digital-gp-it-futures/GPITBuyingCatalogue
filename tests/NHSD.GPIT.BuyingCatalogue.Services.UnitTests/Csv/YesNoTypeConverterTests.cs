using CsvHelper;
using CsvHelper.Configuration;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Services.Csv;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Csv;

public static class YesNoTypeConverterTests
{
    [Theory]
    [MockInlineAutoData("Yes", true)]
    [MockInlineAutoData("YES", true)]
    [MockInlineAutoData("yes", true)]
    [MockInlineAutoData("YeS", true)]
    [MockInlineAutoData("No", false)]
    [MockInlineAutoData("no", false)]
    [MockInlineAutoData("", false)]
    public static void ConvertFromString_ReturnsExpected(
        string text,
        bool expected,
        IReaderRow readerRow,
        MemberMapData memberMap,
        YesNoTypeConverter converter)
        => converter.ConvertFromString(text, readerRow, memberMap).Should().Be(expected);

    [Theory]
    [MockInlineAutoData(true, "Yes")]
    [MockInlineAutoData(false, "No")]
    public static void ConvertToString_ReturnsExpected(
        bool value,
        string expected,
        IWriterRow writerRow,
        MemberMapData memberMap,
        YesNoTypeConverter converter)
        => converter.ConvertToString(value, writerRow, memberMap).Should().Be(expected);
}
