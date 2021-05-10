using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.Comparers
{
    public sealed class AspNetUserEditableInformationComparer : IEqualityComparer<AspNetUser>
    {
        private static readonly Lazy<AspNetUserEditableInformationComparer> ComparerInstance =
            new(() => new AspNetUserEditableInformationComparer());

        private AspNetUserEditableInformationComparer()
        {
        }

        public static AspNetUserEditableInformationComparer Instance
        {
            get
            {
                return ComparerInstance.Value;
            }
        }

        public bool Equals(AspNetUser original, AspNetUser comparison)
        {
            if (original is null)
                return comparison is null;

            if (comparison is null)
                return false;

            if (original.GetType() != comparison.GetType())
                return false;

            return string.Equals(original.FirstName, comparison.FirstName, StringComparison.Ordinal)
                && string.Equals(original.LastName, comparison.LastName, StringComparison.Ordinal)
                && string.Equals(original.PhoneNumber, comparison.PhoneNumber, StringComparison.Ordinal)
                && string.Equals(original.Email, comparison.Email, StringComparison.Ordinal)
                && string.Equals(original.NormalizedEmail, comparison.NormalizedEmail, StringComparison.Ordinal)
                && string.Equals(original.UserName, comparison.UserName, StringComparison.Ordinal)
                && string.Equals(original.NormalizedUserName, comparison.NormalizedUserName, StringComparison.Ordinal)
                && Equals(original.PrimaryOrganisationId, comparison.PrimaryOrganisationId)
                && Equals(original.OrganisationFunction, comparison.OrganisationFunction)
                && Equals(original.Disabled, comparison.Disabled)
                && Equals(original.CatalogueAgreementSigned, comparison.CatalogueAgreementSigned);
        }

        public int GetHashCode(AspNetUser obj)
        {
            return HashCode.Combine(
                obj.FirstName,
                obj.LastName,
                obj.PhoneNumber,
                obj.Email,
                obj.PrimaryOrganisationId,
                obj.OrganisationFunction,
                obj.Disabled,
                obj.CatalogueAgreementSigned);
        }
    }
}
