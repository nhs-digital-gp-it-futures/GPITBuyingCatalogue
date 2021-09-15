﻿using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class ClientApplicationTypeSectionModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void FromCatalogueItem_ValidCatalogueItem_PropertiesSetAsExpected(
            [Frozen] CatalogueItem catalogueItem,
            ClientApplicationTypeSectionModel expected)
        {
            var actual = new ClientApplicationTypeSectionModel(catalogueItem);

            actual.SolutionId.Should().BeEquivalentTo(expected.SolutionId);
            actual.SolutionName.Should().BeEquivalentTo(expected.SolutionName);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new ClientApplicationTypeSectionModel(null));

            actual.ParamName.Should().Be("catalogueItem");
        }

        [Theory]
        [CommonAutoData]
        public static void StatusClientApplicationType_AvailableType_ReturnsCompleted(
            CatalogueItem catalogueItem,
            ClientApplication clientApplication)
        {
            clientApplication.ClientApplicationTypes = new HashSet<string> { "browser-based" };
            catalogueItem.Solution.ClientApplication = JsonConvert.SerializeObject(clientApplication);

            var model = new ClientApplicationTypeSectionModel(catalogueItem);

            var actual = model.ClientApplicationTypeStatus();

            actual.Should().Be(TaskProgress.Completed);
        }

        [Fact]
        public static void StatusClientApplicationType_NoApplicationTypeAdded_ReturnsNotStarted()
        {
            var model = new ClientApplicationTypeSectionModel();

            var actual = model.ClientApplicationTypeStatus();

            actual.Should().Be(TaskProgress.NotStarted);
        }
    }
}
