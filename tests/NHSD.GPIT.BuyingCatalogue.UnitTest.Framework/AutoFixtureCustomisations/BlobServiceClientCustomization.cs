using System;
using AutoFixture;
using AutoFixture.Kernel;
using Azure.Storage.Blobs;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;

public class BlobServiceClientCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<BlobServiceClient>(_ => new BlobServiceClientSpecimenBuilder());
    }

    private sealed class BlobServiceClientSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (!(request as Type == typeof(BlobServiceClient)))
                return new NoSpecimen();

            return new BlobServiceClient("DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;");
        }
    }
}
