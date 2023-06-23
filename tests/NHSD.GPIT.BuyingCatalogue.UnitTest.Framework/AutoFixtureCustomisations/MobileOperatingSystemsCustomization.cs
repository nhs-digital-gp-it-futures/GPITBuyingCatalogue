using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class MobileOperatingSystemsCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            ISpecimenBuilder ComposerTransformation(ICustomizationComposer<MobileOperatingSystems> composer) => composer
                .With(m => m.OperatingSystems, new HashSet<string> { "android", "Apple iOS" });

            fixture.Customize<MobileOperatingSystems>(ComposerTransformation);
        }
    }
}
