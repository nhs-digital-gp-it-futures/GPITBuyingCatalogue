﻿using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Order
{
    public static class FundingTypeDescriptionModelTest
    {
        [Theory]
        [MockInlineAutoData(new[] { OrderItemFundingType.MixedFunding })]
        [MockInlineAutoData(
            new[]
            {
                OrderItemFundingType.None,
                OrderItemFundingType.NoFundingRequired,
                OrderItemFundingType.LocalFunding,
                OrderItemFundingType.CentralFunding,
            })]
        [MockInlineAutoData(
            new[]
            {
                OrderItemFundingType.None,
                OrderItemFundingType.NoFundingRequired,
                OrderItemFundingType.LocalFundingOnly,
                OrderItemFundingType.CentralFunding,
            })]
        [MockInlineAutoData(
            new[]
            {
                OrderItemFundingType.None,
                OrderItemFundingType.NoFundingRequired,
                OrderItemFundingType.LocalFundingOnly,
                OrderItemFundingType.LocalFunding,
                OrderItemFundingType.CentralFunding,
            })]
        public static void Funding_Combination(OrderItemFundingType[] fundingTypes)
        {
            var sut = new FundingTypeDescriptionModel(fundingTypes);
            var result = sut.Value("NAME");
            result.Should().Be("This NAME is being paid for using a combination of central and local funding.");
        }

        [Theory]
        [MockInlineAutoData(new[] { OrderItemFundingType.LocalFunding })]
        [MockInlineAutoData(new[] { OrderItemFundingType.LocalFundingOnly })]
        [MockInlineAutoData(
            new[]
            {
                OrderItemFundingType.None,
                OrderItemFundingType.NoFundingRequired,
                OrderItemFundingType.LocalFunding,
            })]
        [MockInlineAutoData(
            new[]
            {
                OrderItemFundingType.None,
                OrderItemFundingType.NoFundingRequired,
                OrderItemFundingType.LocalFundingOnly,
            })]
        public static void Funding_LocalFunding(OrderItemFundingType[] fundingTypes)
        {
            var sut = new FundingTypeDescriptionModel(fundingTypes);
            var result = sut.Value("NAME");
            result.Should().Be("This NAME is being paid for using local funding.");
        }

        [Theory]
        [MockInlineAutoData(new[] { OrderItemFundingType.CentralFunding })]
        [MockInlineAutoData(new[] { OrderItemFundingType.None, OrderItemFundingType.NoFundingRequired, OrderItemFundingType.CentralFunding })]
        public static void Funding_CentralFunding(OrderItemFundingType[] fundingTypes)
        {
            var sut = new FundingTypeDescriptionModel(fundingTypes);
            var result = sut.Value("NAME");
            result.Should().Be("This NAME is being paid for using central funding.");
        }

        [Theory]
        [MockInlineAutoData(new[] { OrderItemFundingType.NoFundingRequired })]
        [MockInlineAutoData(
            new[]
            {
                OrderItemFundingType.None,
                OrderItemFundingType.NoFundingRequired,
            })]
        public static void Funding_NotRequired(OrderItemFundingType[] fundingTypes)
        {
            var sut = new FundingTypeDescriptionModel(fundingTypes);
            var result = sut.Value("NAME");
            result.Should().Be("This NAME does not require funding.");
        }

        [Theory]
        [MockInlineAutoData(new[] { OrderItemFundingType.None })]
        public static void Funding_NotEntered(OrderItemFundingType[] fundingTypes)
        {
            var sut = new FundingTypeDescriptionModel(fundingTypes);
            var result = sut.Value("NAME");
            result.Should().Be("Funding information has not been entered for this NAME.");
        }

        [Theory]
        [MockInlineAutoData(new[] { int.MaxValue })]
        public static void Funding_Unexpected(OrderItemFundingType[] fundingTypes)
        {
            var sut = new FundingTypeDescriptionModel(fundingTypes);
            sut.Invoking(s => s.Value("NAME"))
                .Should()
                .Throw<InvalidOperationException>();
        }
    }
}
