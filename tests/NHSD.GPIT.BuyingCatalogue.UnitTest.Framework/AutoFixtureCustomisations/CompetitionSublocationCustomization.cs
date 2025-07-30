using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class CompetitionSublocationCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<CompetitionSublocation> composer)
            {
                return composer
                    .Without(x => x.Competition)
                    .Without(x => x.CompetitionId);
            }

            fixture.Customize<CompetitionSublocation>(ComposerTransformation);
        }
    }
}
