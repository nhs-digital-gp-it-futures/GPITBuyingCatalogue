using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using OrganisationImporter.Interfaces;
using OrganisationImporter.Models;
using OrganisationImporter.Services;
using Xunit;

namespace OrganisationImporterTests.Services;

public static class TrudServiceTests
{
    [Theory]
    [MockAutoData]
    public static async Task GetTrudData_NullTrudDataFile_ReturnsNull(
        Uri url,
        [Frozen] IZipService zipService,
        TrudService trudService)
    {
        zipService.GetTrudDataFileAsync(Arg.Any<Stream>()).Returns((Stream)null);

        var result = await trudService.GetTrudDataAsync(url);

        result.Should().BeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task GetTrudData_InvalidXml_ReturnsNull(
        Uri url,
        string randomText,
        [Frozen] IZipService zipService,
        TrudService trudService)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(randomText));

        zipService.GetTrudDataFileAsync(Arg.Any<Stream>()).Returns(stream);

        var result = await trudService.GetTrudDataAsync(url);

        result.Should().BeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task GetTrudData_ValidXml_ReturnsExpected(
        Uri url,
        OrgRefData trudData,
        [Frozen] IHttpService httpService,
        [Frozen] IZipService zipService,
        TrudService trudService)
    {
        var xml = trudData.ToXml();

        httpService.DownloadAsync(Arg.Any<Uri>()).Returns(xml.ToStream());

        zipService.GetTrudDataFileAsync(Arg.Any<Stream>()).Returns(xml.ToStream());

        var result = await trudService.GetTrudDataAsync(url);

        result.Should().BeEquivalentTo(trudData);
    }

    [Theory(Skip = "Currently fails with 'Object reference not set to an instance of an object.' because the Sqlite provider doesn't support JSON columns. Re-evaluate with EF 8")]
    [MockInMemoryDbAutoData]
    public static async Task SaveTrudData_ValidRequest_SavesData(
        OrgRefData trudData,
        [Frozen] ILogger<TrudService> logger,
        [Frozen] BuyingCatalogueDbContext dbContext,
        TrudService trudService)
    {
        MapRoleIds(trudData);
        var mappedData = new OdsOrganisationMapping(trudData, logger);
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
