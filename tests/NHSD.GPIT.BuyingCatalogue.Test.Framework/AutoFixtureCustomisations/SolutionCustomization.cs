using System;
using System.Collections.Generic;
using System.Linq;
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
                                .Create()
                        ))
                    .With(s => s.Features, JsonConvert.SerializeObject(fixture.Create<string[]>()))
                    .With(s => s.Hosting, JsonConvert.SerializeObject(fixture.Create<Hosting>()))
                    .With(s => s.Integrations, JsonConvert.SerializeObject(GetIntegrations(fixture)))
            );
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

        private List<Integration> GetIntegrations(IFixture fixture)
        {
            var result = fixture.Build<Integration>()
                .With(i => i.SubTypes, fixture.CreateMany<IntegrationSubType>().ToArray)
                .CreateMany()
                .ToList();

            foreach (var integration in result)
            {
                for (int i = 0; i < integration.SubTypes.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        integration.SubTypes[i].DetailsDictionary.Clear();
                    }
                    else
                    {
                        integration.SubTypes[i].DetailsSystemDictionary.Clear();
                    }
                }
            }

            return result;
        }
    }
}
