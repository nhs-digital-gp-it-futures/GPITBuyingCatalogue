﻿using AutoFixture;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public class SignInManagerSubstituteCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<QueueServiceClient>(_ => new SubstituteRelaySpecimenBuilder<SignInManager<AspNetUser>>());
        }
    }
}
