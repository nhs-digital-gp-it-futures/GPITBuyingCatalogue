using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

public class SolutionQuantitySublocationRecipientCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(
            ICustomizationComposer<SolutionQuantitySublocationRecipient> composer)
        {
            return composer
                .Without(x => x.CompetitionSolution)
                .Without(x => x.CompetitionSublocationRecipient);
        }

        fixture.Customize<SolutionQuantitySublocationRecipient>(ComposerTransformation);
    }
}
