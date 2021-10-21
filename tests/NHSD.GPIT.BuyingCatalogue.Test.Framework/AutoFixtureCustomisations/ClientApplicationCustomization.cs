using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class ClientApplicationCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var browsersSupported = new HashSet<string>
            {
                "Internet Explorer 11",
                "Google Chrome",
                "OPERA",
                "safari",
                "mozilla firefox",
            };

            ISpecimenBuilder ComposerTransformation(ICustomizationComposer<ClientApplication> composer) => composer
                .With(c => c.BrowsersSupported, browsersSupported)
                .With(c => c.ClientApplicationTypes, GetClientApplicationTypes);

            fixture.Customize<ClientApplication>(ComposerTransformation);
        }

        private static HashSet<string> GetClientApplicationTypes()
        {
            var result = new HashSet<string>();

            if (DateTime.Now.Ticks % 2 == 0)
                result.Add("browser-based");
            if (DateTime.Now.Ticks % 2 == 0)
                result.Add("native-mobile");
            if (DateTime.Now.Ticks % 2 == 0)
                result.Add("native-desktop");

            return result;
        }
    }
}
