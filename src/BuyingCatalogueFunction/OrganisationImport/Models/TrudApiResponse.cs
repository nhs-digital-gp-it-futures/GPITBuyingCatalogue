using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BuyingCatalogueFunction.OrganisationImport.Models;

public class TrudApiResponse
{
    [JsonPropertyName("releases")] public IEnumerable<Release> Releases { get; set; }

    public class Release
    {
        [JsonPropertyName("id")] public string Id { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("releaseDate")] public DateTime ReleaseDate { get; set; }

        [JsonPropertyName("archiveFileUrl")] public Uri ArchiveFileUrl { get; set; }
    }
}
