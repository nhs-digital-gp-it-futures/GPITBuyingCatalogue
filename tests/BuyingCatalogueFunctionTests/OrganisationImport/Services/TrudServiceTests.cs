using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using BuyingCatalogueFunction.OrganisationImport.Interfaces;
using BuyingCatalogueFunction.OrganisationImport.Models;
using BuyingCatalogueFunction.OrganisationImport.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using Xunit;

namespace BuyingCatalogueFunctionTests.OrganisationImport.Services;

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
}
