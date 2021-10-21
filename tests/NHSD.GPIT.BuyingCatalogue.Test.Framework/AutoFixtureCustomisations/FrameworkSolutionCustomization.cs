using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class FrameworkSolutionCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<FrameworkSolution> composer) => composer
                .Without(f => f.LastUpdatedByUser)
                .Without(f => f.Solution);

            fixture.Customize<FrameworkSolution>(ComposerTransformation);
        }
    }
}
