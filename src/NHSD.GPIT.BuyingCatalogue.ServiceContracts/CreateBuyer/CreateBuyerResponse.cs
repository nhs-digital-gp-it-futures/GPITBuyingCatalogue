using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.CreateBuyer
{
    public class CreateBuyerResponse
    {
        public string UserId { get; set; }

        public IEnumerable<ErrorMessage> Errors { get; set; }
    }

    public sealed class ErrorMessage
    {
        public ErrorMessage(string id, string? field = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Field = field;
        }

        public string Id { get; }

        public string? Field { get; }
    }
}
