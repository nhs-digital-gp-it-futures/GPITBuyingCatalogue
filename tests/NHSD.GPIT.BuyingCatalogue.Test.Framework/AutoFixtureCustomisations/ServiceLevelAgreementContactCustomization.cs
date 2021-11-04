using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    internal sealed class ServiceLevelAgreementContactCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<SlaContact> composer) => composer
                .Without(slac => slac.SolutionId)
                .Without(slac => slac.LastUpdatedByUser)
                .Without(slac => slac.ServiceLevelAgreement);

            fixture.Customize<SlaContact>(ComposerTransformation);
        }
    }
}
