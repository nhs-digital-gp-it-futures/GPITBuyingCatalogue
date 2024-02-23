using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;
using NHSD.GPIT.BuyingCatalogue.Services.Capabilities;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Capabilities;

public static class Gen2UploadServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(Gen2UploadService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonAutoData]
    public static async Task GetCapabilitiesFromCsv_WithRows_ReturnsCapabilities(
        string fileName,
        Gen2UploadService service)
    {
        var stream = CreateCapabilitiesCsvStream(
            "SYN-8169,10055,10055-001,C41,A001,Passed - Full",
            "SYN-1182,10055,10055-001,C41,,Passed - Full",
            "SYN-15189,10032,10032-001,C37,,Passed - Full");

        var result = await service.GetCapabilitiesFromCsv(fileName, stream);

        result.Should().NotBeNull();
        result.FileName.Should().Be(fileName);
        result.Imported.Should().HaveCount(3);
        result.Failed.Should().BeEmpty();
    }

    [Theory]
    [CommonAutoData]
    public static async Task GetCapabilitiesFromCsv_WithNewlines_RemovesNewlinesAndReturnsCapabilities(
        string fileName,
        Gen2UploadService service)
    {
        var stream = CreateCapabilitiesCsvStream(
            "SYN-8169,10055,10055-001,C41,\"\n\nA001\",Passed - Full",
            "SYN-1182,10055,\"\n\n10055-001\",C41,,Passed - Full",
            "SYN-15189,\"\n\n10032\",10032-001,C37,,Passed - Full");

        var result = await service.GetCapabilitiesFromCsv(fileName, stream);

        result.Should().NotBeNull();
        result.FileName.Should().Be(fileName);
        result.Imported.Should().HaveCount(3);
        result.Failed.Should().BeEmpty();
    }

    [Theory]
    [CommonAutoData]
    public static async Task GetCapabilitiesFromCsv_WithMissingData_ReturnsCapabilities(
        string fileName,
        Gen2UploadService service)
    {
        var stream = CreateCapabilitiesCsvStream(
            "SYN-8169,10055,10055,C41,A001,Passed - Full",
            "SYN-1182,10055,10055-001,C41,,Passed - Full",
            "SYN-15189,10032,10032-001,,,Passed - Full");

        var result = await service.GetCapabilitiesFromCsv(fileName, stream);

        result.Should().NotBeNull();
        result.FileName.Should().Be(fileName);
        result.Imported.Should().HaveCount(3);
        result.Failed.Should().HaveCount(2);
    }

    [Theory]
    [CommonAutoData]
    public static async Task GetCapabilitiesFromCsv_WithNoRows_ReturnsEmpty(
        string fileName,
        Gen2UploadService service)
    {
        var stream = CreateCapabilitiesCsvStream();

        var result = await service.GetCapabilitiesFromCsv(fileName, stream);

        result.Should().NotBeNull();
        result.FileName.Should().Be(fileName);
        result.Imported.Should().BeEmpty();
        result.Failed.Should().BeEmpty();
    }

    [Theory]
    [CommonAutoData]
    public static async Task GetCapabilitiesFromCsv_WithMissingColumnHeaders_ReturnsNull(
        string fileName,
        Gen2UploadService service)
    {
        var stream = CreateCsvStream(
            string.Empty,
            "SYN-8169,10055,10055-001,C41,A001,Passed - Full",
            "SYN-1182,10055,10055-001,C41,,Passed - Full",
            "SYN-15189,10032,10032-001,C37,,Passed - Full");

        var result = await service.GetCapabilitiesFromCsv(fileName, stream);

        result.Should().BeNull();
    }

    [Theory]
    [CommonAutoData]
    public static async Task GetCapabilitiesFromCsv_WithMissingRowColumns_ReturnsNull(
        string fileName,
        Gen2UploadService service)
    {
        var stream = CreateCapabilitiesCsvStream(
            "SYN-8169,10055,10055-001,C41,A001,Passed - Full",
            "SYN-1182,10055-001,C41,,Passed - Full",
            "SYN-15189,10032,10032-001,C37,");

        var result = await service.GetCapabilitiesFromCsv(fileName, stream);

        result.Should().BeNull();
    }

    [Theory]
    [CommonAutoData]
    public static async Task GetEpicsFromCsv_WithRows_ReturnsEpics(
        string fileName,
        Gen2UploadService service)
    {
        var stream = CreateEpicsCsvStream(
            "SYN-15203,10032,10032-001,,C37,E00666,Passed",
            "SYN-15201,10032,10032-001,,C37,E00664,Passed",
            "SYN-15200,10032,10032-001,,C37,E00663,Passed");

        var result = await service.GetEpicsFromCsv(fileName, stream);

        result.Should().NotBeNull();
        result.FileName.Should().Be(fileName);
        result.Imported.Should().HaveCount(3);
        result.Failed.Should().BeEmpty();
    }

    [Theory]
    [CommonAutoData]
    public static async Task GetEpicsFromCsv_WithNewlines_RemovesNewlinesAndReturnsEpics(
        string fileName,
        Gen2UploadService service)
    {
        var stream = CreateEpicsCsvStream(
            "SYN-15203,10032,\"\n\n10032-001\",,C37,E00666,Passed",
            "SYN-15201,10032,10032-001,,C37,\"\n\nE00664\",Passed",
            "SYN-15200,\"\n\n10032\",10032-001,,C37,E00663,Passed");

        var result = await service.GetEpicsFromCsv(fileName, stream);

        result.Should().NotBeNull();
        result.FileName.Should().Be(fileName);
        result.Imported.Should().HaveCount(3);
        result.Failed.Should().BeEmpty();
    }

    [Theory]
    [CommonAutoData]
    public static async Task GetEpicsFromCsv_WithMissingData_ReturnsEpics(
        string fileName,
        Gen2UploadService service)
    {
        var stream = CreateEpicsCsvStream(
            "SYN-15203,10032,10032-001,,C37,E00666,Passed",
            "SYN-15201,10032,10032-001,,C37,,Passed",
            "SYN-15200,10032,10032-001,,,E00663,Passed");

        var result = await service.GetEpicsFromCsv(fileName, stream);

        result.Should().NotBeNull();
        result.FileName.Should().Be(fileName);
        result.Imported.Should().HaveCount(3);
        result.Failed.Should().HaveCount(2);
    }

    [Theory]
    [CommonAutoData]
    public static async Task GetEpicsFromCsv_WithNoRows_ReturnsEmpty(
        string fileName,
        Gen2UploadService service)
    {
        var stream = CreateEpicsCsvStream();

        var result = await service.GetEpicsFromCsv(fileName, stream);

        result.Should().NotBeNull();
        result.FileName.Should().Be(fileName);
        result.Imported.Should().BeEmpty();
        result.Failed.Should().BeEmpty();
    }

    [Theory]
    [CommonAutoData]
    public static async Task GetEpicsFromCsv_WithMissingColumnHeaders_ReturnsNull(
        string fileName,
        Gen2UploadService service)
    {
        var stream = CreateCsvStream(
            string.Empty,
            "SYN-15203,10032,10032-001,,C37,E00666,Passed",
            "SYN-15201,10032,10032-001,,C37,E00664,Passed",
            "SYN-15200,10032,10032-001,,C37,E00663,Passed");

        var result = await service.GetEpicsFromCsv(fileName, stream);

        result.Should().BeNull();
    }

    [Theory]
    [CommonAutoData]
    public static async Task GetEpicsFromCsv_WithMissingRowColumns_ReturnsNull(
        string fileName,
        Gen2UploadService service)
    {
        var stream = CreateEpicsCsvStream(
            "SYN-15203,10032,10032-001,,C37,E00666,Passed",
            "SYN-15201,10032,,C37,E00664,Passed",
            "SYN-15200,10032,10032-001,,C37,Passed");

        var result = await service.GetEpicsFromCsv(fileName, stream);

        result.Should().BeNull();
    }

    [Theory]
    [CommonAutoData]
    public static async Task AddToCache_Capabilities_AddsToCache(
        Gen2CsvImportModel<Gen2CapabilitiesCsvModel> records,
        [Frozen] Mock<IDistributedCache> distributedCache,
        Gen2UploadService service)
    {
        var expected = JsonSerializer.Serialize(records);

        _ = await service.AddToCache(records);

        distributedCache.Verify(
            x => x.SetAsync(
                It.IsAny<string>(),
                Encoding.UTF8.GetBytes(expected),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()));
    }

    [Theory]
    [CommonAutoData]
    public static async Task AddToCache_Epics_AddsToCache(
        Gen2CsvImportModel<Gen2EpicsCsvModel> records,
        [Frozen] Mock<IDistributedCache> distributedCache,
        Gen2UploadService service)
    {
        var expected = JsonSerializer.Serialize(records);

        _ = await service.AddToCache(records);

        distributedCache.Verify(
            x => x.SetAsync(
                It.IsAny<string>(),
                Encoding.UTF8.GetBytes(expected),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()));
    }

    [Theory]
    [CommonAutoData]
    public static async Task GetCachedCapabilities_ReturnsExpected(
        Gen2CsvImportModel<Gen2CapabilitiesCsvModel> expected,
        [Frozen] Mock<IDistributedCache> distributedCache,
        Gen2UploadService service)
    {
        var serialized = JsonSerializer.Serialize(expected);
        var bytes = Encoding.UTF8.GetBytes(serialized);

        distributedCache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bytes);

        var cachedCapabilities = await service.GetCachedCapabilities(Guid.NewGuid());

        cachedCapabilities.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [CommonAutoData]
    public static async Task GetCachedEpics_ReturnsExpected(
        Gen2CsvImportModel<Gen2EpicsCsvModel> expected,
        [Frozen] Mock<IDistributedCache> distributedCache,
        Gen2UploadService service)
    {
        var serialized = JsonSerializer.Serialize(expected);
        var bytes = Encoding.UTF8.GetBytes(serialized);

        distributedCache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bytes);

        var cachedCapabilities = await service.GetCachedEpics(Guid.NewGuid());

        cachedCapabilities.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [CommonAutoData]
    public static async Task WriteToCsv_Capabilities_ReturnsStream(
        List<Gen2CapabilitiesCsvModel> capabilities,
        Gen2UploadService service)
    {
        var result = await service.WriteToCsv(capabilities);

        result.Should().NotBeNull();
        result.Should().BeReadable();
    }

    [Theory]
    [CommonAutoData]
    public static async Task WriteToCsv_Epics_ReturnsStream(
        List<Gen2EpicsCsvModel> epics,
        Gen2UploadService service)
    {
        var result = await service.WriteToCsv(epics);

        result.Should().NotBeNull();
        result.Should().BeReadable();
    }

    private static MemoryStream CreateCapabilitiesCsvStream(params string[] rows) => CreateCsvStream(
        "Key,Supplier ID,Solution ID,Capability ID,Additional Service ID,Capability Assessment Result",
        rows);

    private static MemoryStream CreateEpicsCsvStream(params string[] rows) => CreateCsvStream(
        "Key,Supplier ID,Solution ID,Additional Service ID,Capability ID,Epic ID,Epic Final Assessment Result",
        rows);

    private static MemoryStream CreateCsvStream(string header, params string[] capabilityRows)
    {
        var memoryStream = new MemoryStream();
        var streamWriter = new StreamWriter(memoryStream);

        if (!string.IsNullOrWhiteSpace(header))
            streamWriter.WriteLine(header);

        foreach (var row in capabilityRows)
        {
            streamWriter.WriteLine(row);
        }

        streamWriter.Flush();
        memoryStream.Position = 0;

        return memoryStream;
    }
}
