using OrganisationImporter.Services;
using FluentAssertions;
using Xunit;
using System;
using System.Threading.Tasks;

namespace OrganisationImporterTests.Services;

public static class OrganisationImportServiceTests
{
    [Theory]
    [AutoMoqData]
    public static Task ImportFromUrl_Returns(
        Uri importUrl,
        OrganisationImportService service)
        => service.Invoking(x => x.ImportFromUrl(importUrl)).Should().NotThrowAsync();
}