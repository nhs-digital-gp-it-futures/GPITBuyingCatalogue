using System.Linq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public class SolutionDetailModel
    {
        public SolutionDetailModel(CatalogueItem catalogueItem)
        {
            CatalogueItem = catalogueItem;

            //DecodeFeatures();
            //PopulateContactInformation();
            //PopulateFrameworks();
        }

        public CatalogueItem CatalogueItem { get; private set; }

        public string SolutionDescriptionStatus
        {
            get 
            { 
                return string.IsNullOrWhiteSpace(CatalogueItem.Solution.Summary) ? "INCOMPLETE" : "COMPLETE";
            }
        }

        public string FeaturesStatus
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CatalogueItem.Solution.Features))
                    return "INCOMPLETE";
                
                var features = JsonConvert.DeserializeObject<string[]>(CatalogueItem.Solution.Features);

                return features.Any() ? "COMPLETE" : "INCOMPLETE";
            }
        }

        public string IntegrationsStatus
        {
            get
            {
                return string.IsNullOrWhiteSpace(CatalogueItem.Solution.IntegrationsUrl) ? "INCOMPLETE" : "COMPLETE";
            }
        }

        public string ImplementationTimescalesStatus
        {
            get
            {
                return string.IsNullOrWhiteSpace(CatalogueItem.Solution.ImplementationDetail) ? "INCOMPLETE" : "COMPLETE";
            }
        }

        public string RoadmapStatus
        {
            get
            {
                return string.IsNullOrWhiteSpace(CatalogueItem.Solution.RoadMap) ? "INCOMPLETE" : "COMPLETE";
            }
        }

        //public string[] Features { get; private set; }

        //public string Frameworks { get; private set; }

        //public string ContactName { get; private set; }

        //public string Department { get; private set; }

        //public string PhoneNumber { get; private set; }

        //public string EmailAddress { get; private set; }

        //private void DecodeFeatures()
        //{
        //    Features = new string[0];

        //    if(!string.IsNullOrWhiteSpace(CatalogueItem.Solution.Features))
        //        Features = JsonConvert.DeserializeObject<string[]>(CatalogueItem.Solution.Features);
        //}

        //private void PopulateContactInformation()
        //{
        //    if( CatalogueItem.Solution.MarketingContacts.Any())
        //    {
        //        var contact = CatalogueItem.Solution.MarketingContacts.First();
        //        ContactName = $"{contact.FirstName} {contact.LastName}";
        //        Department = contact.Department;
        //        PhoneNumber = contact.PhoneNumber;
        //        EmailAddress = contact.Email;
        //    }
        //}

        //private void PopulateFrameworks()
        //{
        //    Frameworks = string.Join(',', CatalogueItem.Solution.FrameworkSolutions.Select(x => x.Framework.ShortName));
        //}
    }
}
