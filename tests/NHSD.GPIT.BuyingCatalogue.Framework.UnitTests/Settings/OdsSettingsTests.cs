using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Settings
{
    public static class OdsSettingsTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void GetOrganisationType_NullOrEmptyRole_ThrowsException(string primaryRoleId, OdsSettings settings)
        {
            Assert.Throws<ArgumentNullException>(() => settings.GetOrganisationType(primaryRoleId));
        }

        [Theory]
        [AutoData]
        public static void GetOrganisationType_NonBuyerRoleId_ThrowsException(string primaryRoleId, OdsSettings settings)
        {
            Assert.Throws<ArgumentException>(() => settings.GetOrganisationType(primaryRoleId));
        }

        [Theory]
        [AutoData]
        public static void GetOrganisationType_ValidRoleId_ReturnsOrgType(string primaryRoleId, OrganisationType orgType, OdsSettings settings)
        {
            settings.BuyerOrganisationRoles[0].PrimaryRoleId = primaryRoleId;
            settings.BuyerOrganisationRoles[0].OrganisationType = orgType;

            var result = settings.GetOrganisationType(primaryRoleId);
            result.Should().Be(orgType);
        }

        [Theory]
        [AutoData]
        public static void GetPrimaryRoleId_NonBuyerOrgType_ThrowsException(OrganisationType orgType, OdsSettings settings)
        {
            Assert.Throws<ArgumentException>(() => settings.GetPrimaryRoleId(orgType));
        }

        [Theory]
        [AutoData]
        public static void GetPrimaryRoleId_ValidOrgType_ReturnsPrimaryRole(string primaryRoleId, OrganisationType orgType, OdsSettings settings)
        {
            settings.BuyerOrganisationRoles[0].PrimaryRoleId = primaryRoleId;
            settings.BuyerOrganisationRoles[0].OrganisationType = orgType;

            var result = settings.GetPrimaryRoleId(orgType);
            result.Should().Be(primaryRoleId);
        }
    }
}
