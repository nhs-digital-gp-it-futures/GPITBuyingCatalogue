using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionApplicationType
{
    public enum MobileOrTabletApplications
    {
        [EnumMember(Value = "Supported operating system")]
        operating_systems,
        [EnumMember(Value = "Connectivity")]
        connectivity,
        [EnumMember(Value = "Memory and storage")]
        memory_and_storage,
        [EnumMember(Value = "Third-party components and device capabilities")]
        third_party_components,
        [EnumMember(Value = "Hardware requirements")]
        hardware_requirements,
        [EnumMember(Value = "Additional information")]
        additional_information,
    }
}
