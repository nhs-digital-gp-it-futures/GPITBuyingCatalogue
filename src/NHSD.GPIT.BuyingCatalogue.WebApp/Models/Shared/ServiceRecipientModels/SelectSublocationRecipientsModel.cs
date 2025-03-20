using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class SelectSublocationRecipientsModel : NavBaseModel
    {
        private readonly SelectionMode? selectionMode;

        public SelectSublocationRecipientsModel()
        {
        }

        public SelectSublocationRecipientsModel(
            Competition competition,
            SublocationModel selectedSublocation,
            IEnumerable<ServiceRecipientModel> possibleServiceRecipients,
            IEnumerable<string> requestParameterRecipients,
            string backLinkHref,
            SelectionMode? selectionMode = null,
            bool isAmendment = false)
        {
            Title = "Add service recipients";
            Caption = competition.Name;
            Advice = "Select all the organisations that will be receiving this order";
            BackLink = backLinkHref;

            this.selectionMode = selectionMode;

            PreviouslySelected = selectedSublocation.ServiceRecipients;

            Sublocation = selectedSublocation;

            IsAmendment = isAmendment;

            WorkingServiceRecipients = PreviouslySelected
                .Concat(
                    possibleServiceRecipients.Where(
                        nsr => PreviouslySelected.All(esr => esr.OdsCode != nsr.OdsCode)))
                .OrderBy(x => x.Name)
                .ToList();

            SelectServiceRecipients(requestParameterRecipients, WorkingServiceRecipients);

            RenderedServiceRecipients = WorkingServiceRecipients;
        }

        public SublocationModel Sublocation { get; init; }

        public bool HasImportedRecipients { get; init; }

        public IReadOnlyCollection<ServiceRecipientModel> PreviouslySelected { get; init; }

        public bool IsAmendment { get; init; }

        public IReadOnlyList<ServiceRecipientModel> RenderedServiceRecipients { get; init; }

        private List<ServiceRecipientModel> WorkingServiceRecipients { get; } = [];

        private void SelectServiceRecipients(
            IEnumerable<string> requestParameterRecipients,
            List<ServiceRecipientModel> modifyList)
        {
            switch (selectionMode)
            {
                case SelectionMode.All:
                    modifyList.ForEach(x => x.Selected = true);
                    break;
                case SelectionMode.None:
                    modifyList.ForEach(x => x.Selected = false);
                    break;
                default:
                    if (requestParameterRecipients == null) return;

                    List<ServiceRecipientModel> matchingRecipients = modifyList
                        .Where(x => requestParameterRecipients.Contains(x.OdsCode))
                        .ToList();

                    if (matchingRecipients.Count == 0) return;

                    matchingRecipients.ForEach(x => x.Selected = true);

                    break;
            }
        }
    }
}
