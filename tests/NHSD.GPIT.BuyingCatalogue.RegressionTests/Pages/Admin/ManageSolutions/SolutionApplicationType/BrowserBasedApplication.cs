using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Admin.ManageSolutions.SolutionApplicationType
{
    public enum BrowserBasedApplication
    {
        [EnumMember(Value = "Supported browsers")]
        supported_browser,
        [EnumMember(Value = "Plug-ins or extensions")]
        plug_ins_or_extensions,
        [EnumMember(Value = "Connectivity and resolution")]
        connectivity_and_resolution,
        [EnumMember(Value = "Hardware requirements")]
        hardware_requirementsn,
        [EnumMember(Value = "Additional information")]
        additional_information,
    }
}
