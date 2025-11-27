using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

public class CompetitionItemQuantityustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(
            ICustomizationComposer<CompetitionItemQuantity> composer)
        {
            return composer;
        }

        fixture.Customize<CompetitionItemQuantity>(ComposerTransformation);
    }
}
