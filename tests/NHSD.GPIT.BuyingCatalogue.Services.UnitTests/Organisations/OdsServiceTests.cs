using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using Flurl.Http.Testing;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Addresses.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Services.Organisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Organisations
{
    public static class OdsServiceTests
    {
        private const string OdsCode = "XYZ";

        private const string ValidResponseBody = @"{""Organisation"": "
            + @"{""Name"": ""SOUTH EAST - H&J COMMISSIONING HUB"", "
            + @"""Date"": [{""Type"": ""Operational"", ""Start"": ""2019-10-25""}, "
            + @"{""Type"": ""Legal"", ""Start"": ""2020-04-01""}], "
            + @"""OrgId"": {""root"": ""2.16.840.1.113883.2.1.3.2.4.18.48"", ""assigningAuthorityName"": ""HSCIC"", ""extension"": ""97T""}, "
            + @"""Status"": ""Active"", ""LastChangeDate"": ""2020-04-01"", ""orgRecordClass"": ""RC1"", "
            + @"""GeoLoc"": {""Location"": {""AddrLn1"": ""C/O NHS ENGLAND"", ""AddrLn2"": ""1W09, 1ST FLOOR, QUARRY HOUSE"", "
            + @"""AddrLn3"": ""QUARRY HILL"", ""Town"": ""LEEDS"", ""PostCode"": ""LS2 7UA"", ""Country"": ""ENGLAND""}}, "
            + @"""Roles"": {""Role"": [{""id"": ""RO218"", ""uniqueRoleId"": 391223, "
            + @"""Date"": [{""Type"": ""Operational"", ""Start"": ""2019-10-25""}, "
            + @"{""Type"": ""Legal"", ""Start"": ""2020-04-01""}], ""Status"": ""Active""}, "
            + @"{""id"": ""RO98"", ""uniqueRoleId"": 386574, ""primaryRole"": true, "
            + @"""Date"": [{""Type"": ""Operational"", ""Start"": ""2019-10-25""},"
            + @" {""Type"": ""Legal"", ""Start"": ""2020-04-01""}], ""Status"": ""Active""}]}, "
            + @"""Rels"": {""Rel"": [{""Date"": [{""Type"": ""Operational"", ""Start"": ""2019-10-25""}, "
            + @"{""Type"": ""Legal"", ""Start"": ""2020-04-01""}], ""Status"": ""Active"", "
            + @"""Target"": {""OrgId"": {""root"": ""2.16.840.1.113883.2.1.3.2.4.18.48"", ""assigningAuthorityName"": ""HSCIC"", ""extension"": ""Y59""}, "
            + @"""PrimaryRoleId"": {""id"": ""RO209"", ""uniqueRoleId"": 299360}}, ""id"": ""RE5"", ""uniqueRelId"": 619596}]}, "
            + @"""Succs"": {""Succ"": [{""uniqueSuccId"": 37762, ""Date"": [{""Type"": ""Legal"", ""Start"": ""2020-04-01""}], ""Type"": ""Predecessor"", "
            + @"""Target"": {""OrgId"": {""root"": ""2.16.840.1.113883.2.1.3.2.4.18.48"", ""assigningAuthorityName"": ""HSCIC"", ""extension"": ""14W""}, "
            + @"""PrimaryRoleId"": {""id"": ""RO98"", ""uniqueRoleId"": 296831}}}, {""uniqueSuccId"": 37761, "
            + @"""Date"": [{""Type"": ""Legal"", ""Start"": ""2020-04-01""}], ""Type"": ""Predecessor"", "
            + @"""Target"": {""OrgId"": {""root"": ""2.16.840.1.113883.2.1.3.2.4.18.48"", ""assigningAuthorityName"": ""HSCIC"", ""extension"": ""14V""}, "
            + @"""PrimaryRoleId"": {""id"": ""RO98"", ""uniqueRoleId"": 296829}}}]}}}";

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OdsService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public static void GetOrganisationByOdsCode_NoOdsCode_ThrowsException(string odsCode)
        {
            var serviceMock = new Mock<IOdsService>();

            _ = Assert.ThrowsAsync<ArgumentNullException>(() => _ = serviceMock.Object.GetOrganisationByOdsCode(odsCode));
        }

        [Theory]
        [CommonAutoData]
        public static async Task GetOrganisationByOdsCode_CachedCode_ReturnsFromCache(
            string odsCode,
            OdsOrganisation organisation)
        {
            var settingsMock = new Mock<IOdsSettings>();
            var memoryCacheMock = new Mock<IMemoryCache>();

            object expectedValue = organisation;
            memoryCacheMock
                .Setup(m => m.TryGetValue($"ODS-{odsCode}", out expectedValue))
                .Returns(true);

            var service = new OdsService(settingsMock.Object, memoryCacheMock.Object);

            var (org, error) = await service.GetOrganisationByOdsCode(odsCode);

            error.Should().BeNull();
            org.Should().BeEquivalentTo(organisation);
            settingsMock.Verify(v => v.ApiBaseUrl, Times.Never);
        }

        [Fact]
        public static async Task GetBuyerOrganisationByOdsCode_WithValidResponse_Returns_BuyerOrganisation()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWith(status: 200, body: ValidResponseBody);

            var expected = new OdsOrganisation
            {
                OdsCode = OdsCode,
                OrganisationName = "SOUTH EAST - H&J COMMISSIONING HUB",
                PrimaryRoleId = "RO98",
                Address = new Address
                {
                    Line1 = "C/O NHS ENGLAND",
                    Line2 = "1W09, 1ST FLOOR, QUARRY HOUSE",
                    Line3 = "QUARRY HILL",
                    Town = "LEEDS",
                    Postcode = "LS2 7UA",
                    Country = "ENGLAND",
                },
                IsActive = true,
                IsBuyerOrganisation = true,
            };

            var settingsMock = new Mock<IOdsSettings>();
            var memoryCacheMock = new Mock<IMemoryCache>();

            object expectedValue = null;
            memoryCacheMock
                .Setup(m => m.TryGetValue($"ODS-{OdsCode}", out expectedValue))
                .Returns(false);

            memoryCacheMock.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            settingsMock.Setup(s => s.ApiBaseUrl).Returns("https://spineservice");
            settingsMock.Setup(s => s.BuyerOrganisationRoleIds).Returns(new string[] { "RO98", "RO177", "RO213", "RO272" });

            var service = new OdsService(settingsMock.Object, memoryCacheMock.Object);

            var (org, error) = await service.GetOrganisationByOdsCode(OdsCode);

            error.Should().BeNull();
            org.Should().BeOfType<OdsOrganisation>();
            org.Should().NotBeNull();
            org.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static async Task GetBuyerOrganisationByOdsCode_WithValidResponse_NotBuyerRole_ReturnsError()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWith(status: 200, body: ValidResponseBody);

            var settingsMock = new Mock<IOdsSettings>();
            var memoryCacheMock = new Mock<IMemoryCache>();

            object expectedValue = null;
            memoryCacheMock
                .Setup(x => x.TryGetValue($"ODS-{OdsCode}", out expectedValue))
                .Returns(false);

            memoryCacheMock.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            settingsMock.Setup(s => s.ApiBaseUrl).Returns("https://spineservice");
            settingsMock.Setup(s => s.BuyerOrganisationRoleIds).Returns(new string[] { "X123" });

            var service = new OdsService(settingsMock.Object, memoryCacheMock.Object);

            var (org, error) = await service.GetOrganisationByOdsCode(OdsCode);

            error.Should().Be("Not a buyer organisation");
            org.Should().BeNull();
            memoryCacheMock.Verify(v => v.CreateEntry(It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public static async Task GetBuyerOrganisationByOdsCode_WithInvalidResponse_ReturnsError()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new { ErrorCode = 404, ErrorText = "Not Found." }, 404);

            var settingsMock = new Mock<IOdsSettings>();
            var memoryCacheMock = new Mock<IMemoryCache>();

            object expectedValue = null;
            memoryCacheMock
                .Setup(m => m.TryGetValue($"ODS-{OdsCode}", out expectedValue))
                .Returns(false);

            memoryCacheMock.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            settingsMock.Setup(s => s.ApiBaseUrl).Returns("https://spineservice");

            var service = new OdsService(settingsMock.Object, memoryCacheMock.Object);

            var (org, error) = await service.GetOrganisationByOdsCode(OdsCode);

            error.Should().Be("Organisation not found");
            org.Should().BeNull();
            memoryCacheMock.Verify(v => v.CreateEntry(It.IsAny<object>()), Times.Never);
        }
    }
}
