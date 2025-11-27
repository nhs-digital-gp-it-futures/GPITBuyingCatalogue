using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

public sealed class CompetitionAdditionalServiceCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<CompetitionAdditionalService> composer) => composer
            .Without(x => x.CatalogueItem)
            .Without(x => x.Price)
            .Without(x => x.Quantities);

        fixture.Customize<CompetitionAdditionalService>(ComposerTransformation);
    }
}
