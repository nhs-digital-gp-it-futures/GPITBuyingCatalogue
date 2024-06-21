using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Services.FundingTypes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.FundingTypes
{
    public class FundingTypeServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(FundingTypeService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static void GetFundingType_Central_AllOrderItemsCentral_ReturnsCentral(
            FundingTypeService service)
        {
            var fundingTypes = new List<OrderItemFundingType>
            {
                OrderItemFundingType.CentralFunding,
            };

            var result = service.GetFundingType(fundingTypes, OrderItemFundingType.CentralFunding);

            result.Should().Be(OrderItemFundingType.CentralFunding);
        }

        [Theory]
        [MockInlineAutoData(OrderItemFundingType.LocalFunding)]
        [MockInlineAutoData(OrderItemFundingType.LocalFundingOnly)]
        public static void GetFundingType_Local_AllOrderItemsLocal_ReturnsLocal(
            OrderItemFundingType fundingType,
            FundingTypeService service)
        {
            var fundingTypes = new List<OrderItemFundingType>
            {
                OrderItemFundingType.LocalFunding,
                OrderItemFundingType.LocalFundingOnly,
            };

            var result = service.GetFundingType(fundingTypes, fundingType);

            result.Should().Be(OrderItemFundingType.LocalFunding);
        }

        [Theory]
        [MockAutoData]
        public static void GetFundingType_Mixed_AllOrderItemsMixed_ReturnsCentral(
            FundingTypeService service)
        {
            var fundingTypes = new List<OrderItemFundingType>
            {
                OrderItemFundingType.MixedFunding,
            };

            var result = service.GetFundingType(fundingTypes, OrderItemFundingType.MixedFunding);

            result.Should().Be(OrderItemFundingType.CentralFunding);
        }

        [Theory]
        [MockAutoData]
        public static void GetFundingType_GPIT_AllOrderItemsGPIT_ReturnsGPIT(
            FundingTypeService service)
        {
            var fundingTypes = new List<OrderItemFundingType>
            {
                OrderItemFundingType.Gpit,
            };

            var result = service.GetFundingType(fundingTypes, OrderItemFundingType.Gpit);

            result.Should().Be(OrderItemFundingType.Gpit);
        }

        [Theory]
        [MockAutoData]
        public static void GetFundingType_GPIT_ReturnsGPIT(
            FundingTypeService service)
        {
            var fundingTypes = new List<OrderItemFundingType>
            {
                OrderItemFundingType.Gpit,
                OrderItemFundingType.LocalFunding,
                OrderItemFundingType.None,
                OrderItemFundingType.Pcarp,
            };

            var result = service.GetFundingType(fundingTypes, OrderItemFundingType.Gpit);

            result.Should().Be(OrderItemFundingType.Gpit);
        }

        [Theory]
        [MockAutoData]
        public static void GetFundingType_PCARP_AllOrderItemsPCARP_ReturnsPCARP(
            FundingTypeService service)
        {
            var fundingTypes = new List<OrderItemFundingType>
            {
                OrderItemFundingType.Pcarp,
            };

            var result = service.GetFundingType(fundingTypes, OrderItemFundingType.Pcarp);

            result.Should().Be(OrderItemFundingType.Pcarp);
        }

        [Theory]
        [MockAutoData]
        public static void GetFundingType_PCARP_ReturnsPCARP(
            FundingTypeService service)
        {
            var fundingTypes = new List<OrderItemFundingType>
            {
                OrderItemFundingType.Gpit,
                OrderItemFundingType.LocalFunding,
                OrderItemFundingType.None,
                OrderItemFundingType.Pcarp,
            };

            var result = service.GetFundingType(fundingTypes, OrderItemFundingType.Pcarp);

            result.Should().Be(OrderItemFundingType.Pcarp);
        }

        [Theory]
        [MockInlineAutoData(OrderItemFundingType.CentralFunding)]
        [MockInlineAutoData(OrderItemFundingType.MixedFunding)]
        public static void GetFundingType_MixtureOfCentralAndMixedOrderItems_ReturnsCentral(
            OrderItemFundingType fundingType,
            FundingTypeService service)
        {
            var fundingTypes = new List<OrderItemFundingType>
            {
                OrderItemFundingType.CentralFunding,
                OrderItemFundingType.MixedFunding,
            };

            var result = service.GetFundingType(fundingTypes, fundingType);

            result.Should().Be(OrderItemFundingType.CentralFunding);
        }

        [Theory]
        [MockInlineAutoData(OrderItemFundingType.LocalFunding)]
        [MockInlineAutoData(OrderItemFundingType.LocalFundingOnly)]
        [MockInlineAutoData(OrderItemFundingType.MixedFunding)]
        public static void GetFundingType_MixtureOfLocalAndMixedOrderItems_ReturnsLocal(
            OrderItemFundingType fundingType,
            FundingTypeService service)
        {
            var fundingTypes = new List<OrderItemFundingType>
            {
                OrderItemFundingType.LocalFunding,
                OrderItemFundingType.LocalFundingOnly,
                OrderItemFundingType.MixedFunding,
            };

            var result = service.GetFundingType(fundingTypes, fundingType);

            result.Should().Be(OrderItemFundingType.LocalFunding);
        }

        [Theory]
        [MockInlineAutoData(OrderItemFundingType.CentralFunding)]
        [MockInlineAutoData(OrderItemFundingType.NoFundingRequired)]
        [MockInlineAutoData(OrderItemFundingType.None)]
        public static void GetFundingType_MixtureOfCentralAndNotFundedOrderItems_ReturnsCentral(
            OrderItemFundingType fundingType,
            FundingTypeService service)
        {
            var fundingTypes = new List<OrderItemFundingType>
            {
                OrderItemFundingType.CentralFunding,
                OrderItemFundingType.NoFundingRequired,
                OrderItemFundingType.None,
            };

            var result = service.GetFundingType(fundingTypes, fundingType);

            result.Should().Be(OrderItemFundingType.CentralFunding);
        }

        [Theory]
        [MockInlineAutoData(OrderItemFundingType.LocalFunding)]
        [MockInlineAutoData(OrderItemFundingType.LocalFundingOnly)]
        public static void GetFundingType_MixtureOfLocalAndNotFundedOrderItems_ReturnsLocal(
            OrderItemFundingType fundingType,
            FundingTypeService service)
        {
            var fundingTypes = new List<OrderItemFundingType>
            {
                OrderItemFundingType.LocalFunding,
                OrderItemFundingType.LocalFundingOnly,
                OrderItemFundingType.NoFundingRequired,
                OrderItemFundingType.None,
            };

            var result = service.GetFundingType(fundingTypes, fundingType);

            result.Should().Be(OrderItemFundingType.LocalFunding);
        }

        [Theory]
        [MockInlineAutoData(OrderItemFundingType.CentralFunding, OrderItemFundingType.CentralFunding)]
        [MockInlineAutoData(OrderItemFundingType.LocalFunding, OrderItemFundingType.LocalFunding)]
        [MockInlineAutoData(OrderItemFundingType.LocalFundingOnly, OrderItemFundingType.LocalFunding)]
        [MockInlineAutoData(OrderItemFundingType.NoFundingRequired, OrderItemFundingType.CentralFunding)]
        [MockInlineAutoData(OrderItemFundingType.None, OrderItemFundingType.CentralFunding)]
        public static void GetFundingType_MixtureOfCentralAndLocalAndNotFundedOrderItems_ReturnsExpectedResult(
            OrderItemFundingType input,
            OrderItemFundingType output,
            FundingTypeService service)
        {
            var fundingTypes = new List<OrderItemFundingType>
            {
                OrderItemFundingType.CentralFunding,
                OrderItemFundingType.LocalFunding,
                OrderItemFundingType.LocalFundingOnly,
                OrderItemFundingType.NoFundingRequired,
                OrderItemFundingType.None,
            };

            var result = service.GetFundingType(fundingTypes, input);

            result.Should().Be(output);
        }

        [Theory]
        [MockInlineAutoData(OrderItemFundingType.NoFundingRequired)]
        [MockInlineAutoData(OrderItemFundingType.None)]
        public static void GetFundingType_NoFundingRequiredOnly_ReturnsNone(
            OrderItemFundingType fundingType,
            FundingTypeService service)
        {
            var fundingTypes = new List<OrderItemFundingType>
            {
                OrderItemFundingType.NoFundingRequired,
                OrderItemFundingType.None,
            };

            var result = service.GetFundingType(fundingTypes, fundingType);

            result.Should().Be(OrderItemFundingType.None);
        }

        [Theory]
        [MockInlineAutoData(OrderItemFundingType.NoFundingRequired)]
        [MockInlineAutoData(OrderItemFundingType.None)]
        public static void GetFundingType_MixtureOfNewAndExistingFundingTypes_NoFunding_ReturnsNone(
            OrderItemFundingType fundingType,
            FundingTypeService service)
        {
            var fundingTypes = new List<OrderItemFundingType>
            {
                OrderItemFundingType.Pcarp,
                OrderItemFundingType.Gpit,
                OrderItemFundingType.NoFundingRequired,
                OrderItemFundingType.None,
            };

            var result = service.GetFundingType(fundingTypes, fundingType);

            result.Should().Be(OrderItemFundingType.None);
        }

        [Theory]
        [MockInlineAutoData(OrderItemFundingType.CentralFunding, OrderItemFundingType.CentralFunding)]
        [MockInlineAutoData(OrderItemFundingType.LocalFunding, OrderItemFundingType.LocalFunding)]
        [MockInlineAutoData(OrderItemFundingType.LocalFundingOnly, OrderItemFundingType.LocalFunding)]
        public static void GetFundingType_MixtureOfCentralAndLocalOrderItems_ReturnsExpectedResult(
            OrderItemFundingType input,
            OrderItemFundingType output,
            FundingTypeService service)
        {
            var fundingTypes = new List<OrderItemFundingType>
            {
                OrderItemFundingType.CentralFunding,
                OrderItemFundingType.LocalFunding,
                OrderItemFundingType.LocalFundingOnly,
            };

            var result = service.GetFundingType(fundingTypes, input);

            result.Should().Be(output);
        }

        [Theory]
        [MockInlineAutoData(OrderItemFundingType.CentralFunding, OrderItemFundingType.CentralFunding)]
        [MockInlineAutoData(OrderItemFundingType.LocalFunding, OrderItemFundingType.LocalFunding)]
        [MockInlineAutoData(OrderItemFundingType.LocalFundingOnly, OrderItemFundingType.LocalFunding)]
        [MockInlineAutoData(OrderItemFundingType.MixedFunding, OrderItemFundingType.CentralFunding)]
        public static void GetFundingType_MixtureOfCentralAndLocalAndMixedOrderItems_ReturnsExpectedResult(
            OrderItemFundingType input,
            OrderItemFundingType output,
            FundingTypeService service)
        {
            var fundingTypes = new List<OrderItemFundingType>
            {
                OrderItemFundingType.CentralFunding,
                OrderItemFundingType.LocalFunding,
                OrderItemFundingType.LocalFundingOnly,
                OrderItemFundingType.MixedFunding,
            };

            var result = service.GetFundingType(fundingTypes, input);

            result.Should().Be(output);
        }

        [Theory]
        [MockInlineAutoData(OrderItemFundingType.MixedFunding)]
        [MockInlineAutoData(OrderItemFundingType.NoFundingRequired)]
        [MockInlineAutoData(OrderItemFundingType.None)]
        public static void GetFundingType_MixtureOfMixedAndNotFundedOrderItems_ReturnsCentral(
            OrderItemFundingType fundingType,
            FundingTypeService service)
        {
            var fundingTypes = new List<OrderItemFundingType>
            {
                OrderItemFundingType.MixedFunding,
                OrderItemFundingType.NoFundingRequired,
                OrderItemFundingType.None,
            };

            var result = service.GetFundingType(fundingTypes, fundingType);

            result.Should().Be(OrderItemFundingType.CentralFunding);
        }
    }
}
