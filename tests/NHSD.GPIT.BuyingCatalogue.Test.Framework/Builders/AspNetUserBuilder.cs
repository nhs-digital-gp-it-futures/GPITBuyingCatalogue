using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.Test.Framework.Builders
{
    public sealed class AspNetUserBuilder
    {
        private static readonly IDictionary<
            OrganisationFunction,
            Func<AspNetUserBuilder, AspNetUser>> ApplicationUserFactory =
            new Dictionary<OrganisationFunction, Func<AspNetUserBuilder, AspNetUser>>
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

        private Guid userId;
        private string firstName;
        private string lastName;
        private string phoneNumber;
        private string emailAddress;
        private string username;
        private Guid primaryOrganisationId;
        private bool disabled;
        private bool catalogueAgreementSigned;
        private OrganisationFunction organisationFunction;

        private AspNetUserBuilder()
        {
            userId = Guid.NewGuid();
            firstName = "Bob";
            lastName = "Smith";
            phoneNumber = "0123456789";
            emailAddress = "a.b@c.com";
            username = emailAddress;
            primaryOrganisationId = Guid.NewGuid();
            catalogueAgreementSigned = false;
            organisationFunction = OrganisationFunction.Buyer;
        }

        public static AspNetUserBuilder Create() => new();

        public AspNetUserBuilder WithFirstName(string name)
        {
            firstName = name;
            return this;
        }

        public AspNetUserBuilder WithLastName(string name)
        {
            lastName = name;
            return this;
        }

        public AspNetUserBuilder WithPhoneNumber(string number)
        {
            phoneNumber = number;
            return this;
        }

        public AspNetUserBuilder WithEmailAddress(string address)
        {
            emailAddress = address;
            return this;
        }

        public AspNetUserBuilder WithPrimaryOrganisationId(Guid id)
        {
            primaryOrganisationId = id;
            return this;
        }

        public AspNetUser Build() => CreateUserByOrganisationFunction();

        internal AspNetUserBuilder WithUserId(Guid id)
        {
            userId = id;
            return this;
        }

        internal AspNetUserBuilder WithUsername(string name)
        {
            username = name;
            return this;
        }

        internal AspNetUserBuilder WithOrganisationFunction(OrganisationFunction function)
        {
            organisationFunction = function;
            return this;
        }

        internal AspNetUserBuilder WithDisabled(bool isDisabled)
        {
            disabled = isDisabled;
            return this;
        }

        internal AspNetUserBuilder WithCatalogueAgreementSigned(bool agreementSigned)
        {
            catalogueAgreementSigned = agreementSigned;
            return this;
        }

        private static AspNetUser CreateBuyer(
            string userName,
            string firstName,
            string lastName,
            string phoneNumber,
            string email,
            Guid primaryOrganisationId)
        {
            return new()
            {
                UserName = userName,
                NormalizedUserName = userName.ToUpper(),
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                Email = email,
                NormalizedEmail = email.ToUpper(),
                OrganisationFunction = OrganisationFunction.Buyer.DisplayName,
                PrimaryOrganisationId = primaryOrganisationId,
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
            return new()
            {
                UserName = userName,
                NormalizedUserName = userName.ToUpper(),
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                Email = email,
                NormalizedEmail = email.ToUpper(),
                OrganisationFunction = OrganisationFunction.Authority.DisplayName,
                PrimaryOrganisationId = primaryOrganisationId,
            };
        }

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
    }
}
