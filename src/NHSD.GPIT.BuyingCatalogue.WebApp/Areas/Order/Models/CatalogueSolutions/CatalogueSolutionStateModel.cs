using System;
using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public class CatalogueSolutionStateModel
    {
        public DateTime? CommencementDate { get; set; }

        public string SupplierId { get; set; }

        public string SelectedSolutionId { get; set; }

        public DateTime? PlannedDeliveryDate { get; set; }

        public string SolutionName { get; set; }

        public IList<ServiceRecipientsModel> ServiceRecipients { get; set; }

        public int? Quantity { get; set; }
    }
}
