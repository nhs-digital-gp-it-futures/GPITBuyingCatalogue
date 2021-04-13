using System;
using System.Collections.Generic;
using FluentAssertions;
using HealthChecks.Network;
using HealthChecks.Network.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions.DependencyInjection;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.DependencyInjection
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class HealthChecksBuilderExtensionsTests
    {
        #region SmtpHealthCheck Tests

        [Test]
        public static void AddSmtpHealthCheck_NullHealthChecksBuilder_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => HealthChecksBuilderExtensions.AddSmtpHealthCheck(null!, new SmtpSettings()));
        }

        [Test]
        public static void AddSmtpHealthCheck_NullSmtpSettings_ThrowsArgumentNullException()
        {
            var builder = Mock.Of<IHealthChecksBuilder>();

            Assert.Throws<ArgumentNullException>(() => HealthChecksBuilderExtensions.AddSmtpHealthCheck(builder, null!));
        }

        [Test]
        public static void AddSmtpHealthCheck_SetsExpectedFailureStatus()
        {
            AddSmtpHealthCheckArguments healthCheckArguments = null;

            var mockBuilder = new Mock<IHealthChecksBuilder>();
            mockBuilder.Setup(b => b.Add(It.IsNotNull<HealthCheckRegistration>()))
                .Callback<HealthCheckRegistration>(r => AddSmtpHealthCheckCallback(r, out healthCheckArguments));

            var builder = mockBuilder.Object;
            builder.AddSmtpHealthCheck(new SmtpSettings());

            healthCheckArguments.FailureStatus.Should().Be(HealthStatus.Degraded);
        }

        [Test]
        public static void AddSmtpHealthCheck_SetsExpectedName()
        {
            AddSmtpHealthCheckArguments healthCheckArguments = null;

            var mockBuilder = new Mock<IHealthChecksBuilder>();
            mockBuilder.Setup(b => b.Add(It.IsNotNull<HealthCheckRegistration>()))
                .Callback<HealthCheckRegistration>(r => AddSmtpHealthCheckCallback(r, out healthCheckArguments));

            var builder = mockBuilder.Object;
            builder.AddSmtpHealthCheck(new SmtpSettings());

            healthCheckArguments.Name.Should().Be(HealthCheck.Name);
        }

        [Test]
        public static void AddSmtpHealthCheck_NullTags_SetsDefaultTags()
        {
            AddSmtpHealthCheckArguments healthCheckArguments = null;

            var mockBuilder = new Mock<IHealthChecksBuilder>();
            mockBuilder.Setup(b => b.Add(It.IsNotNull<HealthCheckRegistration>()))
                .Callback<HealthCheckRegistration>(r => AddSmtpHealthCheckCallback(r, out healthCheckArguments));

            var builder = mockBuilder.Object;
            builder.AddSmtpHealthCheck(new SmtpSettings());

            healthCheckArguments.Tags.Should().BeEquivalentTo(HealthCheck.DefaultTags);
        }

        [Test]
        public static void AddSmtpHealthCheck_SetsAllowInvalidRemoteCertificatesOption()
        {
            AddSmtpHealthCheckArguments healthCheckArguments = null;

            var mockBuilder = new Mock<IHealthChecksBuilder>();
            mockBuilder.Setup(b => b.Add(It.IsNotNull<HealthCheckRegistration>()))
                .Callback<HealthCheckRegistration>(r => AddSmtpHealthCheckCallback(r, out healthCheckArguments));

            var settings = new SmtpSettings { AllowInvalidCertificate = true };

            var builder = mockBuilder.Object;
            builder.AddSmtpHealthCheck(settings);

            healthCheckArguments.Options.AllowInvalidRemoteCertificates.Should().BeTrue();
        }

        [Test]
        public static void AddSmtpHealthCheck_SetsConnectionTypeOption()
        {
            AddSmtpHealthCheckArguments healthCheckArguments = null;

            var mockBuilder = new Mock<IHealthChecksBuilder>();
            mockBuilder.Setup(b => b.Add(It.IsNotNull<HealthCheckRegistration>()))
                .Callback<HealthCheckRegistration>(r => AddSmtpHealthCheckCallback(r, out healthCheckArguments));

            var builder = mockBuilder.Object;
            builder.AddSmtpHealthCheck(new SmtpSettings());

            healthCheckArguments.Options.ConnectionType.Should().Be(SmtpConnectionType.TLS);
        }

        [Test]
        public static void AddSmtpHealthCheck_SetsHostOption()
        {
            const string expectedHost = "myHost";

            AddSmtpHealthCheckArguments healthCheckArguments = null;

            var mockBuilder = new Mock<IHealthChecksBuilder>();
            mockBuilder.Setup(b => b.Add(It.IsNotNull<HealthCheckRegistration>()))
                .Callback<HealthCheckRegistration>(r => AddSmtpHealthCheckCallback(r, out healthCheckArguments));

            var settings = new SmtpSettings { Host = expectedHost };

            var builder = mockBuilder.Object;
            builder.AddSmtpHealthCheck(settings);

            healthCheckArguments.Options.Host.Should().Be(expectedHost);
        }

        [Test]
        public static void AddSmtpHealthCheck_SetsPortOption()
        {
            const int expectedPort = 1234;

            AddSmtpHealthCheckArguments healthCheckArguments = null;

            var mockBuilder = new Mock<IHealthChecksBuilder>();
            mockBuilder.Setup(b => b.Add(It.IsNotNull<HealthCheckRegistration>()))
                .Callback<HealthCheckRegistration>(r => AddSmtpHealthCheckCallback(r, out healthCheckArguments));

            var settings = new SmtpSettings { Port = expectedPort };

            var builder = mockBuilder.Object;
            builder.AddSmtpHealthCheck(settings);

            healthCheckArguments.Options.Port.Should().Be(expectedPort);
        }

        [Test]
        public static void AddSmtpHealthCheck_SetsExpectedTags()
        {
            AddSmtpHealthCheckArguments healthCheckArguments = null;

            var mockBuilder = new Mock<IHealthChecksBuilder>();
            mockBuilder.Setup(b => b.Add(It.IsNotNull<HealthCheckRegistration>()))
                .Callback<HealthCheckRegistration>(r => AddSmtpHealthCheckCallback(r, out healthCheckArguments));

            var tags = new[] { "myTag" };

            var builder = mockBuilder.Object;
            builder.AddSmtpHealthCheck(new SmtpSettings(), tags);

            healthCheckArguments.Tags.Should().BeEquivalentTo(tags);
        }

        [Test]
        public static void AddSmtpHealthCheck_NullTimeout_SetsDefaultTimeout()
        {
            AddSmtpHealthCheckArguments healthCheckArguments = null;

            var mockBuilder = new Mock<IHealthChecksBuilder>();
            mockBuilder.Setup(b => b.Add(It.IsNotNull<HealthCheckRegistration>()))
                .Callback<HealthCheckRegistration>(r => AddSmtpHealthCheckCallback(r, out healthCheckArguments));

            var builder = mockBuilder.Object;
            builder.AddSmtpHealthCheck(new SmtpSettings());

            healthCheckArguments.Timeout.Should().Be(HealthCheck.DefaultTimeout);
        }

        [Test]
        public static void AddSmtpHealthCheck_SetsExpectedTimeout()
        {
            AddSmtpHealthCheckArguments healthCheckArguments = null;

            var mockBuilder = new Mock<IHealthChecksBuilder>();
            mockBuilder.Setup(b => b.Add(It.IsNotNull<HealthCheckRegistration>()))
                .Callback<HealthCheckRegistration>(r => AddSmtpHealthCheckCallback(r, out healthCheckArguments));

            var timeout = TimeSpan.FromSeconds(90);

            var builder = mockBuilder.Object;
            builder.AddSmtpHealthCheck(new SmtpSettings(), timeout: timeout);

            healthCheckArguments.Timeout.Should().Be(timeout);
        }

        #endregion

        #region DatabaseHealthCheck Tests

        [Test]
        public static void AddDatabaseHealthCheck_NullHealthChecksBuilder_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => HealthChecksBuilderExtensions.AddDatabaseHealthCheck(null!, "connection string"));
        }

        [Test]
        public static void AddDatabaseHealthCheck_NullConnectionString_ThrowsArgumentNullException()
        {
            var builder = Mock.Of<IHealthChecksBuilder>();

            Assert.Throws<ArgumentNullException>(() => HealthChecksBuilderExtensions.AddDatabaseHealthCheck(builder, null!));
        }

        [Test]
        public static void AddDatabaseHealthCheck_EmptyConnectionString_ThrowsArgumentException()
        {
            var builder = Mock.Of<IHealthChecksBuilder>();

            Assert.Throws<ArgumentException>(() => HealthChecksBuilderExtensions.AddDatabaseHealthCheck(builder, " "));
        }

        #endregion

        private static void AddSmtpHealthCheckCallback(
            HealthCheckRegistration registration,
            out AddSmtpHealthCheckArguments arguments)
        {
            var factoryTarget = registration.Factory.Target;
            var options = factoryTarget
                ?.GetType()
                .GetField("options")
                ?.GetValue(factoryTarget) as SmtpHealthCheckOptions;

            arguments = new AddSmtpHealthCheckArguments
            {
                FailureStatus = registration.FailureStatus,
                Name = registration.Name,
                Options = options,
                Tags = registration.Tags,
                Timeout = registration.Timeout,
            };
        }

        private sealed class AddSmtpHealthCheckArguments
        {
            internal HealthStatus? FailureStatus { get; init; }

            internal string Name { get; init; }

            internal SmtpHealthCheckOptions Options { get; init; }

            internal ICollection<string> Tags { get; init; }

            internal TimeSpan? Timeout { get; init; }
        }      
    }
}
