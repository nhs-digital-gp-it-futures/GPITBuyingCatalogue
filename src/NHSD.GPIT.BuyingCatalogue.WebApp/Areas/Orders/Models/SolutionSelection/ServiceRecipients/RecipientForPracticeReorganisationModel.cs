using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients
{
    public sealed class RecipientForPracticeReorganisationModel : NavBaseModel
    {
        public RecipientForPracticeReorganisationModel()
        {
        }

        public RecipientForPracticeReorganisationModel(
            Organisation organisation,
            CallOffId callOffId,
            OrderType orderType,
            List<ServiceRecipientModel> recipients)
        {
            GetTitleAndAdviceFromOrderType(orderType);
            Caption = $"Order {callOffId}";

            OrganisationName = organisation.Name;
            OrganisationType = organisation.OrganisationType.GetValueOrDefault();

            SubLocations = recipients
                .GroupBy(x => x.Location)
                .Select(
                    x => new SublocationModel(
                        x.Key,
                        x.ToList()))
                .OrderBy(x => x.Name)
                .ToArray();
        }

        public string OrganisationName { get; set; }

        public OrganisationType OrganisationType { get; set; }

        public SublocationModel[] SubLocations { get; set; } = [];

        public string SelectedOdsCode { get; set; }

        private void GetTitleAndAdviceFromOrderType(OrderType orderType)
        {
            Title = orderType.GetPracticeReorganisationRecipientTitle();
            Advice = orderType.Value switch
            {
                OrderTypeEnum.AssociatedServiceSplit =>
                    "Select the Service Recipient that will be losing patients as part of the split.",
                OrderTypeEnum.AssociatedServiceMerger =>
                    "Select the Service Recipient that will still exist after the merger.",
                _ => throw new ArgumentOutOfRangeException(nameof(orderType)),
            };
        }
    }
}
