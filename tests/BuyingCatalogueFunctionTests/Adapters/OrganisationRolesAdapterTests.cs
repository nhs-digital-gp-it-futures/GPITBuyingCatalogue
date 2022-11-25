﻿using System;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using BuyingCatalogueFunction.Adapters;
using BuyingCatalogueFunction.Models.Ods;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace BuyingCatalogueFunctionTests.Adapters
{
    public static class OrganisationRolesAdapterTests
    {
        [Fact]
        public static void Constructor_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrganisationRolesAdapter).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static void Process_InputIsNull_ThrowsException(
            OrganisationRolesAdapter adapter)
        {
            FluentActions
                .Invoking(() => adapter.Process(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [CommonAutoData]
        public static void Process_InputIsNotNull_ReturnsExpectedResult(
            Org input,
            OrganisationRolesAdapter adapter)
        {
            var result = adapter.Process(input).ToList();

            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Count.Should().Be(input.Roles.Role.Count);

            foreach (var output in result)
            {
                var existing = input.Roles.Role.FirstOrDefault(x => x.uniqueRoleId == output.Id);

                existing.Should().NotBeNull();

                output.OrganisationId.Should().Be(input.OrgId.extension);
                output.RoleId.Should().Be(existing!.id);
                output.IsPrimaryRole.Should().Be(existing.primaryRole ?? false);
            }
        }
    }
}
