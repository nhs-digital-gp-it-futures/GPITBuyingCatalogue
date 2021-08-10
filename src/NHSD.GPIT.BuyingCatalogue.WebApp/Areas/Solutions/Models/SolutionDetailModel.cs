using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionDetailModel : NavBaseModel
    {
        public SolutionDetailModel(CatalogueItem catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            CatalogueItem = catalogueItem;
            Features = catalogueItem.Solution.GetFeatures();
            PopulateContactInformation();
            PopulateFrameworks();
            PopulateCapabilities();
            BrowserBased = new ClientApplicationTypeModel(ClientApplicationTypeModel.ClientApplicationType.BrowserBased, CatalogueItem.Solution.GetClientApplication());
            NativeMobile = new ClientApplicationTypeModel(ClientApplicationTypeModel.ClientApplicationType.NativeMobile, CatalogueItem.Solution.GetClientApplication());
            NativeDesktop = new ClientApplicationTypeModel(ClientApplicationTypeModel.ClientApplicationType.NativeDesktop, CatalogueItem.Solution.GetClientApplication());
            PublicCloud = new HostingTypeModel(CatalogueItem.Solution.GetHosting().PublicCloud);
            PrivateCloud = new HostingTypeModel(CatalogueItem.Solution.GetHosting().PrivateCloud);
            HybridHostingType = new HostingTypeModel(CatalogueItem.Solution.GetHosting().HybridHostingType);
            OnPremise = new HostingTypeModel(CatalogueItem.Solution.GetHosting().OnPremise);
        }

        public List<Address> Contacts { get; set; }

        public CatalogueItem CatalogueItem { get; }

        public string Frameworks { get; private set; }

        public string[] Features { get; }

        public ClientApplicationTypeModel BrowserBased { get; set; }

        public ClientApplicationTypeModel NativeMobile { get; set; }

        public ClientApplicationTypeModel NativeDesktop { get; set; }

        public bool DisplayClientApplicationTypes
        {
            get { return BrowserBased.DisplayClientApplication || NativeMobile.DisplayClientApplication || NativeDesktop.DisplayClientApplication; }
        }

        public bool DisplayHostingType
        {
            get { return PublicCloud.DisplayHostingType || PrivateCloud.DisplayHostingType || HybridHostingType.DisplayHostingType || OnPremise.DisplayHostingType; }
        }

        public HostingTypeModel PublicCloud { get; set; }

        public HostingTypeModel PrivateCloud { get; set; }

        public HostingTypeModel HybridHostingType { get; set; }

        public HostingTypeModel OnPremise { get; set; }

        public bool DisplayPublicCloud
        {
            get
            {
                var publicCloud = CatalogueItem.Solution.GetHosting().PublicCloud;

                if ((publicCloud != null && !string.IsNullOrWhiteSpace(publicCloud.Summary)) ||
                    !string.IsNullOrWhiteSpace(publicCloud.Link) ||
                    !string.IsNullOrWhiteSpace(publicCloud.RequiresHscn))
                    return true;

                return false;
            }
        }

        public bool DisplayPublicCloudSummary
        {
            get
            {
                var publicCloud = CatalogueItem.Solution.GetHosting().PublicCloud;

                if ((publicCloud != null && !string.IsNullOrWhiteSpace(publicCloud.Summary)) ||
                    !string.IsNullOrWhiteSpace(publicCloud.Link))
                    return true;

                return false;
            }
        }

        public CatalogueItemCapabilitiesModel[] Capabilities { get; set; }

        public bool DisplayContacts
        {
            get { return CatalogueItem.Solution.MarketingContacts.Any(); }
        }

        public bool DisplaySupplier
        {
            get
            {
                return !string.IsNullOrWhiteSpace(CatalogueItem.Supplier.Summary) ||
                    !string.IsNullOrWhiteSpace(CatalogueItem.Supplier.SupplierUrl);
            }
        }

        private void PopulateContactInformation()
        {
           if (CatalogueItem.Solution.MarketingContacts.Any())
           {
                var contact = CatalogueItem.Solution.MarketingContacts.First();
                Contacts = new List<Address>
                {
                    new Address { Line1 = $"{contact.FirstName} {contact.LastName}", Line2 = contact.Department, Line3 = contact.PhoneNumber, Line4 = contact.Email },
                };

                if (CatalogueItem.Solution.MarketingContacts.Count > 1)
                {
                    contact = CatalogueItem.Solution.MarketingContacts.Skip(1).First();
                    Contacts.Add(new Address { Line1 = $"{contact.FirstName} {contact.LastName}", Line2 = contact.Department, Line3 = contact.PhoneNumber, Line4 = contact.Email });
                }
            }
        }

        private void PopulateFrameworks()
        {
            Frameworks = string.Join(',', CatalogueItem.Solution.FrameworkSolutions.Select(f => f.Framework.ShortName));
        }

        private void PopulateCapabilities()
        {
            var capabilities = new List<CatalogueItemCapabilitiesModel>();

            foreach (var capability in CatalogueItem.CatalogueItemCapabilities.OrderBy(cic => cic.Capability.Name))
            {
                capabilities.Add(new CatalogueItemCapabilitiesModel(capability, CatalogueItem));
            }

            Capabilities = capabilities.ToArray();
        }
    }
}
