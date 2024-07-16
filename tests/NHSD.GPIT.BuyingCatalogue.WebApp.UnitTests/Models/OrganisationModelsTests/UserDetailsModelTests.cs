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
        [MockAutoData]
        public static void WithOrganisationConstruction_PropertiesSetAsExpected(
            Organisation organisation, int maxNumberAccountManagers)
        {
            var actual = new UserDetailsModel(organisation)
            {
                MaxNumberOfAccountManagers = maxNumberAccountManagers,
            };

            actual.OrganisationName.Should().Be(organisation.Name);
            actual.UserId.Should().Be(0);
            actual.Title.Should().Be("Add user");
            actual.MaximumAccountManagerMessage.Should().Be(
                $"You can only add buyers for this organisation. This is because there are already {maxNumberAccountManagers} active account managers which is the maximum allowed.");
        }

        [Theory]
        [MockAutoData]
        public static void WithUserConstruction_PropertiesSetAsExpected(
            Organisation organisation,
            AspNetUser user,
            int maxNumberAccountManagers)
        {
            var actual = new UserDetailsModel(organisation, user)
            {
                MaxNumberOfAccountManagers = maxNumberAccountManagers,
            };

            actual.OrganisationName.Should().Be(organisation.Name);
            actual.UserId.Should().Be(user.Id);
            actual.Title.Should().Be("Edit user");
            actual.FirstName.Should().Be(user.FirstName);
            actual.MaximumAccountManagerMessage.Should().Be(
                $"You cannot make this user an account manager. This is because there are already {maxNumberAccountManagers} active account managers for this organisation, which is the maximum allowed.");
        }

        [Theory]
        [MockAutoData]
        public static void IsDefaultAccountType_True_PropertiesSetAsExpected(
            Organisation organisation)
        {
            var actual = new UserDetailsModel(organisation) { OrganisationId = 100, IsDefaultAccountType = true };
            actual.SelectedAccountType.Should().Be(OrganisationFunction.Buyer.Name);
        }

        [Theory]
        [MockAutoData]
        public static void IsDefaultAccountType_False_PropertiesSetAsExpected(
            Organisation organisation)
        {
            var actual = new UserDetailsModel(organisation) { OrganisationId = 100, IsDefaultAccountType = false };
            actual.SelectedAccountType.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
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
