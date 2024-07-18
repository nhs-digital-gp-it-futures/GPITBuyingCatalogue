using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using OrganisationImporter.Services;
using Xunit;

namespace OrganisationImporterTests.Services;

public static class HttpServiceTests
{
    [Theory]
    [MockAutoData]
    public static async Task DownloadAsync_UnsuccessfulRequest_ReturnsNull(
        Uri url,
        [Frozen] IHttpClientFactory httpClientFactory,
        HttpService httpService)
    {
        var stubbedClient =
            new StubbedHttpClient(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)));

        httpClientFactory.CreateClient(Arg.Any<string>()).Returns(stubbedClient);

        var result = await httpService.DownloadAsync(url);

        result.Should().BeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task DownloadAsync_HttpRequestException_BubblesUpException(
        Uri url,
        [Frozen] IHttpClientFactory httpClientFactory,
        HttpService httpService)
    {
        var stubbedClient = new StubbedHttpClient(() => throw new HttpRequestException());

        httpClientFactory.CreateClient(Arg.Any<string>()).Returns(stubbedClient);

        await httpService.Invoking(x => x.DownloadAsync(url)).Should().ThrowAsync<HttpRequestException>();
    }

    [Theory]
    [MockAutoData]
    public static async Task DownloadAsync_TaskCanceledException_BubblesUpException(
        Uri url,
        [Frozen] IHttpClientFactory httpClientFactory,
        HttpService httpService)
    {
        var stubbedClient = new StubbedHttpClient(() => throw new TaskCanceledException());

        httpClientFactory.CreateClient(Arg.Any<string>()).Returns(stubbedClient);

        await httpService.Invoking(x => x.DownloadAsync(url)).Should().ThrowAsync<TaskCanceledException>();
    }

    [Theory]
    [MockAutoData]
    public static async Task DownloadAsync_ValidResponse_ReturnsStream(
        Uri url,
        string randomContent,
        [Frozen] IHttpClientFactory httpClientFactory,
        HttpService httpService)
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(randomContent) };

        var stubbedClient = new StubbedHttpClient(() => Task.FromResult(httpResponseMessage));

        httpClientFactory.CreateClient(Arg.Any<string>()).Returns(stubbedClient);

        var response = await httpService.DownloadAsync(url);

        response.Should().NotBeNull();
        response.Should().BeReadable();
        response.Should().BeSeekable();
    }
}
