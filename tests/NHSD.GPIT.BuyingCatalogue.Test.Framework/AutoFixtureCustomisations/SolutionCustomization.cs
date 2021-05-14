using System;
using System.Collections.Generic;
using AutoFixture;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    internal sealed class SolutionCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<Solution>(
                c => c.With(s => s.ClientApplication,
                    JsonConvert.SerializeObject(
                        fixture
                            .Build<ClientApplication>()
                            .With(ca => ca.BrowsersSupported, new HashSet<string>
                                {"Internet Explorer 11", "Google Chrome", "OPERA", "safari", "mozilla firefox"})
                            .With(ca => ca.ClientApplicationTypes, GetClientApplicationTypes())
                            .With(ca => ca.MobileOperatingSystems, fixture.Build<MobileOperatingSystems>()
                                .With(m => m.OperatingSystems, new HashSet<string>
                                {
                                    "andrOID",
                                    "Apple ios",
                                }).Create())
                            .Create()
                    ))
                    .With(s => s.Hosting, JsonConvert.SerializeObject(fixture.Create<Hosting>())));
        }
        
        private static HashSet<string> GetClientApplicationTypes()
        {
            var result = new HashSet<string>();

            if (DateTime.Now.Ticks % 2 == 0)
                result.Add("browser-BASED");
            if (DateTime.Now.Ticks % 2 == 0)
                result.Add("NATive-mobile");
            if (DateTime.Now.Ticks % 2 == 0)
                result.Add("native-DESKtop");

            return result;
        }
    }
}