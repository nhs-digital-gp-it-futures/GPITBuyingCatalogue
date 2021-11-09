using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Verification;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Verifications
{
    public class VerificationService : IVerificationService
    {
        public async Task<bool> VerifyUrl(string uRL)
        {
            try
            {
                if (string.IsNullOrEmpty(uRL))
                    return true;
                Uri uri = new Uri(uRL);
                using var client = new HttpClient();
                var result = await client.GetAsync(uri);
                return result.StatusCode == HttpStatusCode.OK;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
    }
}
