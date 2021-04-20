using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public class MobileConnectionDetails
    {
        public HashSet<string> ConnectionType { get; set; }

        public string MinimumConnectionSpeed { get; set; }

        public string Description { get; set; }
    }
}
