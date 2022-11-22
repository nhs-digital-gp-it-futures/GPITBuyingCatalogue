using OrganisationImporter.Services;
using FluentAssertions;
using Xunit;
using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Moq;
using OrganisationImporter.Interfaces;
using OrganisationImporter.Models;
using OrganisationImporterTests.AutoFixtureCustomizations;

namespace OrganisationImporterTests.Services;

public static class OrganisationImportServiceTests
{
    [Theory]
    [AutoMoqData]
    public static Task ImportFromUrl_Returns(
        Uri importUrl,
        OrganisationImportService service)
        => service.Invoking(x => x.ImportFromUrl(importUrl)).Should().NotThrowAsync();

    [Theory]
    [AutoMoqData]
    public static async Task ImportFromUrl_InvalidTrudData_DoesNotSave(
        Uri importUrl,
        [Frozen] Mock<ITrudService> trudService,
        OrganisationImportService service)
    {
        trudService
            .Setup(s => s.GetTrudDataAsync(importUrl))
            .ReturnsAsync((OrgRefData)null);

        await service.ImportFromUrl(importUrl);

        trudService.Verify(x => x.SaveTrudDataAsync(It.IsAny<OdsOrganisationMapping>()), Times.Never());
    }

    [Theory]
    [AutoMoqData]
    public static async Task ImportFromUrl_ValidTrudData_Saves(
        Uri importUrl,
        OrgRefData trudData,
        [Frozen] Mock<ITrudService> trudService,
        OrganisationImportService service)
    {
        trudService
            .Setup(s => s.GetTrudDataAsync(importUrl))
            .ReturnsAsync(trudData);

        await service.ImportFromUrl(importUrl);

        trudService.Verify(x => x.SaveTrudDataAsync(It.IsAny<OdsOrganisationMapping>()), Times.Once());
    }
}
