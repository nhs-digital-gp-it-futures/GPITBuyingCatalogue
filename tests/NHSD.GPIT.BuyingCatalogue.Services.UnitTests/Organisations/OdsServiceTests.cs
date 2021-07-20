﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
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
            var memoryCacheMock = new Mock<IMemoryCache>();

            object expectedValue = organisation;
            memoryCacheMock
                .Setup(m => m.TryGetValue($"ODS-{odsCode}", out expectedValue))
                .Returns(true);

            var settings = new OdsSettings();

            var service = new OdsService(settings, memoryCacheMock.Object);

            var (org, error) = await service.GetOrganisationByOdsCode(odsCode);

            error.Should().BeNull();
            org.Should().BeEquivalentTo(organisation);
            memoryCacheMock.Verify(v => v.CreateEntry(It.IsAny<object>()), Times.Never);
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

            var memoryCacheMock = new Mock<IMemoryCache>();

            object expectedValue = null;
            memoryCacheMock
                .Setup(m => m.TryGetValue($"ODS-{OdsCode}", out expectedValue))
                .Returns(false);

            memoryCacheMock.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            var settings = new OdsSettings
            {
                ApiBaseUrl = "https://spineservice",
                BuyerOrganisationRoleIds = new[] { "RO98", "RO177", "RO213", "RO272" },
            };

            var service = new OdsService(settings, memoryCacheMock.Object);

            var (org, error) = await service.GetOrganisationByOdsCode(OdsCode);

            error.Should().BeNull();
            org.Should().BeOfType<OdsOrganisation>();
            org.Should().NotBeNull();
            org.Should().BeEquivalentTo(expected);
            memoryCacheMock.Verify(v => v.CreateEntry(It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public static async Task GetBuyerOrganisationByOdsCode_WithValidResponse_NotBuyerRole_ReturnsError()
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWith(status: 200, body: ValidResponseBody);

            var memoryCacheMock = new Mock<IMemoryCache>();

            object expectedValue = null;
            memoryCacheMock
                .Setup(m => m.TryGetValue($"ODS-{OdsCode}", out expectedValue))
                .Returns(false);

            memoryCacheMock.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            var settings = new OdsSettings
            {
                ApiBaseUrl = "https://spineservice",
                BuyerOrganisationRoleIds = new[] { "X123" },
            };

            var service = new OdsService(settings, memoryCacheMock.Object);

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

            var memoryCacheMock = new Mock<IMemoryCache>();

            object expectedValue = null;
            memoryCacheMock
                .Setup(m => m.TryGetValue($"ODS-{OdsCode}", out expectedValue))
                .Returns(false);

            memoryCacheMock.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            var settings = new OdsSettings
            {
                ApiBaseUrl = "https://spineservice",
            };

            var service = new OdsService(settings, memoryCacheMock.Object);

            var (org, error) = await service.GetOrganisationByOdsCode(OdsCode);

            error.Should().Be("Organisation not found");
            org.Should().BeNull();
            memoryCacheMock.Verify(v => v.CreateEntry(It.IsAny<object>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public static void GetServiceRecipientsByParentOdsCode_NoOdsCode_ThrowsException(string odsCode)
        {
            var serviceMock = new Mock<IOdsService>();

            _ = Assert.ThrowsAsync<ArgumentNullException>(() => _ = serviceMock.Object.GetServiceRecipientsByParentOdsCode(odsCode));
        }

        [Theory]
        [CommonAutoData]
        public static async Task GetServiceRecipientsByParentOdsCode_CachedCode_ReturnsFromCache(
            string odsCode,
            IEnumerable<ServiceRecipient> serviceRecipients)
        {
            var memoryCacheMock = new Mock<IMemoryCache>();

            object expectedValue = serviceRecipients;
            memoryCacheMock
                .Setup(m => m.TryGetValue($"ServiceRecipients-ODS-{odsCode}", out expectedValue))
                .Returns(true);

            var settings = new OdsSettings();

            var service = new OdsService(settings, memoryCacheMock.Object);

            var result = await service.GetServiceRecipientsByParentOdsCode(odsCode);

            result.Should().BeEquivalentTo(serviceRecipients);
            memoryCacheMock.Verify(v => v.CreateEntry(It.IsAny<object>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task GetServiceRecipientsByParentOdsCode_SinglePage_ReturnsOrganisation(
            string odsCode,
            ServiceRecipient childOrg)
        {
            childOrg.PrimaryRoleId = "RO177";

            var json = CreatePageJson(childOrg);

            using var httpTest = new HttpTest();
            httpTest.RespondWith(status: 200, body: json);

            var memoryCacheMock = new Mock<IMemoryCache>();

            object expectedValue = null;
            memoryCacheMock
                .Setup(m => m.TryGetValue($"ServiceRecipients-ODS-{odsCode}", out expectedValue))
                .Returns(false);

            memoryCacheMock.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            var settings = new OdsSettings
            {
                ApiBaseUrl = "https://spineservice",
                GetChildOrganisationSearchLimit = 2,
                GpPracticeRoleId = "RO177",
            };

            var service = new OdsService(settings, memoryCacheMock.Object);

            var result = await service.GetServiceRecipientsByParentOdsCode(odsCode);

            result.Should().BeEquivalentTo(childOrg);
            memoryCacheMock.Verify(v => v.CreateEntry(It.IsAny<object>()), Times.Once);
        }

        [Theory]
        [CommonAutoData]
        public static async Task GetServiceRecipientsByParentOdsCode_MultiplePages_ReturnsOrganisation(
            string odsCode,
            ServiceRecipient childOne,
            ServiceRecipient childTwo)
        {
            childOne.PrimaryRoleId = "RO177";
            childTwo.PrimaryRoleId = "RO177";
            var jsonPageOne = CreatePageJson(childOne);
            var jsonPageTwo = CreatePageJson(childTwo);
            var jsonPageThree = CreatePageJson();

            using var httpTest = new HttpTest();
            httpTest.RespondWith(status: 200, body: jsonPageOne)
            .RespondWith(status: 200, body: jsonPageTwo)
            .RespondWith(status: 200, body: jsonPageThree);

            var memoryCacheMock = new Mock<IMemoryCache>();

            object expectedValue = null;
            memoryCacheMock
                .Setup(m => m.TryGetValue($"ServiceRecipients-ODS-{odsCode}", out expectedValue))
                .Returns(false);

            memoryCacheMock.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            var settings = new OdsSettings
            {
                ApiBaseUrl = "https://spineservice",
                GetChildOrganisationSearchLimit = 1,
                GpPracticeRoleId = "RO177",
            };

            var service = new OdsService(settings, memoryCacheMock.Object);

            var result = await service.GetServiceRecipientsByParentOdsCode(odsCode);

            result.Should().BeEquivalentTo(childOne, childTwo);
            memoryCacheMock.Verify(v => v.CreateEntry(It.IsAny<object>()), Times.Once);
        }

        [Theory]
        [CommonAutoData]
        public static async Task GetServiceRecipientsByParentOdsCode_SinglePageDifferentRoleIds_ReturnsOnlyMatching(
            string odsCode,
            ServiceRecipient childOne,
            ServiceRecipient childTwo)
        {
            childOne.PrimaryRoleId = "RO177";
            childTwo.PrimaryRoleId = "RO178";
            var jsonPageOne = CreatePageJson(childOne, childTwo);

            using var httpTest = new HttpTest();
            httpTest.RespondWith(status: 200, body: jsonPageOne);

            var memoryCacheMock = new Mock<IMemoryCache>();

            object expectedValue = null;
            memoryCacheMock
                .Setup(m => m.TryGetValue($"ServiceRecipients-ODS-{odsCode}", out expectedValue))
                .Returns(false);

            memoryCacheMock.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            var settings = new OdsSettings
            {
                ApiBaseUrl = "https://spineservice",
                GetChildOrganisationSearchLimit = 3,
                GpPracticeRoleId = "RO177",
            };

            var service = new OdsService(settings, memoryCacheMock.Object);

            var result = await service.GetServiceRecipientsByParentOdsCode(odsCode);

            result.Should().BeEquivalentTo(childOne);
            memoryCacheMock.Verify(v => v.CreateEntry(It.IsAny<object>()), Times.Once);
        }

        [Theory]
        [CommonAutoData]
        public static async Task GetServiceRecipientsByParentOdsCode_NoOrganisations_ReturnsEmptyList(
            string odsCode)
        {
            using var httpTest = new HttpTest();
            httpTest.RespondWith(status: 200, body: CreatePageJson());

            var memoryCacheMock = new Mock<IMemoryCache>();

            object expectedValue = null;
            memoryCacheMock
                .Setup(m => m.TryGetValue($"ServiceRecipients-ODS-{odsCode}", out expectedValue))
                .Returns(false);

            memoryCacheMock.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            var settings = new OdsSettings
            {
                ApiBaseUrl = "https://spineservice",
                GetChildOrganisationSearchLimit = 3,
                GpPracticeRoleId = "RO177",
            };

            var service = new OdsService(settings, memoryCacheMock.Object);

            var result = await service.GetServiceRecipientsByParentOdsCode(odsCode);

            result.Should().BeEmpty();
            memoryCacheMock.Verify(v => v.CreateEntry(It.IsAny<object>()), Times.Once);
        }

        private static string CreatePageJson(params ServiceRecipient[] serviceRecipients)
        {
            var recipientJson = serviceRecipients.Select(r => JsonSerializer.Serialize(r));
            var json = string.Join(',', recipientJson);

            return $@"{{""Organisations"": [{json}]}}";
        }
    }
}
