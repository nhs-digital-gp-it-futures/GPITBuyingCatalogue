using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
        public static void EndDate_Required_For_RemainingTermInMonths()
        {
            var maximumTerm = 36;
            var commencementDate = (DateTime?)null;
            var endDate = new EndDate(commencementDate, maximumTerm);
            endDate.Invoking(e => e.RemainingTermInMonths(DateTime.Now))
                .Should()
                .Throw<InvalidOperationException>();
        }

        [Fact]
        public static void EndDate_Required_For_RemainingDays()
        {
            var maximumTerm = 36;
            var commencementDate = (DateTime?)null;
            var endDate = new EndDate(commencementDate, maximumTerm);
            endDate.Invoking(e => e.RemainingDays(DateTime.Now))
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

            endDate.RemainingTermInMonths(new DateTime(year, month, day)).Should().Be(remainingTerm);
        }

        [Theory]
        [MockInlineAutoData(2000, 1, 1, 12)]
        [MockInlineAutoData(2000, 12, 1, 1)]
        [MockInlineAutoData(2000, 12, 31, 1)]
        [MockInlineAutoData(2000, 7, 1, 6)]
        [MockInlineAutoData(2000, 7, 30, 6)]
        [MockInlineAutoData(2001, 1, 1, 0)]
        public static void RemainingTerm_Start_Of_Month(int year, int month, int day, int remainingTerm)
        {
            var maximumTerm = 12;
            var commencementDate = new DateTime(2000, 1, 1);
            var endDate = new EndDate(commencementDate, maximumTerm);

            endDate.RemainingTermInMonths(new DateTime(year, month, day)).Should().Be(remainingTerm);
        }

        [Theory]
        [MockInlineAutoData(2000, 2, 1, 0)]
        [MockInlineAutoData(2000, 1, 31, 0)]
        [MockInlineAutoData(2000, 1, 30, 1)]
        public static void RemainingDays(int year, int month, int day, int remainingDays)
        {
            var maximumTerm = 1;
            var commencementDate = new DateTime(2000, 1, 1);
            var endDate = new EndDate(commencementDate, maximumTerm);

            endDate.RemainingDays(new DateTime(year, month, day)).Should().Be(remainingDays);
        }

        [Theory]
        [MockInlineAutoData(2000, 2, 1, 0, EventTypeEnum.Nothing)]
        [MockInlineAutoData(2000, 1, 31, 0, EventTypeEnum.Nothing)]
        [MockInlineAutoData(2000, 1, 30, 1, EventTypeEnum.OrderEnteredSecondExpiryThreshold)]
        [MockInlineAutoData(2000, 1, 17, 14, EventTypeEnum.OrderEnteredSecondExpiryThreshold)]
        [MockInlineAutoData(2000, 1, 16, 15, EventTypeEnum.OrderEnteredFirstExpiryThreshold)]
        [MockInlineAutoData(2000, 1, 1, 30, EventTypeEnum.OrderEnteredFirstExpiryThreshold)]
        [MockInlineAutoData(1999, 12, 31, 31, EventTypeEnum.Nothing)]
        public static void RemainingDays_ShortTerm_DetermineEventToRaise(
            int year,
            int month,
            int day,
            int remainingDays,
            EventTypeEnum eventTypeEnum)
        {
            var maximumTerm = 1;
            var commencementDate = new DateTime(2000, 1, 1);
            var endDate = new EndDate(commencementDate, maximumTerm);

            endDate.RemainingDays(new DateTime(year, month, day)).Should().Be(remainingDays);
            endDate.DetermineEventToRaise(new DateTime(year, month, day), new List<OrderEvent>()).Should().Be(eventTypeEnum);
        }

        [Theory]
        [MockInlineAutoData(2000, 4, 1, 0, EventTypeEnum.Nothing)]
        [MockInlineAutoData(2000, 3, 31, 0, EventTypeEnum.Nothing)]
        [MockInlineAutoData(2000, 3, 30, 1, EventTypeEnum.OrderEnteredSecondExpiryThreshold)]
        [MockInlineAutoData(2000, 2, 15, 45, EventTypeEnum.OrderEnteredSecondExpiryThreshold)]
        [MockInlineAutoData(2000, 2, 14, 46, EventTypeEnum.OrderEnteredFirstExpiryThreshold)]
        [MockInlineAutoData(2000, 1, 1, 90, EventTypeEnum.OrderEnteredFirstExpiryThreshold)]
        [MockInlineAutoData(1999, 12, 31, 91, EventTypeEnum.Nothing)]
        public static void RemainingDays_LongTerm_DetermineEventToRaise(
            int year,
            int month,
            int day,
            int remainingDays,
            EventTypeEnum eventTypeEnum)
        {
            var maximumTerm = 3;
            var commencementDate = new DateTime(2000, 1, 1);
            var endDate = new EndDate(commencementDate, maximumTerm);

            endDate.RemainingDays(new DateTime(year, month, day)).Should().Be(remainingDays);
            endDate.DetermineEventToRaise(new DateTime(year, month, day), new List<OrderEvent>()).Should().Be(eventTypeEnum);
        }

        [Theory]
        [MockInlineAutoData(2000, 2, 1, 0)]
        [MockInlineAutoData(2000, 1, 31, 0)]
        [MockInlineAutoData(2000, 1, 30, 1)]
        [MockInlineAutoData(2000, 1, 17, 14)]
        [MockInlineAutoData(2000, 1, 16, 15)]
        [MockInlineAutoData(2000, 1, 1, 30)]
        [MockInlineAutoData(1999, 12, 31, 31)]
        public static void RemainingDays_DetermineEventToRaise_Suppress(
            int year,
            int month,
            int day,
            int remainingDays,
            OrderEvent orderEvent1,
            OrderEvent orderEvent2)
        {
            var maximumTerm = 1;
            var commencementDate = new DateTime(2000, 1, 1);
            var endDate = new EndDate(commencementDate, maximumTerm);

            orderEvent1.EventTypeId = (int)EventTypeEnum.OrderEnteredFirstExpiryThreshold;
            orderEvent2.EventTypeId = (int)EventTypeEnum.OrderEnteredSecondExpiryThreshold;

            var orderEvents = new List<OrderEvent>
            {
                orderEvent1,
                orderEvent2,
            };

            endDate.RemainingDays(new DateTime(year, month, day)).Should().Be(remainingDays);
            endDate.DetermineEventToRaise(new DateTime(year, month, day), orderEvents).Should().Be(EventTypeEnum.Nothing);
        }
    }
}
