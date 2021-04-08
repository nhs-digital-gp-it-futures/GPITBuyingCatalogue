using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using System;
using System.Collections.Generic;


namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.Builders
{
    public sealed class ApplicationUserBuilder
    {
        private static readonly IDictionary<
            OrganisationFunction,
            Func<ApplicationUserBuilder, AspNetUser>> ApplicationUserFactory =
            new Dictionary<OrganisationFunction, Func<ApplicationUserBuilder, AspNetUser>>
            {
                {
                    OrganisationFunction.Authority, builder =>
                        CreateAuthority(
                            builder.username,
                            builder.firstName,
                            builder.lastName,
                            builder.phoneNumber,
                            builder.emailAddress,
                            builder.primaryOrganisationId)
                },
                {
                    OrganisationFunction.Buyer, builder =>
                        CreateBuyer(
                            builder.username,
                            builder.firstName,
                            builder.lastName,
                            builder.phoneNumber,
                            builder.emailAddress,
                            builder.primaryOrganisationId)
                },
            };

        private string userId;
        private string firstName;
        private string lastName;
        private string phoneNumber;
        private string emailAddress;
        private string username;
        private Guid primaryOrganisationId;
        private bool disabled;
        private bool catalogueAgreementSigned;
        private OrganisationFunction organisationFunction;

        private ApplicationUserBuilder()
        {
            userId = Guid.NewGuid().ToString();
            firstName = "Bob";
            lastName = "Smith";
            phoneNumber = "0123456789";
            emailAddress = "a.b@c.com";
            username = emailAddress;
            primaryOrganisationId = Guid.NewGuid();
            catalogueAgreementSigned = false;
            organisationFunction = OrganisationFunction.Buyer;
        }

        public static ApplicationUserBuilder Create() => new();

        internal ApplicationUserBuilder WithUserId(string id)
        {
            userId = id;
            return this;
        }

        public ApplicationUserBuilder WithFirstName(string name)
        {
            firstName = name;
            return this;
        }

        public ApplicationUserBuilder WithLastName(string name)
        {
            lastName = name;
            return this;
        }

        internal ApplicationUserBuilder WithPhoneNumber(string number)
        {
            phoneNumber = number;
            return this;
        }

        public ApplicationUserBuilder WithEmailAddress(string address)
        {
            emailAddress = address;
            return this;
        }

        internal ApplicationUserBuilder WithUsername(string name)
        {
            username = name;
            return this;
        }

        internal ApplicationUserBuilder WithPrimaryOrganisationId(Guid id)
        {
            primaryOrganisationId = id;
            return this;
        }

        internal ApplicationUserBuilder WithOrganisationFunction(OrganisationFunction function)
        {
            organisationFunction = function;
            return this;
        }

        internal ApplicationUserBuilder WithDisabled(bool isDisabled)
        {
            disabled = isDisabled;
            return this;
        }

        internal ApplicationUserBuilder WithCatalogueAgreementSigned(bool agreementSigned)
        {
            catalogueAgreementSigned = agreementSigned;
            return this;
        }

        public AspNetUser Build() => CreateUserByOrganisationFunction();

        private AspNetUser CreateUserByOrganisationFunction()
        {
            if (!ApplicationUserFactory.TryGetValue(organisationFunction, out var factory))
            {
                throw new InvalidOperationException($"Unknown type of user '{organisationFunction?.DisplayName}'");
            }

            var user = factory(this);
            user.Id = userId;

            if (disabled)
            {
                user.Disabled = true;
            }

            if (catalogueAgreementSigned)
            {
                user.CatalogueAgreementSigned = true;
            }

            return user;
        }

        private static AspNetUser CreateBuyer(
            string userName,
            string firstName,
            string lastName,
            string phoneNumber,
            string email,
            Guid primaryOrganisationId)
        {
            return new AspNetUser
            {
                UserName = userName,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                Email = email,
                OrganisationFunction = OrganisationFunction.Buyer.DisplayName,
                PrimaryOrganisationId = primaryOrganisationId

            };
        }

        private static AspNetUser CreateAuthority(
            string userName,
            string firstName,
            string lastName,
            string phoneNumber,
            string email,
            Guid primaryOrganisationId)
        {
            return new AspNetUser
            {
                UserName = userName,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                Email = email,
                OrganisationFunction = OrganisationFunction.Authority.DisplayName,
                PrimaryOrganisationId = primaryOrganisationId

            };
        }
    }
}
