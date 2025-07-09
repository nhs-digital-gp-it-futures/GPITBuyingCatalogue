using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BuyingCatalogueFunctionTests.OrganisationImport;

public class StubbedHttpClient : HttpClient
{
    public StubbedHttpClient(Func<Task<HttpResponseMessage>> invocationAction) : base(new StubbedHttpMessageHandler(invocationAction))
    {
    }

    private class StubbedHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<Task<HttpResponseMessage>> _invocationAction;

        public StubbedHttpMessageHandler(Func<Task<HttpResponseMessage>> invocationAction)
        {
            _invocationAction = invocationAction;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
            => _invocationAction?.Invoke();
    }
}
