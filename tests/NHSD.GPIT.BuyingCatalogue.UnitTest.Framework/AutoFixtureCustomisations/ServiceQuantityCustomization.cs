using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

public class ServiceQuantityCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<ServiceQuantity> composer) => composer
            .Without(x => x.SolutionService)
            .Without(x => x.CompetitionRecipient);

        fixture.Customize<ServiceQuantity>(ComposerTransformation);
    }
}
