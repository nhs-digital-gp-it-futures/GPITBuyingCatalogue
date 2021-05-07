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
                    JsonConvert.SerializeObject(fixture
                        .Build<ClientApplication>()
                        .With(ca => ca.BrowsersSupported, new HashSet<string>
                            {"Internet Explorer 11", "Google Chrome", "OPERA", "safari", "mozilla firefox"})
                        .Create()
                    )));
        }
    }
}