using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.DeliveryDates
{
    public class ConfirmChangesModel : NavBaseModel
    {
        public ConfirmChangesModel()
        {
        }

        public ConfirmChangesModel(string internalOrgId, CallOffId callOffId, DateTime currentDeliveryDate, DateTime newDeliveryDate)
        {
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            CurrentDeliveryDate = currentDeliveryDate;
            NewDeliveryDate = newDeliveryDate;
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public DateTime CurrentDeliveryDate { get; set; }

        public DateTime NewDeliveryDate { get; set; }

        public bool? ConfirmChanges { get; set; }

        public IEnumerable<SelectableRadioOption<bool>> Options => new List<SelectableRadioOption<bool>>
        {
            new("Yes, I want to confirm changes to my planned delivery date", true),
            new("No, I do no want to confirm changes to my planned delivery date", false),
        };
    }
}
