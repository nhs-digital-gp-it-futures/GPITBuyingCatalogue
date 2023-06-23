using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Shared
{
    public class ContractBillingItemTableModel
    {
        public ContractBillingItemTableModel()
        {
        }

        public ContractBillingItemTableModel(string title, IEnumerable<ContractBillingItem> contractBillingItems, CallOffId callOffId, string internalOrgId)
        {
            Title = title;
            ContractBillingItems = contractBillingItems ?? Enumerable.Empty<ContractBillingItem>();
            CallOffId = callOffId;
            InternalOrgId = internalOrgId;
        }

        public IEnumerable<ContractBillingItem> ContractBillingItems { get; set; }

        public string Title { get; set; }

        public bool IsAction { get; set; }

        public CallOffId CallOffId { get; set; }

        public string InternalOrgId { get; set; }
    }
}
