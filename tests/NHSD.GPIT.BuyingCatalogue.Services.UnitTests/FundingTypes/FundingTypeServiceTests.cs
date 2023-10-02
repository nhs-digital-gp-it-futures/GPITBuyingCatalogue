using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Services.FundingTypes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.FundingTypes
{
    public class FundingTypeServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(FundingTypeService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
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
        [CommonInlineAutoData(OrderItemFundingType.LocalFunding)]
        [CommonInlineAutoData(OrderItemFundingType.LocalFundingOnly)]
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
        [CommonAutoData]
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
        [CommonAutoData]
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
        [CommonAutoData]
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
        [CommonAutoData]
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
        [CommonAutoData]
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
        [CommonInlineAutoData(OrderItemFundingType.CentralFunding)]
        [CommonInlineAutoData(OrderItemFundingType.MixedFunding)]
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
        [CommonInlineAutoData(OrderItemFundingType.LocalFunding)]
        [CommonInlineAutoData(OrderItemFundingType.LocalFundingOnly)]
        [CommonInlineAutoData(OrderItemFundingType.MixedFunding)]
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
        [CommonInlineAutoData(OrderItemFundingType.CentralFunding)]
        [CommonInlineAutoData(OrderItemFundingType.NoFundingRequired)]
        [CommonInlineAutoData(OrderItemFundingType.None)]
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
        [CommonInlineAutoData(OrderItemFundingType.LocalFunding)]
        [CommonInlineAutoData(OrderItemFundingType.LocalFundingOnly)]
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
        [CommonInlineAutoData(OrderItemFundingType.CentralFunding, OrderItemFundingType.CentralFunding)]
        [CommonInlineAutoData(OrderItemFundingType.LocalFunding, OrderItemFundingType.LocalFunding)]
        [CommonInlineAutoData(OrderItemFundingType.LocalFundingOnly, OrderItemFundingType.LocalFunding)]
        [CommonInlineAutoData(OrderItemFundingType.NoFundingRequired, OrderItemFundingType.CentralFunding)]
        [CommonInlineAutoData(OrderItemFundingType.None, OrderItemFundingType.CentralFunding)]
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
        [CommonInlineAutoData(OrderItemFundingType.NoFundingRequired)]
        [CommonInlineAutoData(OrderItemFundingType.None)]
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
        [CommonInlineAutoData(OrderItemFundingType.NoFundingRequired)]
        [CommonInlineAutoData(OrderItemFundingType.None)]
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
        [CommonInlineAutoData(OrderItemFundingType.CentralFunding, OrderItemFundingType.CentralFunding)]
        [CommonInlineAutoData(OrderItemFundingType.LocalFunding, OrderItemFundingType.LocalFunding)]
        [CommonInlineAutoData(OrderItemFundingType.LocalFundingOnly, OrderItemFundingType.LocalFunding)]
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
        [CommonInlineAutoData(OrderItemFundingType.CentralFunding, OrderItemFundingType.CentralFunding)]
        [CommonInlineAutoData(OrderItemFundingType.LocalFunding, OrderItemFundingType.LocalFunding)]
        [CommonInlineAutoData(OrderItemFundingType.LocalFundingOnly, OrderItemFundingType.LocalFunding)]
        [CommonInlineAutoData(OrderItemFundingType.MixedFunding, OrderItemFundingType.CentralFunding)]
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
        [CommonInlineAutoData(OrderItemFundingType.MixedFunding)]
        [CommonInlineAutoData(OrderItemFundingType.NoFundingRequired)]
        [CommonInlineAutoData(OrderItemFundingType.None)]
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
