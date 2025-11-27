using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

public sealed class CompetitionSolutionCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<CompetitionSolution> composer) => composer
            .Without(x => x.CatalogueItem)
            .Without(x => x.Services)
            .Without(x => x.Price)
            .Without(x => x.Scores)
            .Without(x => x.Quantities)
            .Without(x => x.Competition);

        fixture.Customize<CompetitionSolution>(ComposerTransformation);
    }
}
