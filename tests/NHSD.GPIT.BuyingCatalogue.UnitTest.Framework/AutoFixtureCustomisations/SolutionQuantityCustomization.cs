using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

public class SolutionQuantityCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<SolutionQuantity> composer) => composer
            .Without(x => x.CompetitionSolution)
            .Without(x => x.CompetitionRecipient);

        fixture.Customize<SolutionQuantity>(ComposerTransformation);
    }
}
