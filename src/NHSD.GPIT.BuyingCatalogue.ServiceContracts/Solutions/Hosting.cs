using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public class Hosting
    {
        public PublicCloud PublicCloud { get; set; }

        public PrivateCloud PrivateCloud { get; set; }

        public HybridHostingType HybridHostingType { get; set; }

        public OnPremise OnPremise { get; set; }
    }
}
