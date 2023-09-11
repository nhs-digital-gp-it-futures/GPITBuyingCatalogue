using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.ServiceHelpers
{
    public static class SolutionsFilterHelperTests
    {
        [Fact]
        public static void ParseCapabilityAndEpics_Empty()
        {
            var input = new Dictionary<int, string[]>() { };

            var result = SolutionsFilterHelper.ParseCapabilityAndEpicIds(input.ToFilterString());

            var expected = new Dictionary<int, string[]>() { };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseCapabilityAndEpics_CapabilityId_Epics_Null()
        {
            var input = new Dictionary<int, string[]>() { { 1, null } };

            var result = SolutionsFilterHelper.ParseCapabilityAndEpicIds(input.ToFilterString());

            var expected = new Dictionary<int, string[]>() { { 1, Array.Empty<string>() } };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseCapabilityAndEpics_CapabilityId_Epics_Empty()
        {
            var input = new Dictionary<int, string[]>() { { 1, Array.Empty<string>() } };

            var result = SolutionsFilterHelper.ParseCapabilityAndEpicIds(input.ToFilterString());

            var expected = new Dictionary<int, string[]>() { { 1, Array.Empty<string>() } };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseCapabilityAndEpics_Capability_With_Epics()
        {
            var input = new Dictionary<int, string[]>()
            {
                { 1, new string[] { "Epic1", "Epic2" } },
                { 2, new string[] { "Epic1", "Epic3" } },
            };

            var result = SolutionsFilterHelper.ParseCapabilityAndEpicIds(input.ToFilterString());

            var expected = new Dictionary<int, string[]>()
            {
                { 1, new string[] { "Epic1", "Epic2" } },
                { 2, new string[] { "Epic1", "Epic3" } },
            };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseApplicationTypeIds_OneItemNotParseable_GeneratesResults()
        {
            var input = "0.1.hello.2";

            var result = SolutionsFilterHelper.ParseApplicationTypeIds(input);

            var expected = new List<ApplicationType> { ApplicationType.BrowserBased, ApplicationType.Desktop, ApplicationType.MobileTablet };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseApplicationTypeIds_OneItemNotInEnum_GeneratesResults()
        {
            var input = "0.1.2.6";

            var result = SolutionsFilterHelper.ParseApplicationTypeIds(input);

            var expected = new List<ApplicationType> { ApplicationType.BrowserBased, ApplicationType.Desktop, ApplicationType.MobileTablet };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseApplicationTypeIds_EmptyAndWhiteSpace_GeneratesResults()
        {
            var input = "0.1. .2..    ";

            var result = SolutionsFilterHelper.ParseApplicationTypeIds(input);

            var expected = new List<ApplicationType> { ApplicationType.BrowserBased, ApplicationType.Desktop, ApplicationType.MobileTablet };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseApplicationTypeIds_RandomInput_GeneratesResults()
        {
            var input = "iogjhoiudfhjgouhouhagdf souihadsfgouihdsfg";

            var result = SolutionsFilterHelper.ParseApplicationTypeIds(input);

            var expected = new List<ApplicationType>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseApplicationTypeIds_NullString_GeneratesResults()
        {
            var input = string.Empty;

            var result = SolutionsFilterHelper.ParseApplicationTypeIds(input);

            var expected = new List<ApplicationType>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseApplicationTypeIds_CorrectInput_GeneratesResults()
        {
            var input = "0.1.2";

            var result = SolutionsFilterHelper.ParseApplicationTypeIds(input);

            var expected = new List<ApplicationType> { ApplicationType.BrowserBased, ApplicationType.Desktop, ApplicationType.MobileTablet };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseHostingTypeIds_OneItemNotParseable_GeneratesResults()
        {
            var input = "0.1.hello.2.3";

            var result = SolutionsFilterHelper.ParseHostingTypeIds(input);

            var expected = new List<HostingType> { HostingType.PublicCloud, HostingType.PrivateCloud, HostingType.Hybrid, HostingType.OnPremise };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseHostingTypeIds_OneItemNotInEnum_GeneratesResults()
        {
            var input = "0.1.2.6.3";

            var result = SolutionsFilterHelper.ParseHostingTypeIds(input);

            var expected = new List<HostingType> { HostingType.PublicCloud, HostingType.PrivateCloud, HostingType.Hybrid, HostingType.OnPremise };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseHostingTypeIds_EmptyAndWhiteSpace_GeneratesResults()
        {
            var input = "0.1. .2..    .3";

            var result = SolutionsFilterHelper.ParseHostingTypeIds(input);

            var expected = new List<HostingType> { HostingType.PublicCloud, HostingType.PrivateCloud, HostingType.Hybrid, HostingType.OnPremise };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseHostingTypeIds_RandomInput_GeneratesResults()
        {
            var input = "iogjhoiudfhjgouhouhagdf souihadsfgouihdsfg";

            var result = SolutionsFilterHelper.ParseHostingTypeIds(input);

            var expected = new List<ApplicationType>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseHostingTypeIds_NullString_GeneratesResults()
        {
            var input = string.Empty;

            var result = SolutionsFilterHelper.ParseHostingTypeIds(input);

            var expected = new List<ApplicationType>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseHostingTypeIds_CorrectInput_GeneratesResults()
        {
            var input = "0.1.2.3";

            var result = SolutionsFilterHelper.ParseHostingTypeIds(input);

            var expected = new List<HostingType> { HostingType.PublicCloud, HostingType.PrivateCloud, HostingType.Hybrid, HostingType.OnPremise };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseInteropIm1IntegrationsIds_OneItemNotParseable_GeneratesResults()
        {
            var input = "0.1.hello.2.3";

            var result = SolutionsFilterHelper.ParseInteropIm1IntegrationsIds(input);

            var expected = new List<InteropIm1IntegrationType> { InteropIm1IntegrationType.Bulk, InteropIm1IntegrationType.Transactional, InteropIm1IntegrationType.Patient_Facing };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseInteropIm1IntegrationsIds_OneItemNotInEnum_GeneratesResults()
        {
            var input = "0.1.2.6.3";

            var result = SolutionsFilterHelper.ParseInteropIm1IntegrationsIds(input);

            var expected = new List<InteropIm1IntegrationType> { InteropIm1IntegrationType.Bulk, InteropIm1IntegrationType.Transactional, InteropIm1IntegrationType.Patient_Facing };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseInteropIm1IntegrationsIds_EmptyAndWhiteSpace_GeneratesResults()
        {
            var input = "0.1. .2..    .3";

            var result = SolutionsFilterHelper.ParseInteropIm1IntegrationsIds(input);

            var expected = new List<InteropIm1IntegrationType> { InteropIm1IntegrationType.Bulk, InteropIm1IntegrationType.Transactional, InteropIm1IntegrationType.Patient_Facing };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseInteropIm1IntegrationsIds_RandomInput_GeneratesResults()
        {
            var input = "iogjhoiudfhjgouhouhagdf souihadsfgouihdsfg";

            var result = SolutionsFilterHelper.ParseInteropIm1IntegrationsIds(input);

            var expected = new List<InteropIm1IntegrationType>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseInteropIm1IntegrationsIds_NullString_GeneratesResults()
        {
            var input = string.Empty;

            var result = SolutionsFilterHelper.ParseInteropIm1IntegrationsIds(input);

            var expected = new List<InteropIm1IntegrationType>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseInteropIm1IntegrationsIds_CorrectInput_GeneratesResults()
        {
            var input = "0.1.2.3";

            var result = SolutionsFilterHelper.ParseInteropIm1IntegrationsIds(input);

            var expected = new List<InteropIm1IntegrationType> { InteropIm1IntegrationType.Bulk, InteropIm1IntegrationType.Transactional, InteropIm1IntegrationType.Patient_Facing };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseInteropGpConnectIntegrationsIds_OneItemNotParseable_GeneratesResults()
        {
            var input = "0.1.hello.2.3";

            var result = SolutionsFilterHelper.ParseInteropGpConnectIntegrationsIds(input);

            var expected = new List<InteropGpConnectIntegrationType> { InteropGpConnectIntegrationType.HTML_View, InteropGpConnectIntegrationType.Appointment_Booking, InteropGpConnectIntegrationType.Structured_Record };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseInteropGpConnectIntegrationsIds_OneItemNotInEnum_GeneratesResults()
        {
            var input = "0.1.2.6.3";

            var result = SolutionsFilterHelper.ParseInteropGpConnectIntegrationsIds(input);

            var expected = new List<InteropGpConnectIntegrationType> { InteropGpConnectIntegrationType.HTML_View, InteropGpConnectIntegrationType.Appointment_Booking, InteropGpConnectIntegrationType.Structured_Record };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseInteropGpConnectIntegrationsIds_EmptyAndWhiteSpace_GeneratesResults()
        {
            var input = "0.1. .2..    .3";

            var result = SolutionsFilterHelper.ParseInteropGpConnectIntegrationsIds(input);

            var expected = new List<InteropGpConnectIntegrationType> { InteropGpConnectIntegrationType.HTML_View, InteropGpConnectIntegrationType.Appointment_Booking, InteropGpConnectIntegrationType.Structured_Record };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseInteropGpConnectIntegrationsIds_RandomInput_GeneratesResults()
        {
            var input = "iogjhoiudfhjgouhouhagdf souihadsfgouihdsfg";

            var result = SolutionsFilterHelper.ParseInteropGpConnectIntegrationsIds(input);

            var expected = new List<InteropGpConnectIntegrationType>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseInteropGpConnectIntegrationsIds_NullString_GeneratesResults()
        {
            var input = string.Empty;

            var result = SolutionsFilterHelper.ParseInteropGpConnectIntegrationsIds(input);

            var expected = new List<InteropGpConnectIntegrationType>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseInteropGpConnectIntegrationsIds_CorrectInput_GeneratesResults()
        {
            var input = "0.1.2.3";

            var result = SolutionsFilterHelper.ParseInteropGpConnectIntegrationsIds(input);

            var expected = new List<InteropGpConnectIntegrationType> { InteropGpConnectIntegrationType.HTML_View, InteropGpConnectIntegrationType.Appointment_Booking, InteropGpConnectIntegrationType.Structured_Record };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseInteropIntegrationTypeIds_OneItemNotParseable_GeneratesResults()
        {
            var input = "0.1.hello.2.3";

            var result = SolutionsFilterHelper.ParseInteropIntegrationTypeIds(input);

            var expected = new List<InteropIntegrationType> { InteropIntegrationType.Im1, InteropIntegrationType.GpConnect };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseInteropIntegrationTypeIds_OneItemNotInEnum_GeneratesResults()
        {
            var input = "0.1.2.6.3";

            var result = SolutionsFilterHelper.ParseInteropIntegrationTypeIds(input);

            var expected = new List<InteropIntegrationType> { InteropIntegrationType.Im1, InteropIntegrationType.GpConnect };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseInteropIntegrationTypeIds_EmptyAndWhiteSpace_GeneratesResults()
        {
            var input = "0.1. .2..    .3";

            var result = SolutionsFilterHelper.ParseInteropIntegrationTypeIds(input);

            var expected = new List<InteropIntegrationType> { InteropIntegrationType.Im1, InteropIntegrationType.GpConnect };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseInteropIntegrationTypeIds_RandomInput_GeneratesResults()
        {
            var input = "iogjhoiudfhjgouhouhagdf souihadsfgouihdsfg";

            var result = SolutionsFilterHelper.ParseInteropIntegrationTypeIds(input);

            var expected = new List<InteropIntegrationType>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseInteropIntegrationTypeIds_NullString_GeneratesResults()
        {
            var input = string.Empty;

            var result = SolutionsFilterHelper.ParseInteropIntegrationTypeIds(input);

            var expected = new List<InteropIntegrationType>();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void ParseInteropIntegrationTypeIds_CorrectInput_GeneratesResults()
        {
            var input = "0.1.2.3";

            var result = SolutionsFilterHelper.ParseInteropIntegrationTypeIds(input);

            var expected = new List<InteropIntegrationType> { InteropIntegrationType.Im1, InteropIntegrationType.GpConnect };

            result.Should().BeEquivalentTo(expected);
        }
    }
}
