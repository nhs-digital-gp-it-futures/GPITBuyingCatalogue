using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Admin
{
    public sealed class SolutionLoadingStatusesModel
    {
        public TaskProgress AdditionalServices { get; init; }

        public TaskProgress AssociatedServices { get; init; }

        public TaskProgress CapabilitiesAndEpics { get; init; }

        public TaskProgress ApplicationType { get; init; }

        public TaskProgress Description { get; init; }

        public TaskProgress DevelopmentPlans { get; init; }

        public TaskProgress Features { get; init; }

        public TaskProgress HostingType { get; init; }

        public TaskProgress DataProcessing { get; init; }

        public TaskProgress Implementation { get; init; }

        public TaskProgress Interoperability { get; init; }

        public TaskProgress ListPrice { get; init; }

        public TaskProgress ServiceLevelAgreement { get; init; }

        public TaskProgress SupplierDetails { get; init; }
    }
}
