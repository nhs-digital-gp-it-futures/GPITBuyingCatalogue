using System;
using AutoFixture.Xunit2;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using MemberAutoDataAttribute = NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations.MemberAutoDataAttribute;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;

#pragma warning disable SA1402
public class MockAutoDataAttribute() : AutoDataAttribute(FixtureFactoryV2.Create);

public class MockInlineAutoDataAttribute(params object[] arguments)
    : InlineAutoDataAttribute(new MockAutoDataAttribute(), arguments);

public class MockMemberAutoDataAttribute(string memberName, params object[] parameters)
    : MemberAutoDataAttribute(memberName, parameters, FixtureFactoryV2.Create);

public class MockInMemoryDbAutoDataAttribute() : AutoDataAttribute(
    () => FixtureFactoryV2.Create(
        new BuyingCatalogueDbContextCustomization(),
        new InMemoryDbCustomization(Guid.NewGuid().ToString()),
        new UserManagerCustomization()));

public class MockInMemoryDbInlineAutoDataAttribute(params object[] arguments)
    : InlineAutoDataAttribute(new MockInMemoryDbAutoDataAttribute(), arguments);

public class MockInMemoryDbMemberAutoDataAttribute(string memberName, params object[] parameters)
    : MemberAutoDataAttribute(
        memberName,
        parameters,
        () => FixtureFactoryV2.Create(
            new BuyingCatalogueDbContextCustomization(),
            new InMemoryDbCustomization(Guid.NewGuid().ToString()),
            new UserManagerCustomization()));

#pragma warning restore SA1402
