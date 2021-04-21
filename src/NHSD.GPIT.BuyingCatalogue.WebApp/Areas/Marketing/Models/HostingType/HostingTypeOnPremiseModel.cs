using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType
{
    public class HostingTypeOnPremiseModel
    {
        public HostingTypeOnPremiseModel()
        {
        }

        public HostingTypeOnPremiseModel(CatalogueItem catalogueItem)
        {
            SolutionId = catalogueItem.CatalogueItemId;
            OnPremise = catalogueItem.Solution.GetHosting().OnPremise;          
        }

        public string SolutionId { get; set; }

        public OnPremise OnPremise { get; set; }

        public bool RequiresHscnChecked
        {
            get { return !string.IsNullOrWhiteSpace(OnPremise.RequiresHscn); }
            set
            {
                if (value)
                    OnPremise.RequiresHscn = "End user devices must be connected to HSCN/N3";
                else
                    OnPremise.RequiresHscn = null;
            }
        }
    }
}
