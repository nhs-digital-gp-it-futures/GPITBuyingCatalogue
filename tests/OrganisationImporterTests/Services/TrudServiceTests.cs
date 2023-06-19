using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using OrganisationImporter.Interfaces;
using OrganisationImporter.Models;
using OrganisationImporter.Services;
using OrganisationImporterTests.AutoFixtureCustomizations;
using Xunit;

namespace OrganisationImporterTests.Services;

public static class TrudServiceTests
{
    [Theory]
    [AutoMoqData]
    public static async Task GetTrudData_NullTrudDataFile_ReturnsNull(
        Uri url,
        [Frozen] Mock<IZipService> zipService,
        TrudService trudService)
    {
        zipService
            .Setup(x => x.GetTrudDataFileAsync(It.IsAny<Stream>()))
            .ReturnsAsync((Stream)null);

        var result = await trudService.GetTrudDataAsync(url);

        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    public static async Task GetTrudData_InvalidXml_ReturnsNull(
        Uri url,
        string randomText,
        [Frozen] Mock<IZipService> zipService,
        TrudService trudService)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(randomText));

        zipService
            .Setup(x => x.GetTrudDataFileAsync(It.IsAny<Stream>()))
            .ReturnsAsync(stream);

        var result = await trudService.GetTrudDataAsync(url);

        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    public static async Task GetTrudData_ValidXml_ReturnsExpected(
        Uri url,
        OrgRefData trudData,
        [Frozen] Mock<IHttpService> httpService,
        [Frozen] Mock<IZipService> zipService,
        TrudService trudService)
    {
        var xml = trudData.ToXml();

        httpService
            .Setup(x => x.DownloadAsync(It.IsAny<Uri>()))
            .ReturnsAsync(xml.ToStream());

        zipService
            .Setup(x => x.GetTrudDataFileAsync(It.IsAny<Stream>()))
            .ReturnsAsync(xml.ToStream());

        var result = await trudService.GetTrudDataAsync(url);

        result.Should().BeEquivalentTo(trudData);
    }

    [Theory(Skip = "Currently fails with 'Object reference not set to an instance of an object.' because the Sqlite provider doesn't support JSON columns. Re-evaluate with EF 8")]
    [InMemoryDbAutoMoqData]
    public static async Task SaveTrudData_ValidRequest_SavesData(
        OrgRefData trudData,
        [Frozen] Mock<ILogger<TrudService>> logger,
        [Frozen] BuyingCatalogueDbContext dbContext,
        TrudService trudService)
    {
        MapRoleIds(trudData);
        var mappedData = new OdsOrganisationMapping(trudData, logger.Object);
        await trudService.SaveTrudDataAsync(mappedData);

        dbContext.OdsOrganisations.Count().Should().Be(mappedData.OdsOrganisations.Count);
    }

    private static void MapRoleIds(OrgRefData trudData)
    {
        trudData.OrganisationsRoot.Organisations.ForEach(x =>
        {
            x.RolesRoot.Roles.ForEach(y =>
            {
                y.Id = trudData.CodeSystems.CodeSystem
                    .First(z => string.Equals(z.Name, TrudCodeSystemKeys.RolesKey)).Concept.First().Id;
            });
        });
    }
}
