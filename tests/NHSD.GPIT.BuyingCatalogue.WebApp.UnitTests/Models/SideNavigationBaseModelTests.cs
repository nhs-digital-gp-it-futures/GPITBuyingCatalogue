using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models
{
    public static class SideNavigationBaseModelTests
    {
        private static readonly IList<SectionModel> SectionModels = new List<SectionModel>
        {
            new()
            {
                Action = "Action One",
                Controller = "Controller One",
                RouteData = new Dictionary<string, string>() { { "Key", "Value" }, },
                Name = "Section One",
            },
            new()
            {
                Action = "Action Two",
                Controller = "Controller Two",
                RouteData = new Dictionary<string, string>() { { "Key", "Value" }, },
                Name = "Section Two",
            },
            new()
            {
                Action = "Action Three",
                Controller = "Controller Three",
                RouteData = new Dictionary<string, string>() { { "Key", "Value" }, },
                Name = "Section Three",
            },
        };

        [Fact]
        public static void FirstSection_Returns_ExpectedResponse()
        {
            var model = Substitute.For<SideNavigationBaseModel>();
            model.Sections = SectionModels;

            var actual = model.FirstSection;
            actual.Should().Be(SectionModels[0].Name);
        }

        [Fact]
        public static void FirstSection_NoItems_Returns_ExpectedResponse()
        {
            var model = Substitute.For<SideNavigationBaseModel>();
            model.Sections = new List<SectionModel>();

            model.FirstSection.Should().BeNull();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public static void SelectedSection_Returns_ExpectedResponse(int index)
        {
            var model = Substitute.For<SideNavigationBaseModel>();
            model.Sections = SectionModels;

            model.Index.Returns(index);

            model.SelectedSection.Should().Be(SectionModels[index].Name);
        }

        [Fact]
        public static void SelectedSection_IndexOutOfBoundsReturns_Null()
        {
            var model = Substitute.For<SideNavigationBaseModel>();
            model.Sections = SectionModels;

            model.Index.Returns(4);

            model.SelectedSection.Should().BeNull();
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        public static void NotFirstSection_Returns_ExpectedResponse(int index, bool expected)
        {
            var model = Substitute.For<SideNavigationBaseModel>();
            model.Sections = SectionModels;

            model.Index.Returns(index);

            model.NotFirstSection.Should().Be(expected);
        }

        [Fact]
        public static void SetPaginationFooter_Returns_ExpectedResponse()
        {
            const int index = 1;
            var model = Substitute.For<SideNavigationBaseModel>();
            model.Sections = SectionModels;
            model.Index.Returns(index);
            model.SetPaginationFooter();

            model.PaginationFooter.Previous.Should().Be(SectionModels[index - 1]);
            model.PaginationFooter.Next.Should().Be(SectionModels[index + 1]);
        }

        [Fact]
        public static void SetPaginationFooter_FirstItem_Returns_ExpectedResponse()
        {
            var model = Substitute.For<SideNavigationBaseModel>();
            model.Sections = SectionModels;
            model.Index.Returns(0);
            model.SetPaginationFooter();

            model.PaginationFooter.Previous.Should().Be(null);
        }

        [Fact]
        public static void SetPaginationFooter_LastItem_Returns_ExpectedResponse()
        {
            var model = Substitute.For<SideNavigationBaseModel>();
            model.Sections = SectionModels;
            model.Index.Returns(2);
            model.SetPaginationFooter();

            model.PaginationFooter.Next.Should().Be(null);
        }
    }
}
