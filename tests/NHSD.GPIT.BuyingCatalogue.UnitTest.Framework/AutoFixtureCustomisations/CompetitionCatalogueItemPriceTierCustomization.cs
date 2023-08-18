using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

public class CompetitionCatalogueItemPriceTierCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<CompetitionCatalogueItemPriceTier> composer) => composer
            .Without(x => x.CompetitionCatalogueItemPrice);

        fixture.Customize<CompetitionCatalogueItemPriceTier>(ComposerTransformation);
    }
}
