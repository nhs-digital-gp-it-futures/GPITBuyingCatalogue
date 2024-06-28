using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

internal sealed class FilterCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<Filter> composer) => composer
            .Without(x => x.Integrations);

        fixture.Customize<Filter>(ComposerTransformation);
    }
}
