using System;
using System.Collections.Generic;
using AutoFixture;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    internal sealed class SolutionCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<Integration>(
                c => c.With(
                    i => i.Qualifier,
                    GetIntegrationQualifier()));

            fixture.Customize<Solution>(
                c => c.With(
                        s => s.ClientApplication,
                        JsonConvert.SerializeObject(
                            fixture
                                .Build<ClientApplication>()
                                .With(
                                    ca => ca.BrowsersSupported,
                                    new HashSet<string>
                                    {
                                        "Internet Explorer 11",
                                        "Google Chrome",
                                        "OPERA",
                                        "safari",
                                        "mozilla firefox",
                                    })
                                .With(ca => ca.ClientApplicationTypes, GetClientApplicationTypes())
                                .With(
                                    ca => ca.MobileConnectionDetails,
                                    fixture.Build<MobileConnectionDetails>()
                                        .With(
                                            m => m.ConnectionType,
                                            new HashSet<string> { "5g", "lte", "GpRS", "wifi" })
                                        .Create())
                                .With(
                                    ca => ca.MobileOperatingSystems,
                                    fixture.Build<MobileOperatingSystems>()
                                        .With(m => m.OperatingSystems, new HashSet<string> { "andrOID", "Apple ios" })
                                        .Create())
                                .Create()))
                    .With(s => s.Features, JsonConvert.SerializeObject(fixture.Create<string[]>()))
                    .With(
                        s => s.Integrations,
                        JsonConvert.SerializeObject(
                            fixture
                            .Build<Integration[]>()
                            .CreateMany<Integration>())));
        }

        private static string GetIntegrationQualifier()
        {
            var qualifiers = new List<string>
            {
                "IM1",
                "GP Connect",
            };

            return qualifiers[new Random().Next(qualifiers.Count)];
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
