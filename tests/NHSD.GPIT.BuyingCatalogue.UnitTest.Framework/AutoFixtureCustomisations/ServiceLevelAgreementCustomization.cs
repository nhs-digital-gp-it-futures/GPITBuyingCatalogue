using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    internal sealed class ServiceLevelAgreementCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            static ISpecimenBuilder ComposerTransformation(ICustomizationComposer<ServiceLevelAgreements> composer) => composer
                .Without(s => s.SolutionId)
                .Without(s => s.Solution)
                .Without(s => s.Contacts)
                .Without(s => s.ServiceHours)
                .Without(s => s.ServiceLevels);

            fixture.Customize<ServiceLevelAgreements>(ComposerTransformation);
        }
    }
}
