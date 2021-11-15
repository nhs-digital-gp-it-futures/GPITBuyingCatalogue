using System;
using System.Net.Http;
using AutoFixture;
using AutoFixture.Kernel;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations
{
    public sealed class HttpClientSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (!(request as Type == typeof(HttpClient)))
                return new NoSpecimen();

            var mockHttpMessageHandler = context.Create<HttpMessageHandler>();

            return new HttpClient(mockHttpMessageHandler);
        }
    }
}
