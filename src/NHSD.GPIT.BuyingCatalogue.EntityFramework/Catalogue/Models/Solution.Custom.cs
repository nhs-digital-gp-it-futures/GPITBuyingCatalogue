using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed partial class Solution : IAudited
    {
        public ClientApplication EnsureAndGetClientApplication()
        {
            return ClientApplication ?? new ClientApplication();
        }
    }
}
