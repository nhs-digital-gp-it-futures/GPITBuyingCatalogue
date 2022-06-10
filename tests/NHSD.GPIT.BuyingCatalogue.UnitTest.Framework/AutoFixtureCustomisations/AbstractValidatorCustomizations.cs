using System;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using FluentValidation;

namespace NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations
{
    public sealed class AbstractValidatorCustomizations : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var behaviour = new AbstractValidatorTransformation();

            fixture.Behaviors.Add(behaviour);
        }

        private sealed class AbstractValidatorTransformation : ISpecimenBuilderTransformation
        {
            public ISpecimenBuilderNode Transform(ISpecimenBuilder builder)
            {
                var command = new AbstractValidatorPostProcessorCommand();
                var spec = new IsAbstractValidator();

                return new Postprocessor(builder, command, spec);
            }
        }

        private sealed class IsAbstractValidator : IRequestSpecification
        {
            public bool IsSatisfiedBy(object request)
            {
                var type = request as Type;

                return
                    type != null
                    && type.BaseType != null
                    && type.BaseType.IsGenericType
                    && type.BaseType.GetGenericTypeDefinition() == typeof(AbstractValidator<>);
            }
        }

        private sealed class AbstractValidatorPostProcessorCommand : ISpecimenCommand
        {
            public void Execute(object specimen, ISpecimenContext context)
            {
                var type = specimen.GetType();

                var classRule = type.GetProperty("ClassLevelCascadeMode");

                if (classRule != null && classRule.CanWrite)
                    classRule.SetValue(specimen, CascadeMode.Continue);

                var ruleRule = type.GetProperty("RuleLevelCascadeMode");

                if (ruleRule != null && ruleRule.CanWrite)
                    ruleRule.SetValue(specimen, CascadeMode.Stop);
            }
        }
    }
}
