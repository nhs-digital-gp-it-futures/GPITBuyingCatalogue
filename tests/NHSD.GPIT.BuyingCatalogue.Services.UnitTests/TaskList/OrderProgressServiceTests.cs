﻿using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.TaskList
{
    public static class OrderProgressServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderProgressService).GetConstructors();

            assertion.Verify(constructors);
        }
    }
}
