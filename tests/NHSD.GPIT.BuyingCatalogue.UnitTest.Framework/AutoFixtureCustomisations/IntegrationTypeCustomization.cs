using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

internal sealed class IntegrationTypeCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<IntegrationType> composer) => composer
            .Without(x => x.Integration)
            .Without(x => x.Solutions);

        fixture.Customize<IntegrationType>(ComposerTransformation);
    }
}
