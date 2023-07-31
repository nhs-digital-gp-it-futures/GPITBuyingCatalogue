using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

public sealed class CompetitionCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<Competition> composer) => composer
            .Without(x => x.Weightings)
            .Without(x => x.Organisation)
            .Without(x => x.LastUpdatedByUser)
            .Without(x => x.Filter)
            .Without(x => x.CompetitionSolutions)
            .Without(x => x.Recipients)
            .Without(x => x.NonPriceElements)
            .With(x => x.IsDeleted, false);

        fixture.Customize<Competition>(ComposerTransformation);
    }
}
