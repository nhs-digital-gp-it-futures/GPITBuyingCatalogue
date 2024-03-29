﻿using System;
using AutoFixture;
using AutoFixture.Kernel;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class CallOffIdCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<CallOffId>(_ => new CallOffIdSpecimenBuilder());
        }

        private sealed class CallOffIdSpecimenBuilder : ISpecimenBuilder
        {
            public object Create(object request, ISpecimenContext context)
            {
                if (!(request as Type == typeof(CallOffId)))
                    return new NoSpecimen();

                var orderNumber = (context.Create<int>() % CallOffId.MaxOrderNumber) + 1;
                var revision = (context.Create<int>() % CallOffId.MaxRevision) + 1;

                return new CallOffId(orderNumber, revision);
            }
        }
    }
}
