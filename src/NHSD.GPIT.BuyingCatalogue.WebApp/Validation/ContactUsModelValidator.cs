using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation
{
    public class ContactUsModelValidator : AbstractValidator<ContactUsModel>
    {
        public ContactUsModelValidator()
        {
            RuleFor(m => m.FullName)
                .NotEmpty();

            RuleFor(m => m.Message)
                .NotEmpty();

            RuleFor(m => m.EmailAddress)
                .EmailAddress()
                .NotEmpty();

            RuleFor(m => m.ContactMethod)
                .NotEmpty();

            RuleFor(m => m.PrivacyPolicyAccepted)
                .Equal(true);
        }
    }
}
