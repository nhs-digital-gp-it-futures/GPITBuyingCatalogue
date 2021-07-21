using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    [ExcludeFromCodeCoverage]
    public class HomeModel
    {
        public HomeModel(HttpRequest request)
        {
            HeaderInfo = new List<string>();

            foreach (var header in request.Headers)
                HeaderInfo.Add($"{header.Key} {header.Value}");

            HeaderInfo.Add($"** Scheme {request.Scheme}");
        }

        public List<string> HeaderInfo { get; set; }
    }
}
