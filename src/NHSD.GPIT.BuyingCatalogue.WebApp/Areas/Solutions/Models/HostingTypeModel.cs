using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class HostingTypeModel
    {
        public HostingTypeModel(PublicCloud hostingType)
        {
            Label = "Public cloud";
            DataTestTag = "public-cloud";
            Summary = hostingType?.Summary;
            Link = hostingType?.Link;
            RequiresHscn = hostingType?.RequiresHscn;
        }

        public HostingTypeModel(PrivateCloud hostingType)
        {
            Label = "Private cloud";
            DataTestTag = "private-cloud";
            Summary = hostingType?.Summary;
            Link = hostingType?.Link;
            RequiresHscn = hostingType?.RequiresHscn;
            HostingModel = hostingType?.HostingModel;
        }

        public HostingTypeModel(HybridHostingType hostingType)
        {
            Label = "Hybrid";
            DataTestTag = "hybrid";
            Summary = hostingType?.Summary;
            Link = hostingType?.Link;
            RequiresHscn = hostingType?.RequiresHscn;
            HostingModel = hostingType?.HostingModel;
        }

        public HostingTypeModel(OnPremise hostingType)
        {
            Label = "On premise";
            DataTestTag = "on-premise";
            Summary = hostingType?.Summary;
            Link = hostingType?.Link;
            RequiresHscn = hostingType?.RequiresHscn;
            HostingModel = hostingType?.HostingModel;
        }

        public string Label { get; set; }

        public string DataTestTag { get; set; }

        public string Summary { get; set; }

        public string Link { get; set; }

        public string HostingModel { get; set; }

        public string RequiresHscn { get; set; }

        public bool DisplayHostingType
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Summary) ||
                    !string.IsNullOrWhiteSpace(Link) ||
                    !string.IsNullOrWhiteSpace(HostingModel) ||
                    !string.IsNullOrWhiteSpace(RequiresHscn);
            }
        }

        public bool DisplaySummary
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Summary) ||
                    !string.IsNullOrWhiteSpace(Link);
            }
        }

        public bool DisplaySummaryDescription
        {
            get { return !string.IsNullOrWhiteSpace(Summary); }
        }

        public bool DisplayLink
        {
            get { return !string.IsNullOrWhiteSpace(Link); }
        }

        public bool DisplayHostingModel
        {
            get { return !string.IsNullOrWhiteSpace(HostingModel); }
        }

        public bool DisplayRequiresHscn
        {
            get { return !string.IsNullOrWhiteSpace(RequiresHscn); }
        }
    }
}
