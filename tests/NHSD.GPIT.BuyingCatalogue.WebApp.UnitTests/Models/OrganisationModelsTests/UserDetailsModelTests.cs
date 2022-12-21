using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models.OrganisationModelsTests
{
    public static class UserDetailsModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithOrganisationConstruction_PropertiesSetAsExpected(
            Organisation organisation)
        {
            var actual = new UserDetailsModel(organisation);

            actual.OrganisationName.Should().Be(organisation.Name);
            actual.UserId.Should().Be(0);
            actual.Title.Should().Be("Add user");
        }

        [Theory]
        [CommonAutoData]
        public static void WithUserConstruction_PropertiesSetAsExpected(
            Organisation organisation,
            AspNetUser user)
        {
            var actual = new UserDetailsModel(organisation, user);

            actual.OrganisationName.Should().Be(organisation.Name);
            actual.UserId.Should().Be(user.Id);
            actual.Title.Should().Be("Edit user");
            actual.FirstName.Should().Be(user.FirstName);
        }

        [Theory]
        [CommonAutoData]
        public static void IsDefaultAccountType_True_PropertiesSetAsExpected(
            Organisation organisation)
        {
            var actual = new UserDetailsModel(organisation) { OrganisationId = 100, IsDefaultAccountType = true };
            actual.SelectedAccountType.Should().Be(OrganisationFunction.Buyer.Name);
        }

        [Theory]
        [CommonAutoData]
        public static void IsDefaultAccountType_False_PropertiesSetAsExpected(
            Organisation organisation)
        {
            var actual = new UserDetailsModel(organisation) { OrganisationId = 100, IsDefaultAccountType = false };
            actual.SelectedAccountType.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void IsNhsDigitalOrganisation_True_PropertiesSetAsExpected(
            Organisation organisation)
        {
            var actual = new UserDetailsModel(organisation)
            {
                IsDefaultAccountType = false, OrganisationId = OrganisationConstants.NhsDigitalOrganisationId,
            };

            actual.SelectedAccountType.Should().Be(OrganisationFunction.Authority.Name);
        }

        [Theory]
        [CommonAutoData]
        public static void InputFields_IncludesWhitespace_WhitespaceRemoved(
            string firstName,
            string lastName,
            string email,
            Organisation organisation)
        {
            var actual = new UserDetailsModel(organisation)
            {
                FirstName = " " + firstName + " ",
                LastName = " " + lastName + "  ",
                EmailAddress = " " + email + "  ",
            };

            actual.FirstName.Should().Be(firstName.Trim());
            actual.LastName.Should().Be(lastName.Trim());
            actual.EmailAddress.Should().Be(email.Trim());
        }
    }
}
