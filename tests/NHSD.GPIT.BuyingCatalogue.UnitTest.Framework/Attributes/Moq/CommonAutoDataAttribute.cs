using System;
using AutoFixture.Xunit2;

// ReSharper disable once CheckNamespace
namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

#pragma warning disable SA1402

[Obsolete("Replace with MockAutoData and migrate the test to use NSubstitute")]
public sealed class CommonAutoDataAttribute() : AutoDataAttribute(FixtureFactory.Create);

[Obsolete("Replace with MockInlineAutoData and migrate the test to use NSubstitute")]
public sealed class CommonInlineAutoDataAttribute(params object[] arguments)
    : InlineAutoDataAttribute(new CommonAutoDataAttribute(), arguments);

[Obsolete("Replace with MockMemberAutoData and migrate the test to use NSubstitute")]
public sealed class CommonMemberAutoDataAttribute(string memberName, params object[] parameters)
    : MemberAutoDataAttribute(memberName, parameters, FixtureFactory.Create);

[Obsolete("Replace with MockInMemoryDbAutoData and migrate the test to use NSubstitute")]
public sealed class InMemoryDbAutoDataAttribute() : AutoDataAttribute(
    () => FixtureFactory.Create(
        new BuyingCatalogueDbContextCustomization(),
        new InMemoryDbCustomization(Guid.NewGuid().ToString(), MockingFramework.Moq),
        new UserManagerCustomization()));

[Obsolete("Replace with MockInMemoryDbInlineAutoData and migrate the test to use NSubstitute")]
public sealed class InMemoryDbInlineAutoDataAttribute(params object[] arguments)
    : InlineAutoDataAttribute(new InMemoryDbAutoDataAttribute(), arguments);

[Obsolete("Replace with MockInMemoryDbMemberAutoData and migrate the test to use NSubstitute")]
public sealed class InMemoryDbMemberAutoDataAttribute(string memberName, params object[] parameters)
    : MemberAutoDataAttribute(
        memberName,
        parameters,
        () => FixtureFactory.Create(
            new BuyingCatalogueDbContextCustomization(),
            new InMemoryDbCustomization(Guid.NewGuid().ToString(), MockingFramework.Moq),
            new UserManagerCustomization()));

#pragma warning restore SA1402
