using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
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
            var model = new Mock<SideNavigationBaseModel> { Object = { Sections = SectionModels } };

            var actual = model.Object.FirstSection;
            actual.Should().Be(SectionModels[0].Name);
        }

        [Fact]
        public static void FirstSection_NoItems_Returns_ExpectedResponse()
        {
            var model = new Mock<SideNavigationBaseModel> { Object = { Sections = new List<SectionModel>() } };

            var actual = model.Object.FirstSection;
            actual.Should().BeNull();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public static void SelectedSection_Returns_ExpectedResponse(int index)
        {
            var model = new Mock<SideNavigationBaseModel> { Object = { Sections = SectionModels, } };

            model.Setup(x => x.Index).Returns(index);
            var actual = model.Object.SelectedSection;
            actual.Should().Be(SectionModels[index].Name);
        }

        [Fact]
        public static void SelectedSection_IndexOutOfBoundsReturns_Null()
        {
            var model = new Mock<SideNavigationBaseModel> { Object = { Sections = SectionModels, } };
            model.Setup(x => x.Index).Returns(4);
            var actual = model.Object.SelectedSection;
            actual.Should().BeNull();
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        public static void NotFirstSection_Returns_ExpectedResponse(int index, bool expected)
        {
            var model = new Mock<SideNavigationBaseModel> { Object = { Sections = SectionModels, } };
            model.Setup(x => x.Index).Returns(index);
            var actual = model.Object.NotFirstSection;
            actual.Should().Be(expected);
        }

        [Fact]
        public static void SetPaginationFooter_Returns_ExpectedResponse()
        {
            const int index = 1;
            var model = new Mock<SideNavigationBaseModel> { Object = { Sections = SectionModels, } };
            model.Setup(x => x.Index).Returns(index);
            model.Object.SetPaginationFooter();

            model.Object.PaginationFooter.Previous.Should().Be(SectionModels[index - 1]);
            model.Object.PaginationFooter.Next.Should().Be(SectionModels[index + 1]);
        }

        [Fact]
        public static void SetPaginationFooter_FirstItem_Returns_ExpectedResponse()
        {
            var model = new Mock<SideNavigationBaseModel> { Object = { Sections = SectionModels, } };
            model.Setup(x => x.Index).Returns(0);
            model.Object.SetPaginationFooter();

            model.Object.PaginationFooter.Previous.Should().Be(null);
        }

        [Fact]
        public static void SetPaginationFooter_LastItem_Returns_ExpectedResponse()
        {
            var model = new Mock<SideNavigationBaseModel> { Object = { Sections = SectionModels, } };
            model.Setup(x => x.Index).Returns(2);
            model.Object.SetPaginationFooter();

            model.Object.PaginationFooter.Next.Should().Be(null);
        }
    }
}
