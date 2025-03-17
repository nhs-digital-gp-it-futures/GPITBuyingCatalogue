using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels
{
    public sealed class SelectRecipientsV2Model : NavBaseModel
    {
        private readonly SelectionMode? selectionMode;

        public SelectRecipientsV2Model(
            Competition competition,
            SublocationModel selectedSublocation,
            IEnumerable<ServiceRecipientModel> possibleServiceRecipients,
            IEnumerable<string> requestParameterRecipients,
            SelectionMode? selectionMode = null,
            bool isAmendment = false)
        {
            Title = "Add service recipients";
            Caption = competition.Name;
            Advice = "Select all the organisations that will be receiving this order";

            this.selectionMode = selectionMode;

            Organisation organisation = competition.Organisation;

            OrganisationName = organisation.Name;
            OrganisationType = organisation.OrganisationType.GetValueOrDefault();
            PreviouslySelected = selectedSublocation.ServiceRecipients;
            PossibleServiceRecipients = possibleServiceRecipients.ToList();

            Sublocation = selectedSublocation;

            IsAmendment = isAmendment;

            PopulateRenderedServiceRecipients();

            SelectServiceRecipients(requestParameterRecipients);
        }

        public string OrganisationName { get; set; }

        public OrganisationType OrganisationType { get; set; }

        public SublocationModel Sublocation { get; set; }

        public ServiceRecipientModel[] SearchRecipients => Sublocation.ServiceRecipients
            .Select(y => new ServiceRecipientModel { Name = y.Name, OdsCode = y.OdsCode })
            .OrderBy(x => x.Name)
            .ToArray();

        public bool HasImportedRecipients { get; set; }

        public List<ServiceRecipientModel> PreviouslySelected { get; set; }

        public int? SelectAtLeast { get; set; }

        public bool IsAmendment { get; set; }

        public List<ServiceRecipientModel> PossibleServiceRecipients { get; set; }

        public List<ServiceRecipientModel> RenderedServiceRecipients { get; set; } = [];

        private void PopulateRenderedServiceRecipients()
        {
            RenderedServiceRecipients.AddRange(PreviouslySelected);
            RenderedServiceRecipients.AddRange(
                PossibleServiceRecipients.Where(
                    nsr => RenderedServiceRecipients.All(esr => esr.OdsCode != nsr.OdsCode)));
            RenderedServiceRecipients.Sort((x, y) => string.CompareOrdinal(x.Name, y.Name));
        }

        private void SelectServiceRecipients(IEnumerable<string> requestParameterRecipients)
        {
            switch (selectionMode)
            {
                case SelectionMode.All:
                    RenderedServiceRecipients.ForEach(x => x.Selected = true);
                    break;
                case SelectionMode.None:
                    RenderedServiceRecipients.ForEach(x => x.Selected = false);
                    break;
                default:
                    if (requestParameterRecipients == null) return;

                    List<ServiceRecipientModel> matchingRecipients = RenderedServiceRecipients
                        .Where(x => requestParameterRecipients.Contains(x.OdsCode))
                        .ToList();
                    if (matchingRecipients.Count == 0) return;

                    matchingRecipients.ForEach(x => x.Selected = true);

                    break;
            }
        }
    }
}
