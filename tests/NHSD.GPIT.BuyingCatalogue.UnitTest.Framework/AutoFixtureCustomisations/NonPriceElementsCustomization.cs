using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

public sealed class NonPriceElementsCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<NonPriceElements> composer) => composer
            .Without(x => x.Competition)
            .Without(x => x.Implementation)
            .Without(x => x.ServiceLevel)
            .Without(x => x.Interoperability)
            .Without(x => x.NonPriceWeights);

        fixture.Customize<NonPriceElements>(ComposerTransformation);
    }
}
