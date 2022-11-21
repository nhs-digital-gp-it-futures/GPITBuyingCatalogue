using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using OrganisationImporter.Models;

namespace OrganisationImporterTests.AutoFixtureCustomizations;

public class CodeSystemsCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        ISpecimenBuilder ComposerTransformation(ICustomizationComposer<CodeSystems> composer) => composer
            .With(x => x.CodeSystem, () => CreateCodeSystems(fixture));

        fixture.Customize<CodeSystems>(ComposerTransformation);
    }

    private static List<CodeSystem> CreateCodeSystems(ISpecimenBuilder fixture)
    {
        var codeSystems = fixture.CreateMany<CodeSystem>(5).ToList();

        codeSystems!.First().Name = TrudCodeSystemKeys.RolesKey;
        codeSystems.Skip(1).First().Name = TrudCodeSystemKeys.RelationshipKey;

        return codeSystems;
    }
}
