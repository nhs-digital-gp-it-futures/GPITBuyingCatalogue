using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

internal static class FixtureFactoryV2
{
    internal static IFixture Create() => Create(Array.Empty<ICustomization>());

    internal static IFixture Create(params ICustomization[] customizations) =>
        new Fixture().Customize(new CompositeCustomization(CreateCustomizations().Concat(customizations)));

    private static IEnumerable<ICustomization> CreateCustomizations()
    {
        var customizations = typeof(FixtureFactoryV2).Assembly.GetTypes()
            .Where(
                x => x.GetInterfaces().Contains(typeof(ICustomization))
                    && x.IsClass
                    && x.GetCustomAttribute(typeof(ExcludesAutoCustomizationAttribute)) is null)
            .Select(x => (ICustomization)Activator.CreateInstance(x))
            .ToList();

        customizations.Insert(0, new AutoNSubstituteCustomization());

        return customizations;
    }
}
