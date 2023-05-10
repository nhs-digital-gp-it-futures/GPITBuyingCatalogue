using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders
{
    public sealed class DeleteOrderModel : NavBaseModel
    {
        public DeleteOrderModel()
        {
        }

        public DeleteOrderModel(CallOffId callOffId)
        {
            CallOffId = callOffId;
        }

        public CallOffId CallOffId { get; set; }

        public bool IsAmendment => CallOffId.IsAmendment;

        public string NameOfRequester { get; set; }

        public string NameOfApprover { get; set; }

        [StringLength(2)]
        public string ApprovalDay { get; set; }

        [StringLength(2)]
        public string ApprovalMonth { get; set; }

        [StringLength(4)]
        public string ApprovalYear { get; set; }

        public DateTime OrderCreationDate { get; set; }

        public DateTime? ApprovalDate
        {
            get
            {
                if (!DateTime.TryParseExact($"{ApprovalDay}/{ApprovalMonth}/{ApprovalYear}", "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var approvalDate))
                    return null;

                return approvalDate.ToUniversalTime();
            }
        }
    }
}
