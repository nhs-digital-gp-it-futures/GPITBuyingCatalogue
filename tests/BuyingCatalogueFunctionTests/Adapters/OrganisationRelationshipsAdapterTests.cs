using System;
using System.Linq;
using BuyingCatalogueFunction.IncrementalUpdate.Adapters;
using BuyingCatalogueFunction.IncrementalUpdate.Models.Ods;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace BuyingCatalogueFunctionTests.Adapters
{
    public static class OrganisationRelationshipsAdapterTests
    {
        [Theory]
        [CommonAutoData]
        public static void Process_InputIsNull_ThrowsException(
            OrganisationRelationshipsAdapter adapter)
        {
            FluentActions
                .Invoking(() => adapter.Process(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [CommonAutoData]
        public static void Process_InputIsNotNull_ReturnsExpectedResult(
            Org input,
            OrganisationRelationshipsAdapter adapter)
        {
            var result = adapter.Process(input).ToList();

            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Count.Should().Be(input.Rels.Rel.Count);

            foreach (var output in result)
            {
                var existing = input.Rels.Rel.FirstOrDefault(x => x.uniqueRelId == output.Id);

                existing.Should().NotBeNull();

                output.OwnerOrganisationId.Should().Be(input.OrgId.extension);
                output.TargetOrganisationId.Should().Be(existing!.Target.OrgId.extension);
                output.RelationshipTypeId.Should().Be(existing.id);
            }
        }
    }
}
