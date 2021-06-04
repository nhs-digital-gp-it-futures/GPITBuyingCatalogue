using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public sealed record ErrorDetails
    {
        public ErrorDetails(string id)
            : this(id, null)
        {
        }

        public ErrorDetails(string id, string field)
            : this(string.Empty, field, id)
        {
        }

        public ErrorDetails(string parentName, string field, string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Field = field;
            ParentName = parentName;
        }

        public string Id { get; }

        public string Field { get; }

        public string ParentName { get; }
    }
}
