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

        public DeleteOrderModel(Order order)
        {
            CallOffId = order.CallOffId;

            if (order.OrderDeletionApproval != null)
            {
                var approvalDate = order.OrderDeletionApproval.DateOfApproval;

                ApprovalDay = approvalDate.Day.ToString("00");
                ApprovalMonth = approvalDate.Month.ToString("00");
                ApprovalYear = approvalDate.Year.ToString("0000");
            }
        }

        public CallOffId CallOffId { get; set; }

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
