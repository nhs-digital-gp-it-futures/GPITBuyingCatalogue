using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Verification;

namespace NHSD.GPIT.BuyingCatalogue.Services.Verifications
{
    public class UrlVerificationService : IUrlVerificationService
    {
        private readonly HttpClient httpclient;

        public UrlVerificationService(HttpClient httpclient)
        {
            this.httpclient = httpclient ?? throw new ArgumentNullException(nameof(httpclient));
        }

        public async Task<bool> VerifyUrl(string siteLink)
        {
            try
            {
                if (string.IsNullOrEmpty(siteLink))
                    return true;
                Uri uri = new Uri(siteLink);
                var result = await httpclient.GetAsync(uri);
                return result.StatusCode == HttpStatusCode.OK;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
    }
}
