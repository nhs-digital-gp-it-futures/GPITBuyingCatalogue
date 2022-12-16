using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.CommencementDate
{
    public class ConfirmChangesModel : NavBaseModel
    {
        public const string YesOption = "Yes, I want to confirm changes to the commencement date";
        public const string NoOption = "No, I do no want to confirm changes to the commencement date";

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public DateTime CurrentDate { get; set; }

        public DateTime NewDate { get; set; }

        public int AffectedPlannedDeliveryDates { get; set; }

        public int TotalPlannedDeliveryDates { get; set; }

        public bool? ConfirmChanges { get; set; }

        public IEnumerable<SelectOption<bool>> Options => new List<SelectOption<bool>>
        {
            new(YesOption, true),
            new(NoOption, false),
        };
    }
}
