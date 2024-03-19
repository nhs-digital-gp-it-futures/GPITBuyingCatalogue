using System;
using AutoFixture.Xunit2;

// ReSharper disable once CheckNamespace
namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

#pragma warning disable SA1402

public sealed class CommonAutoDataAttribute() : AutoDataAttribute(FixtureFactory.Create);

public sealed class CommonInlineAutoDataAttribute(params object[] arguments)
    : InlineAutoDataAttribute(new CommonAutoDataAttribute(), arguments);

public sealed class CommonMemberAutoDataAttribute(string memberName, params object[] parameters)
    : MemberAutoDataAttribute(memberName, parameters, FixtureFactory.Create);

public sealed class InMemoryDbAutoDataAttribute() : AutoDataAttribute(
    () => FixtureFactory.Create(
        new BuyingCatalogueDbContextCustomization(),
        new InMemoryDbCustomization(Guid.NewGuid().ToString(), MockingFramework.Moq),
        new UserManagerCustomization()));

public sealed class InMemoryDbInlineAutoDataAttribute(params object[] arguments)
    : InlineAutoDataAttribute(new InMemoryDbAutoDataAttribute(), arguments);

public sealed class InMemoryDbMemberAutoDataAttribute(string memberName, params object[] parameters)
    : MemberAutoDataAttribute(
        memberName,
        parameters,
        () => FixtureFactory.Create(
            new BuyingCatalogueDbContextCustomization(),
            new InMemoryDbCustomization(Guid.NewGuid().ToString(), MockingFramework.Moq),
            new UserManagerCustomization()));

#pragma warning restore SA1402
