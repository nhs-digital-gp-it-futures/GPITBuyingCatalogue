using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

public class ServiceQuantitySublocationRecipientsCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<ServiceQuantitySublocationRecipient> composer)
        {
            return composer
                .Without(x => x.SolutionService)
                .Without(x => x.CompetitionSublocationRecipient);
        }

        fixture.Customize<ServiceQuantitySublocationRecipient>(ComposerTransformation);
    }
}
