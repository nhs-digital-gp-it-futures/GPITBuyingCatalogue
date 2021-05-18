using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
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

        // MJRTODO - Make this private and add appropriate properties
        public CatalogueItem CatalogueItem { get; private set; }

        public string Frameworks { get; private set; }

        public string[] Features { get; private set; }

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

        public SolutionCapabilitiesModel[] Capabilities { get; set; }

        public string Contact1Name { get; private set; }

        public string Contact1Department { get; private set; }

        public string Contact1PhoneNumber { get; private set; }

        public string Contact1EmailAddress { get; private set; }

        public string Contact2Name { get; private set; }

        public string Contact2Department { get; private set; }

        public string Contact2PhoneNumber { get; private set; }

        public string Contact2EmailAddress { get; private set; }

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
                Contact1Name = $"{contact.FirstName} {contact.LastName}";
                Contact1Department = contact.Department;
                Contact1PhoneNumber = contact.PhoneNumber;
                Contact1EmailAddress = contact.Email;

                if (CatalogueItem.Solution.MarketingContacts.Count > 1)
                {
                    contact = CatalogueItem.Solution.MarketingContacts.Skip(1).First();
                    Contact2Name = $"{contact.FirstName} {contact.LastName}";
                    Contact2Department = contact.Department;
                    Contact2PhoneNumber = contact.PhoneNumber;
                    Contact2EmailAddress = contact.Email;
                }
            }
        }

        private void PopulateFrameworks()
        {
            Frameworks = string.Join(',', CatalogueItem.Solution.FrameworkSolutions.Select(x => x.Framework.ShortName));
        }

        private void PopulateCapabilities()
        {
            var capabilities = new List<SolutionCapabilitiesModel>();

            foreach (var capability in CatalogueItem.Solution.SolutionCapabilities.OrderBy(x => x.Capability.Name))
            {
                capabilities.Add(new SolutionCapabilitiesModel(capability, CatalogueItem.Solution));
            }

            Capabilities = capabilities.ToArray();
        }
    }
}
