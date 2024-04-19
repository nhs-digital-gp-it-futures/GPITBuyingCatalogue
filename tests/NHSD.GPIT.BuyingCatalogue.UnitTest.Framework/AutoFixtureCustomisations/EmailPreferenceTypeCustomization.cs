using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

public sealed class EmailPreferenceTypeCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<EmailPreferenceType> composer) => composer
            .Without(e => e.SupportedEventTypes)
            .Without(e => e.UserPreferences);

        fixture.Customize<EmailPreferenceType>(ComposerTransformation);
    }
}
