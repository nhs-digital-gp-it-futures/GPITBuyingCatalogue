﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.RecipientsModels;

public static class SelectRecipientsModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Organisation organisation,
        List<ServiceRecipientModel> serviceRecipients,
        List<string> existingRecipients,
        List<string> preSelectedRecipients)
    {
        var model = new SelectRecipientsModel(
            organisation,
            serviceRecipients,
            existingRecipients,
            Enumerable.Empty<string>(),
            preSelectedRecipients);

        var groupedSubLocations = serviceRecipients.GroupBy(x => x.Location)
            .Select(
                x => new SublocationModel(
                    x.Key,
                    x.OrderBy(y => y.Name).ToList()))
            .OrderBy(x => x.Name)
            .ToArray();

        model.OrganisationName.Should().Be(organisation.Name);
        model.OrganisationType.Should().Be(organisation.OrganisationType.GetValueOrDefault());

        model.SubLocations.Should().BeEquivalentTo(groupedSubLocations);
    }

    [Theory]
    [MockInlineAutoData(SelectionMode.All, true)]
    [MockInlineAutoData(SelectionMode.None, false)]
    public static void Construct_SelectionMode_SetsSelections(
        SelectionMode selectionMode,
        bool expectedSelection,
        Organisation organisation,
        List<ServiceRecipientModel> serviceRecipients,
        List<string> existingRecipients,
        List<string> preSelectedRecipients)
    {
        serviceRecipients.ForEach(x => x.Selected = false);

        var model = new SelectRecipientsModel(
            organisation,
            serviceRecipients,
            existingRecipients,
            Enumerable.Empty<string>(),
            preSelectedRecipients,
            selectionMode);

        model.GetServiceRecipients().Should().AllSatisfy(x => { x.Selected.Should().Be(expectedSelection); });
    }

    [Theory]
    [MockAutoData]
    public static void Construct_ImportedRecipients_SelectsImportedRecipients(
        Organisation organisation,
        List<ServiceRecipientModel> serviceRecipients,
        List<string> existingRecipients)
    {
        serviceRecipients.ForEach(x => x.Selected = false);

        var preSelectedRecipients = serviceRecipients.Take(2).Select(x => x.OdsCode).ToList();

        var model = new SelectRecipientsModel(
            organisation,
            serviceRecipients,
            existingRecipients,
            Enumerable.Empty<string>(),
            preSelectedRecipients);

        model.GetSelectedServiceRecipients().Select(x => x.OdsCode).Should().BeEquivalentTo(preSelectedRecipients);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_InvalidImportedRecipients_DoesNotSelectImportedRecipients(
        Organisation organisation,
        List<ServiceRecipientModel> serviceRecipients,
        List<string> preSelectedRecipients,
        List<string> existingRecipients)
    {
        serviceRecipients.ForEach(x => x.Selected = false);

        var model = new SelectRecipientsModel(
            organisation,
            serviceRecipients,
            existingRecipients,
            Enumerable.Empty<string>(),
            preSelectedRecipients);

        model.HasSelectedRecipients().Should().BeFalse();
    }

    [Theory]
    [MockAutoData]
    public static void Construct_ImportedRecipients_OverridesExistingRecipients(
        Organisation organisation,
        List<ServiceRecipientModel> serviceRecipients)
    {
        serviceRecipients.ForEach(x => x.Selected = false);

        var preSelectedRecipients = serviceRecipients.Take(2).Select(x => x.OdsCode).ToList();
        var existingRecipients = serviceRecipients.Skip(2).Select(x => x.OdsCode).ToList();

        var model = new SelectRecipientsModel(
            organisation,
            serviceRecipients,
            existingRecipients,
            Enumerable.Empty<string>(),
            preSelectedRecipients);

        model.GetSelectedServiceRecipients().Select(x => x.OdsCode).Should().BeEquivalentTo(preSelectedRecipients);
    }
}
