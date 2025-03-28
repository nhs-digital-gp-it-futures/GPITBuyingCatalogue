using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class SelectSublocationRecipientsModel : NavBaseModel
    {
        public SelectSublocationRecipientsModel()
        {
        }

        public SelectSublocationRecipientsModel(
            Competition competition,
            SublocationModel selectedSublocation,
            IEnumerable<ServiceRecipientModel> possibleServiceRecipients,
            string backLinkHref,
            SelectionMode? selectionMode = null,
            bool isAmendment = false)
        {
            Title = "Add service recipients";
            Caption = competition.Name;
            Advice = "Select all the organisations that will be receiving this order";
            BackLink = backLinkHref;

            SelectionMode = selectionMode;

            Sublocation = selectedSublocation;

            IsAmendment = isAmendment;

            WorkingServiceRecipients = PreviouslySelected
                .Concat(
                    possibleServiceRecipients.Where(
                        nsr => PreviouslySelected.All(esr => esr.OdsCode != nsr.OdsCode)))
                .OrderBy(x => x.Name)
                .ToList();

            SelectServiceRecipients(WorkingServiceRecipients);

            RenderedServiceRecipients = WorkingServiceRecipients;
        }

        public SublocationModel Sublocation { get; init; }

        public IReadOnlyCollection<ServiceRecipientModel> PreviouslySelected => Sublocation.ServiceRecipients;

        public bool IsAmendment { get; init; }

        public IReadOnlyList<ServiceRecipientModel> RenderedServiceRecipients { get; init; }

        public SelectionMode? SelectionMode { get; init; }

        private List<ServiceRecipientModel> WorkingServiceRecipients { get; } = [];

        private void SelectServiceRecipients(
            List<ServiceRecipientModel> modifyList)
        {
            switch (SelectionMode)
            {
                case ServiceRecipientModels.SelectionMode.All:
                    modifyList.ForEach(x => x.Selected = true);
                    break;
                case ServiceRecipientModels.SelectionMode.None:
                    modifyList.ForEach(x => x.Selected = false);
                    break;
                case null:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(SelectionMode), @"Selection mode mot handled");
            }
        }
    }
}
