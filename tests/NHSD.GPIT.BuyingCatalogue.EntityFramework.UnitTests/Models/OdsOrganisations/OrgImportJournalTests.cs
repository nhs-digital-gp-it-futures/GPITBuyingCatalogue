using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.OdsOrganisations;

public static class OrgImportJournalTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        DateTime dateTime)
    {
        var model = new OrgImportJournal(dateTime);

        model.ImportDate.Should().Be(dateTime);
    }
}
