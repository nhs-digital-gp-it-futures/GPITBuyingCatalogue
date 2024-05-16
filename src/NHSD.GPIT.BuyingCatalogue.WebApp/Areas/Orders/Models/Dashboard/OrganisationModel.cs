using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Views.Shared.TagHelpers.Tags;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Dashboard
{
    public sealed class OrganisationModel : OrderingBaseModel
    {
        public OrganisationModel()
        {
        }

        public OrganisationModel(Organisation organisation, IEnumerable<Organisation> secondaryOrganisations, IList<Order> orders)
        {
            ArgumentNullException.ThrowIfNull(organisation, nameof(organisation));

            Title = organisation.Name;
            OrganisationName = organisation.Name;
            InternalOrgId = organisation.InternalIdentifier;
            UserOrganisations = secondaryOrganisations
                .Select(x => new SelectOption<string>(x.Name, x.InternalIdentifier))
                .ToList();
            SelectedOrganisationId = organisation.InternalIdentifier;

            Orders = orders ?? Enumerable.Empty<Order>().ToList();
        }

        public string OrganisationName { get; set; }

        /// <summary>
        /// Gets a value indicating whether the user can act on behalf of other organisations.
        /// </summary>
        /// <remarks>
        /// <see cref="UserOrganisations"/> contains the primary and proxy organisations.
        /// At a minimum, there will be a single organisation in <see cref="UserOrganisations"/> that is the user's primary organisation.
        /// As such, this will return true if the number of organisations is greater than 1.
        /// </remarks>
        public bool CanActOnBehalf => UserOrganisations.Count > 1;

        public IList<Order> Orders { get; set; }

        public IList<SelectOption<string>> UserOrganisations { get; set; } =
            Enumerable.Empty<SelectOption<string>>().ToList();

        public string SelectedOrganisationId { get; set; }

        public PageOptions Options { get; set; }

        public NhsTagsTagHelper.TagColour TagColour(OrderStatus orderStatus) => orderStatus switch
        {
            OrderStatus.Terminated => NhsTagsTagHelper.TagColour.Red,
            OrderStatus.Completed => NhsTagsTagHelper.TagColour.Green,
            OrderStatus.InProgress => NhsTagsTagHelper.TagColour.Blue,
            _ => throw new ArgumentOutOfRangeException(nameof(orderStatus)),
        };
    }
}
