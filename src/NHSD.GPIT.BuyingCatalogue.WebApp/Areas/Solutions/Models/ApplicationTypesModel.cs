using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class ApplicationTypesModel : SolutionDisplayBaseModel, IApplicationTypesDisplayModel
    {
        public ApplicationTypesModel(
            CatalogueItem catalogueItem,
            CatalogueItemContentStatus contentStatus)
            : base(catalogueItem, contentStatus)
        {
            ApplicationTypesDetails = new ApplicationTypesDetailsModel(catalogueItem.Solution.EnsureAndGetApplicationType());

            PaginationFooter.FullWidth = true;
        }

        public override int Index => 9;

        public ApplicationTypesDetailsModel ApplicationTypesDetails { get; }

        [UIHint("DescriptionList")]
        public DescriptionListViewModel ApplicationTypes
        {
            get => ApplicationTypesDetails.ApplicationTypes;
            init => ApplicationTypesDetails.ApplicationTypes = value;
        }

        [UIHint("DescriptionList")]
        public DescriptionListViewModel BrowserBasedApplication
        {
            get => ApplicationTypesDetails.BrowserBasedApplication;
            init => ApplicationTypesDetails.BrowserBasedApplication = value;
        }

        [UIHint("DescriptionList")]
        public DescriptionListViewModel NativeDesktopApplication
        {
            get => ApplicationTypesDetails.NativeDesktopApplication;
            init => ApplicationTypesDetails.NativeDesktopApplication = value;
        }

        [UIHint("DescriptionList")]
        public DescriptionListViewModel NativeMobileApplication
        {
            get => ApplicationTypesDetails.NativeMobileApplication;
            init => ApplicationTypesDetails.NativeMobileApplication = value;
        }

        public ApplicationTypeDetail ApplicationTypeDetail
        {
            get => ApplicationTypesDetails.ApplicationTypeDetail;
            init => ApplicationTypesDetails.ApplicationTypeDetail = value;
        }

        public bool HasApplicationType(ApplicationType applicationType) =>
            ApplicationTypesDetails.HasApplicationType(applicationType);
    }
}
