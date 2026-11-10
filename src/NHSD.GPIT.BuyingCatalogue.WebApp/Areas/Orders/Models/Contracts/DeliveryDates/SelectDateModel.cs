using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;

public class SelectDateModel : DateInputModel
{
    public const string YesOption = "Yes";
    public const string YesAdvice = "You'll apply the same planned delivery date to all organisations. You can come back later to make changes";
    public const string NoOption = "No";
    public const string NoAdvice = "You want to change the planned delivery dates for individual organisations";

    public SelectDateModel()
    {
    }

    public SelectDateModel(string internalOrgId, CallOffId callOffId, Order order, bool? applyToAll, DateTime? existingDeliveryDate)
    {
        ArgumentNullException.ThrowIfNull(order);

        InternalOrgId = internalOrgId;
        CallOffId = callOffId;
        CommencementDate = order.CommencementDate;
        MaximumTerm = order.MaximumTerm;

        IsAmend = order.IsAmendment;
        ExistingDeliveryDate = existingDeliveryDate;

        SetDateFields(order.DeliveryDate);

        ApplyToAll = applyToAll;
    }

    public static IEnumerable<SelectOption<bool>> ApplyToAllOptions =>
    [
        new(YesOption, YesAdvice, true),
        new(NoOption, NoAdvice, false),
    ];

    public string InternalOrgId { get; set; }

    public CallOffId CallOffId { get; set; }

    public DateTime? CommencementDate { get; set; }

    public int? MaximumTerm { get; set; }

    public bool IsAmend { get; set; }

    public DateTime? ExistingDeliveryDate { get; set; }

    public bool? ApplyToAll { get; set; }

    public DateTime? ContractEndDate
    {
        get
        {
            var endDate = new EndDate(CommencementDate, MaximumTerm);
            return endDate.DateTime;
        }
    }
}
