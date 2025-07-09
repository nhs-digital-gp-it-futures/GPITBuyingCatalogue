using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using BuyingCatalogueFunction.EpicsAndCapabilities.Models;
using FluentAssertions;
using Xunit;

namespace BuyingCatalogueFunctionTests.EpicsAndCapabilities.Model
{
    public static class CapabilityIdCSVTests
    {
        [Fact]
        public static void Constructor_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CapabilityIdCsv).GetConstructors();

            assertion.Verify(constructors);
        }

        [Fact]
        public static void Invalid()
        {
            FluentActions
                .Invoking(() => new CapabilityIdCsv("ABC"))
                .Should().Throw<FormatException>();
        }

        [Fact]
        public static void Valid()
        {
            var id = new CapabilityIdCsv("C1");
            id.Value.Should().Be(1);
        }
    }
}
