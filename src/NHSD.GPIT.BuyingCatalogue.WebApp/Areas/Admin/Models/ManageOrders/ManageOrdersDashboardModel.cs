using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageOrders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders
{
    public class ManageOrdersDashboardModel : NavBaseModel
    {
        public ManageOrdersDashboardModel()
        {
        }

        public ManageOrdersDashboardModel(IList<AdminManageOrder> orders, IEnumerable<FrameworkFilterInfo> frameworks, PageOptions options)
        {
            Orders = orders?.OrderByDescending(o => o.Created).ToList();
            Options = options;
            SetAvailableFrameworks(frameworks);
        }

        public IList<AdminManageOrder> Orders { get; set; }

        public PageOptions Options { get; set; }

        public OrderStatus? SelectedFramework { get; set; }

        public List<FrameworkFilterInfo> Frameworks { get; set; }

        public IEnumerable<SelectOption<string>> AvailableFrameworks { get; set; }

        public OrderStatus? SelectedStatus { get; set; }

        public IEnumerable<SelectOption<OrderStatus>> AvailableStatus =>
            Enum.GetValues(typeof(OrderStatus))
                .Cast<OrderStatus>()
                .Select(
                    x => new SelectOption<OrderStatus>
                    {
                        Value = x,
                        Text = x.EnumMemberName(),
                    }).ToList();

        private void SetAvailableFrameworks(IEnumerable<FrameworkFilterInfo> frameworks)
        {
            AvailableFrameworks = frameworks
                .Select(
                    f => new SelectOption<string>
                    {
                        Value = f.Id,
                        Text = $"{f.ShortName}{(f.Expired ? " (expired)" : string.Empty)}",
                    })
                .ToList();
        }
    }
}
