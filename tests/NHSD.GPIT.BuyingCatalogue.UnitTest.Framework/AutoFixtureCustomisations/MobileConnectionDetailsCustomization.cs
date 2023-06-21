using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class MobileConnectionDetailsCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            ISpecimenBuilder ComposerTransformation(ICustomizationComposer<MobileConnectionDetails> composer) => composer
                .With(d => d.ConnectionType, new HashSet<string> { "5g", "lte", "GpRS", "wifi" });

            fixture.Customize<MobileConnectionDetails>(ComposerTransformation);
        }
    }
}
