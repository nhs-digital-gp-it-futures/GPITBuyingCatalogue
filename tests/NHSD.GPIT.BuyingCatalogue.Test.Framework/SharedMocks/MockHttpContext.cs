using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Middleware.CookieConsent;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.SharedMocks
{
    public sealed class MockHttpContext
    {
        private readonly Dictionary<object, object> actualItems = new(2);
        private readonly Mock<HttpContext> httpContextMock = new();
        private readonly Mock<HttpRequest> httpRequestMock = new();
        private readonly Mock<IRequestCookieCollection> requestCookieCollectionMock = new();

        private string cookieContent;

        // ReSharper disable once UnusedMember.Global (required for AutoFixture)
        public MockHttpContext()
            : this(true, true)
        {
        }

        public MockHttpContext(bool includeRequest, bool includeCookieContent)
        {
            if (includeRequest)
                httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);

            httpContextMock.Setup(c => c.Items).Returns(actualItems);
            httpRequestMock.Setup(r => r.Cookies).Returns(requestCookieCollectionMock.Object);

            if (includeCookieContent)
                CookieContent = null;
        }

        public IDictionary<object, object> ActualItems => actualItems;

        public string CookieContent
        {
            set
            {
                cookieContent = value;
                UpdateRequestCookieCollectionMock();
            }
        }

        public HttpContext HttpContext => httpContextMock.Object;

        public void SetCookieData(CookieData data) =>
            CookieContent = JsonSerializer.Serialize(data, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

        private void UpdateRequestCookieCollectionMock()
        {
            requestCookieCollectionMock
                .Setup(r => r.TryGetValue(CatalogueCookies.BuyingCatalogueConsent, out cookieContent))
                .Returns(true);
        }
    }
}
