using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Ordering
{
    public static class EndDateTest
    {
        [Fact]
        public static void With_Null_CommencementDate_Has_EndDate_Value_Null()
        {
            var maximumTerm = 36;
            var commencementDate = (DateTime?)null;
            var endDate = new EndDate(commencementDate, maximumTerm);
            endDate.DateTime.Should().BeNull();
            endDate.DisplayValue.Should().BeEmpty();
        }

        [Fact]
        public static void EndDate_Required_For_RemainingTerm()
        {
            var maximumTerm = 36;
            var commencementDate = (DateTime?)null;
            var endDate = new EndDate(commencementDate, maximumTerm);
            endDate.Invoking(e => e.RemainingTerm(DateTime.Now))
                .Should()
                .Throw<InvalidOperationException>();
        }

        [Fact]
        public static void EndDate_Uses_MaximumTerm()
        {
            var maximumTerm = 36;
            var commencementDate = new DateTime(2000, 2, 2);
            var endDate = new EndDate(commencementDate, maximumTerm);
            endDate.DateTime.Should().Be(new DateTime(2003, 2, 1));
            endDate.DisplayValue.Should().Be("1 February 2003");
        }

        [Theory]
        [InlineAutoData(2000, 2, 24, 12)]
        [InlineAutoData(2000, 2, 25, 12)]
        [InlineAutoData(2000, 2, 28, 12)]
        [InlineAutoData(2000, 3, 1, 11)]
        [InlineAutoData(2000, 3, 24, 11)]
        [InlineAutoData(2000, 3, 25, 11)]
        [InlineAutoData(2001, 1, 30, 1)]
        [InlineAutoData(2001, 2, 1, 0)]
        [InlineAutoData(2001, 2, 23, 0)]
        [InlineAutoData(2001, 2, 24, 0)]
        public static void RemainingTerm_Starting_Mid_Month(int year, int month, int day, int remainingTerm)
        {
            var maximumTerm = 12;
            var commencementDate = new DateTime(2000, 2, 24);
            var endDate = new EndDate(commencementDate, maximumTerm);

            endDate.RemainingTerm(new DateTime(year, month, day)).Should().Be(remainingTerm);
        }

        [Theory]
        [InlineAutoData(2000, 1, 1, 12)]
        [InlineAutoData(2000, 12, 1, 1)]
        [InlineAutoData(2000, 12, 31, 1)]
        [InlineAutoData(2000, 7, 1, 6)]
        [InlineAutoData(2000, 7, 30, 6)]
        [InlineAutoData(2001, 1, 1, 0)]
        public static void RemainingTerm_Start_Of_Month(int year, int month, int day, int remainingTerm)
        {
            var maximumTerm = 12;
            var commencementDate = new DateTime(2000, 1, 1);
            var endDate = new EndDate(commencementDate, maximumTerm);

            endDate.RemainingTerm(new DateTime(year, month, day)).Should().Be(remainingTerm);
        }
    }
}
