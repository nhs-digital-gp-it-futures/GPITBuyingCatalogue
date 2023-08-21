using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

public class CompetitionCatalogueItemPriceCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<CompetitionCatalogueItemPrice> composer) => composer
            .Without(x => x.Competition)
            .Without(x => x.Tiers);

        fixture.Customize<CompetitionCatalogueItemPrice>(ComposerTransformation);
    }
}
