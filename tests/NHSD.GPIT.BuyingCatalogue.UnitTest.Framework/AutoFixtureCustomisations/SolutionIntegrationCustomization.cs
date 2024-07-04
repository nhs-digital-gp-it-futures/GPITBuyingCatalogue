using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

internal sealed class SolutionIntegrationCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<SolutionIntegration> composer) => composer
            .Without(x => x.IntegrationType)
            .Without(x => x.Solution);

        fixture.Customize<SolutionIntegration>(ComposerTransformation);
    }
}
