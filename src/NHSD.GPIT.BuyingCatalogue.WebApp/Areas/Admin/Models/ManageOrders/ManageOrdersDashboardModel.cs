using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageOrders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders
{
    public class ManageOrdersDashboardModel : NavBaseModel
    {
        public ManageOrdersDashboardModel(IList<AdminManageOrder> orders, PageOptions options)
        {
            Orders = orders?.OrderByDescending(o => o.Created).ToList();
            Options = options;
        }

        public IList<AdminManageOrder> Orders { get; set; }

        public PageOptions Options { get; set; }
    }
}
