using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    internal sealed class ServiceAvailabilityTimesCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<ServiceAvailabilityTimes> composer) => composer
                .Without(s => s.SolutionId)
                .Without(s => s.ServiceLevelAgreement)
                .Without(s => s.LastUpdatedByUser);

            fixture.Customize<ServiceAvailabilityTimes>(ComposerTransformation);
        }
    }
}
