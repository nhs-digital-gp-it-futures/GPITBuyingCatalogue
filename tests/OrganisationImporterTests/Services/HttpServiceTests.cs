using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using OrganisationImporter.Models;
using OrganisationImporter.Services;
using OrganisationImporterTests.AutoFixtureCustomizations;
using Xunit;

namespace OrganisationImporterTests.Services;

public static class HttpServiceTests
{
    [Theory]
    [AutoMoqData]
    public static async Task DownloadAsync_UnsuccessfulRequest_ReturnsNull(
        Uri url,
        [Frozen] Mock<IHttpClientFactory> httpClientFactory,
        HttpService httpService)
    {
        var stubbedClient =
            new StubbedHttpClient(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)));

        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(stubbedClient);

        var result = await httpService.DownloadAsync(url);

        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    public static async Task DownloadAsync_HttpRequestException_BubblesUpException(
        Uri url,
        [Frozen] Mock<IHttpClientFactory> httpClientFactory,
        HttpService httpService)
    {
        var stubbedClient = new StubbedHttpClient(() => throw new HttpRequestException());

        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(stubbedClient);

        await httpService.Invoking(x => x.DownloadAsync(url)).Should().ThrowAsync<HttpRequestException>();
    }

    [Theory]
    [AutoMoqData]
    public static async Task DownloadAsync_TaskCanceledException_BubblesUpException(
        Uri url,
        [Frozen] Mock<IHttpClientFactory> httpClientFactory,
        HttpService httpService)
    {
        var stubbedClient = new StubbedHttpClient(() => throw new TaskCanceledException());

        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(stubbedClient);

        await httpService.Invoking(x => x.DownloadAsync(url)).Should().ThrowAsync<TaskCanceledException>();
    }

    [Theory]
    [AutoMoqData]
    public static async Task DownloadAsync_ValidResponse_ReturnsStream(
        Uri url,
        string randomContent,
        [Frozen] Mock<IHttpClientFactory> httpClientFactory,
        HttpService httpService)
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(randomContent) };

        var stubbedClient = new StubbedHttpClient(() => Task.FromResult(httpResponseMessage));

        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(stubbedClient);

        var response = await httpService.DownloadAsync(url);

        response.Should().NotBeNull();
        response.Should().BeReadable();
        response.Should().BeSeekable();
    }
}
