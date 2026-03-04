using System;
using System.Web;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class UriBuilderExtensions
    {
        public static UriBuilder AppendQueryParameterToUrl(this UriBuilder uri, string queryParameterName, string queryParameterValue)
        {
            ArgumentNullException.ThrowIfNull(uri);

            var query = HttpUtility.ParseQueryString(uri.Query);
            query.Set(queryParameterName, queryParameterValue);

            uri.Query = query.ToString();
            return uri;
        }
    }
}
