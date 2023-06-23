using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class ClientApplicationCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var browsersSupported = new HashSet<SupportedBrowser>
            {
                new SupportedBrowser { BrowserName = "Internet Explorer 11" },
                new SupportedBrowser { BrowserName = "Google Chrome" },
                new SupportedBrowser { BrowserName = "OPERA" },
                new SupportedBrowser { BrowserName = "safari" },
                new SupportedBrowser { BrowserName = "mozilla firefox" },
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
