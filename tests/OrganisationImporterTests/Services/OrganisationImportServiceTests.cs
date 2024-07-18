using OrganisationImporter.Services;
using FluentAssertions;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using OrganisationImporter.Interfaces;
using OrganisationImporter.Models;

namespace OrganisationImporterTests.Services;

public static class OrganisationImportServiceTests
{
    [Theory]
    [MockAutoData]
    public static Task ImportFromUrl_Returns(
        Uri importUrl,
        OrganisationImportService service)
        => service.Invoking(x => x.ImportFromUrl(importUrl)).Should().NotThrowAsync();

    [Theory]
    [MockAutoData]
    public static async Task ImportFromUrl_InvalidTrudData_DoesNotSave(
        Uri importUrl,
        [Frozen] ITrudService trudService,
        OrganisationImportService service)
    {
        trudService.GetTrudDataAsync(importUrl).Returns((OrgRefData)null);

        await service.ImportFromUrl(importUrl);

        await trudService.DidNotReceive().SaveTrudDataAsync(Arg.Any<OdsOrganisationMapping>());
    }

    [Theory]
    [MockAutoData]
    public static async Task ImportFromUrl_ValidTrudData_Saves(
        Uri importUrl,
        OrgRefData trudData,
        [Frozen] ITrudService trudService,
        OrganisationImportService service)
    {
        trudData.CodeSystems = new()
        {
            CodeSystem = new List<CodeSystem>
            {
                new() { Name = TrudCodeSystemKeys.RolesKey, Concept = Enumerable.Empty<Concept>().ToList() },
                new() { Name = TrudCodeSystemKeys.RelationshipKey, Concept = Enumerable.Empty<Concept>().ToList() },
            }
        };

        trudService.GetTrudDataAsync(importUrl).Returns(trudData);

        await service.ImportFromUrl(importUrl);

        await trudService.Received().SaveTrudDataAsync(Arg.Any<OdsOrganisationMapping>());
    }
}
